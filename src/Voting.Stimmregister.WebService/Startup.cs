// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Globalization;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using Voting.Lib.Common.DependencyInjection;
using Voting.Lib.Cryptography.Extensions;
using Voting.Lib.Grpc.DependencyInjection;
using Voting.Lib.Grpc.Interceptors;
using Voting.Lib.Rest.Middleware;
using Voting.Lib.Rest.Swagger.DependencyInjection;
using Voting.Stimmregister.Adapter.Data;
using Voting.Stimmregister.Adapter.Data.DependencyInjection;
using Voting.Stimmregister.Adapter.Ech.DependencyInjection;
using Voting.Stimmregister.Adapter.Hsm.DependencyInjection;
using Voting.Stimmregister.Adapter.VotingBasis.DependencyInjection;
using Voting.Stimmregister.Adapter.VotingIam.DependencyInjection;
using Voting.Stimmregister.Core.DependencyInjection;
using Voting.Stimmregister.Core.Import.Cobra.DependencyInjection;
using Voting.Stimmregister.Core.Import.Cobra.Tg.DependencyInjection;
using Voting.Stimmregister.Core.Import.Innosolv.DependencyInjection;
using Voting.Stimmregister.Core.Import.Loganto.DependencyInjection;
using Voting.Stimmregister.Domain.DependencyInjection;
using Voting.Stimmregister.WebService.Configuration;
using Voting.Stimmregister.WebService.Converters;
using Voting.Stimmregister.WebService.DependencyInjection;
using Voting.Stimmregister.WebService.Interceptors;
using Voting.Stimmregister.WebService.Markers;
using Voting.Stimmregister.WebService.Middlewares;
using Voting.Stimmregister.WebService.Services;
using ExceptionHandler = Voting.Stimmregister.WebService.Middlewares.ExceptionHandler;
using ExceptionInterceptor = Voting.Stimmregister.WebService.Interceptors.ExceptionInterceptor;

namespace Voting.Stimmregister.WebService;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
        AppConfig = configuration.Get<AppConfig>()!;
    }

    protected AppConfig AppConfig { get; }

    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddWebServiceServices(AppConfig);
        services.AddDomainServices(AppConfig.Imports);
        services.AddCoreServices(AppConfig.MemoryCache, AppConfig.Filter, AppConfig.Person, AppConfig.Imports, AppConfig.Cleanup);
        services.AddIamServices(AppConfig.SecureConnect);
        services.AddAdapterDataServices(AppConfig.Database, ConfigureDatabase);
        services.AddAdapterEch(AppConfig.Ech);
        services.AddImportDependencies();
        services.AddAdapterLogantoServices();
        services.AddAdapterCobraServices();
        services.AddAdapterCobraTgServices();
        services.AddAdapterInnosolv();
        services.AddAdapterVotingBasisServices(AppConfig.Imports.VotingBasis);
        services.AddCertificatePinning(AppConfig.CertificatePinning);
        services.AddAdapterHsmServices(AppConfig.Hsm, AppConfig.EnablePkcs11Mock);
        services.AddVotingLibPrometheusAdapter(new() { Interval = AppConfig.PrometheusAdapterInterval });
        services.AddAutoMapper(
            typeof(ArchMarker),
            typeof(Abstractions.Adapter.Markers.ArchMarker),
            typeof(Core.Markers.ArchMarker),
            typeof(Domain.Markers.ArchMarker));

        ConfigureHealthChecks(services.AddHealthChecks());
        ConfigureAuthentication(services.AddVotingLibIam(new() { BaseUrl = AppConfig.SecureConnectApi }, AppConfig.AuthStore));

        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new OptionalGuidConverter());
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
            });

        services
            .AddGrpc(o =>
            {
                o.EnableDetailedErrors = AppConfig.EnableDetailedErrors;
                o.Interceptors.Add<ExceptionInterceptor>();
                o.Interceptors.Add<LanguageInterceptor>();
                o.Interceptors.Add<RequestProtoValidatorInterceptor>();
                o.MaxReceiveMessageSize = AppConfig.GrpcService.MaxGrpcMessageSizeBytes;
            });

        services.AddGrpcRequestLoggerInterceptor(_environment);

        if (AppConfig.GrpcService.EnableGrpcWeb)
        {
            services.AddCors(_configuration);
        }

        if (AppConfig.GrpcService.EnableGrpcReflection)
        {
            services.AddGrpcReflection();
        }

        services.AddProtoValidators();
        services.AddSwaggerGenerator(_configuration);

        if (AppConfig.EnableServerTiming)
        {
            services.AddSingleton<ServerTimingMiddleware>();
        }

        ConfigureGlobalFluentValidationOptions();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseMetricServer(AppConfig.MetricPort);
        app.UseRouting();
        app.UseHttpMetrics();
        app.UseGrpcMetrics();

        if (AppConfig.GrpcService.EnableGrpcWeb)
        {
            app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
            app.UseCorsFromConfig();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        if (AppConfig.EnableServerTiming)
        {
            app.UseMiddleware<ServerTimingMiddleware>();
        }

        app.UseMiddleware<ExceptionHandler>();
        app.UseMiddleware<TracingMiddleware>();
        app.UseMiddleware<LanguageMiddleware>();
        app.UseMiddleware<AccessControlListDoiMiddleware>();
        app.UseMiddleware<IamLoggingHandler>();
        app.UseSerilogRequestLoggingWithTraceabilityModifiers();

        app.UseSwaggerGenerator();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapVotingHealthChecks(AppConfig.LowPriorityHealthCheckNames);
            MapEndpoints(endpoints);
        });
    }

    protected virtual void ConfigureAuthentication(AuthenticationBuilder builder)
        => builder.AddSecureConnectScheme(options =>
        {
            options.Audience = AppConfig.SecureConnect.Audience;
            options.Authority = AppConfig.SecureConnect.Authority;
            options.FetchRoleToken = true;
            options.LimitRolesToAppHeaderApps = false;
            options.ServiceAccount = AppConfig.SecureConnect.ServiceAccount;
            options.ServiceAccountPassword = AppConfig.SecureConnect.ServiceAccountPassword;
            options.ServiceAccountScopes = AppConfig.SecureConnect.ServiceAccountScopes;
            options.AllowOnBehalfToken = AppConfig.SecureConnect.AllowOnBehalfToken;
            options.AllowAccessToken = AppConfig.SecureConnect.AllowAccessToken;
        });

    protected virtual void ConfigureDatabase(DbContextOptionsBuilder db)
        => db.UseNpgsql(AppConfig.Database.ConnectionString, o => o.SetPostgresVersion(AppConfig.Database.Version));

    /// <summary>
    /// Force using german for speaking fluent validation errors for the user.
    /// </summary>
    private static void ConfigureGlobalFluentValidationOptions()
        => ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("de-DE");

    private void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapControllers();

        if (AppConfig.GrpcService.EnableGrpcReflection)
        {
            endpoints.MapGrpcReflectionService().RequireAuthorization();
        }

        endpoints.MapGrpcService<PersonGrpcService>();
        endpoints.MapGrpcService<FilterGrpcService>();
        endpoints.MapGrpcService<ImportStatisticGrpcService>();
        endpoints.MapGrpcService<RegistrationStatisticGrpcService>();
        endpoints.MapGrpcService<EcollectingGrpcService>();
    }

    private void ConfigureHealthChecks(IHealthChecksBuilder checks)
    {
        checks
            .AddDbContextCheck<DataContext>()
            .AddCryptoProviderHealthCheck("Pkcs11")
            .ForwardToPrometheus();
    }
}

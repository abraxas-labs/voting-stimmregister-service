// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Testing;
using Voting.Stimmregister.Adapter.Data;
using Voting.Stimmregister.Domain.Authorization;
using Voting.Stimmregister.Test.Utils.MockData;

namespace Voting.Stimmregister.Test.Utils.Helpers;

public abstract class BaseGrpcTest<TService, TFactory, TStartup> : GrpcAuthorizationBaseTest<TFactory, TStartup>
    where TService : ClientBase<TService>
    where TFactory : BaseTestApplicationFactory<TStartup>
    where TStartup : class
{
    private readonly Lazy<TService> _lazyUnauthorizedClient;
    private readonly Lazy<TService> _lazySgManagerClient;
    private readonly Lazy<TService> _lazySgReaderClient;
    private readonly Lazy<TService> _lazyTgManagerClient;
    private readonly Lazy<TService> _lazyTgReaderClient;
    private readonly Lazy<TService> _lazyManualImporterClient;
    private readonly Lazy<TService> _lazyApiImporterClient;
    private readonly Lazy<TService> _lazyManualExporterClient;
    private readonly Lazy<TService> _lazyApiExporterClient;
    private readonly Lazy<TService> _lazyImportObserverClient;
    private readonly Lazy<TService> _lazyRegistrationStatisticReaderClient;
    private readonly Lazy<TService> _lazyUnknownRegistrationStatisticReaderClient;
    private readonly Lazy<TService> _lazyUnknownClient;

    protected BaseGrpcTest(TFactory factory)
        : base(factory)
    {
        _lazyUnauthorizedClient = new(() => CreateGrpcService(CreateGrpcChannel(false)));

        _lazySgManagerClient = new(() =>
            CreateGrpcService(CreateGrpcChannel(true, roles: new[] { Roles.Reader, Roles.Manager }, tenant: VotingIamTenantIds.KTSG)));

        _lazySgReaderClient = new(() =>
            CreateGrpcService(CreateGrpcChannel(true, roles: new[] { Roles.Reader }, tenant: VotingIamTenantIds.KTSG)));

        _lazyTgManagerClient = new(() =>
            CreateGrpcService(CreateGrpcChannel(true, roles: new[] { Roles.Reader, Roles.Manager }, tenant: VotingIamTenantIds.KTTG)));

        _lazyTgReaderClient = new(() =>
            CreateGrpcService(CreateGrpcChannel(true, roles: new[] { Roles.Reader }, tenant: VotingIamTenantIds.KTTG)));

        _lazyApiImporterClient = new(() =>
            CreateGrpcService(CreateGrpcChannel(true, roles: new[] { Roles.ApiImporter }, tenant: VotingIamTenantIds.Abraxas)));

        _lazyManualImporterClient = new(() =>
            CreateGrpcService(CreateGrpcChannel(true, roles: new[] { Roles.ManualImporter }, tenant: VotingIamTenantIds.KTSG)));

        _lazyUnknownClient = new(() =>
            CreateGrpcService(CreateGrpcChannel(true, roles: new[] { Roles.Reader }, tenant: VotingIamTenantIds.Unknown)));

        _lazyManualExporterClient = new(() =>
            CreateGrpcService(CreateGrpcChannel(true, roles: new[] { Roles.ManualExporter }, tenant: VotingIamTenantIds.Abraxas)));

        _lazyApiExporterClient = new(() =>
            CreateGrpcService(CreateGrpcChannel(true, roles: new[] { Roles.ApiExporter }, tenant: VotingIamTenantIds.KTSG)));

        _lazyImportObserverClient = new(() =>
            CreateGrpcService(CreateGrpcChannel(true, roles: new[] { Roles.ImportObserver, Roles.Reader }, tenant: VotingIamTenantIds.Abraxas)));

        _lazyRegistrationStatisticReaderClient = new(() =>
            CreateGrpcService(CreateGrpcChannel(true, roles: new[] { Roles.EVotingStatisticsReader }, tenant: VotingIamTenantIds.KTSG)));

        _lazyUnknownRegistrationStatisticReaderClient = new(() =>
            CreateGrpcService(CreateGrpcChannel(true, roles: new[] { Roles.EVotingStatisticsReader }, tenant: VotingIamTenantIds.Unknown)));
    }

    protected TService UnauthorizedClient => _lazyUnauthorizedClient.Value;

    protected TService UnknownClient => _lazyUnknownClient.Value;

    protected TService ManualImporterClient => _lazyManualImporterClient.Value;

    protected TService ApiImporterClient => _lazyApiImporterClient.Value;

    protected TService SgManagerClient => _lazySgManagerClient.Value;

    protected TService SgReaderClient => _lazySgReaderClient.Value;

    protected TService TgManagerClient => _lazyTgManagerClient.Value;

    protected TService TgReaderClient => _lazyTgReaderClient.Value;

    protected TService ManualExporterClient => _lazyManualExporterClient.Value;

    protected TService ApiExporterClient => _lazyApiExporterClient.Value;

    protected TService ImportObserverClient => _lazyImportObserverClient.Value;

    protected TService RegistrationStatisticReaderClient => _lazyRegistrationStatisticReaderClient.Value;

    protected TService UnknownRegistrationStatisticReaderClient => _lazyUnknownRegistrationStatisticReaderClient.Value;

    protected void RunOnDb(Action<DataContext> action)
        => RunScoped(action);

    protected TResult RunOnDb<TResult>(Func<DataContext, TResult> action)
        => RunScoped(action);

    protected Task RunOnDb(Func<DataContext, Task> action)
        => RunScoped(action);

    protected Task<TResult> RunOnDb<TResult>(Func<DataContext, Task<TResult>> action)
        => RunScoped(action);

    protected Task<TEntity> FindDbEntity<TEntity>(Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        return RunOnDb(db => db.Set<TEntity>().Where(predicate).SingleAsync());
    }

    protected Task<List<TEntity>> FindDbEntities<TEntity>(Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        return RunOnDb(db => db.Set<TEntity>().Where(predicate).ToListAsync());
    }

    protected TService CreateGrpcService(GrpcChannel channel)
        => (TService)Activator.CreateInstance(typeof(TService), channel)!;

    protected override Task AuthorizationTestCall(GrpcChannel channel)
        => AuthorizationTestCall(CreateGrpcService(channel));

    protected abstract Task AuthorizationTestCall(TService service);

    protected override IEnumerable<string> UnauthorizedRoles()
    {
        yield return NoRole;
    }
}

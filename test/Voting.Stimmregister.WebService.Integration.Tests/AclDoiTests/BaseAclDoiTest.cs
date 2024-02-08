// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading;
using System.Threading.Tasks;
using Abraxas.Voting.Basis.Services.V1;
using Abraxas.Voting.Basis.Services.V1.Models;
using Abraxas.Voting.Basis.Services.V1.Requests;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.VotingBasis;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Adapter.VotingBasis.Services;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Test.Utils.Helpers;

namespace Voting.Stimmregister.WebService.Integration.Tests.AclDoiTests;

public abstract class BaseAclDoiTest : BaseWriteableDbTest
{
    protected BaseAclDoiTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    protected Task ImportAcl(params PoliticalDomainOfInfluence[] acls)
    {
        var mockedData = CallHelpers.CreateAsyncUnaryCall(new PoliticalDomainOfInfluenceHierarchies()
        {
            PoliticalDomainOfInfluences = { acls },
        });

        return RunScoped<IServiceProvider>(async serviceProvider =>
        {
            serviceProvider.GetRequiredService<IPermissionService>().SetAbraxasAuthIfNotAuthenticated();
            var service = BuildAclImporterWithMockedGrpcClient(mockedData, serviceProvider);
            await service.ImportAcl();
        });
    }

    protected Task<TResult> RunScopedAuthenticated<TService, TResult>(Func<TService, Task<TResult>> action)
        where TService : notnull
    {
        return RunScoped<IServiceProvider, TResult>(async sp =>
        {
            sp.GetRequiredService<IPermissionService>().SetAbraxasAuthIfNotAuthenticated();
            return await action(sp.GetRequiredService<TService>());
        });
    }

    private IAccessControlListImportService BuildAclImporterWithMockedGrpcClient(
        AsyncUnaryCall<PoliticalDomainOfInfluenceHierarchies> mockedGrpcData,
        IServiceProvider serviceProvider)
    {
        var mockedGrpcClient = new Mock<AdminManagementService.AdminManagementServiceClient>();
        mockedGrpcClient
            .Setup(m => m.GetPoliticalDomainOfInfluenceHierarchyAsync(
                It.IsAny<GetPoliticalDomainOfInfluenceHierarchyRequest>(),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .Returns(mockedGrpcData);

        return new AccessControlListImportService(
            serviceProvider.GetRequiredService<Lib.Common.IClock>(),
            serviceProvider.GetRequiredService<IAccessControlListDoiRepository>(),
            serviceProvider.GetRequiredService<IImportStatisticRepository>(),
            mockedGrpcClient.Object,
            serviceProvider.GetRequiredService<ILogger<AccessControlListImportService>>(),
            serviceProvider.GetRequiredService<IPermissionService>(),
            serviceProvider.GetRequiredService<IDataContext>(),
            serviceProvider.GetRequiredService<ImportsConfig>());
    }
}

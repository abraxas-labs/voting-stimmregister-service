// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Voting.Lib.Iam.Testing.AuthenticationScheme;
using Voting.Lib.Testing;
using Voting.Stimmregister.Adapter.Data;
using Voting.Stimmregister.Domain.Authorization;
using Voting.Stimmregister.Test.Utils.MockData;

namespace Voting.Stimmregister.Test.Utils.Helpers;

public abstract class BaseRestTest<TFactory, TStartup> : RestAuthorizationBaseTest<TFactory, TStartup>
    where TFactory : BaseTestApplicationFactory<TStartup>
    where TStartup : class
{
    private readonly Lazy<HttpClient> _lazyUnauthorizedClient;
    private readonly Lazy<HttpClient> _lazyApiImporterClient;
    private readonly Lazy<HttpClient> _lazyManualImporterClient;
    private readonly Lazy<HttpClient> _lazyApiEVotingClient;
    private readonly Lazy<HttpClient> _lazySgManualExporterClient;
    private readonly Lazy<HttpClient> _lazySgApiExporterClient;
    private readonly Lazy<HttpClient> _lazySgManagerClient;
    private readonly Lazy<HttpClient> _lazySgReaderClient;
    private readonly Lazy<HttpClient> _lazyTgManagerClient;
    private readonly Lazy<HttpClient> _lazyTgReaderClient;

    protected BaseRestTest(TFactory factory)
        : base(factory)
    {
        _lazyUnauthorizedClient = new Lazy<HttpClient>(() =>
            CreateHttpClient(false));

        _lazyApiImporterClient = new Lazy<HttpClient>(() =>
            CreateHttpClient(true, tenant: VotingIamTenantIds.KTSG, userId: SecureConnectTestDefaults.MockedUserService.Loginid, roles: [Roles.ApiImporter]));

        _lazyManualImporterClient = new Lazy<HttpClient>(() =>
            CreateHttpClient(true, tenant: VotingIamTenantIds.KTSG, roles: [Roles.ManualImporter]));

        _lazyApiEVotingClient = new Lazy<HttpClient>(() =>
            CreateHttpClient(true, tenant: VotingIamTenantIds.KTSG, roles: [Roles.EVoting]));

        _lazySgManualExporterClient = new Lazy<HttpClient>(() =>
            CreateHttpClient(true, tenant: VotingIamTenantIds.KTSG, roles: [Roles.ManualExporter]));

        _lazySgApiExporterClient = new Lazy<HttpClient>(() =>
            CreateHttpClient(true, tenant: VotingIamTenantIds.KTSG, roles: [Roles.ApiExporter]));

        _lazySgManagerClient = new Lazy<HttpClient>(() =>
            CreateHttpClient(true, tenant: VotingIamTenantIds.KTSG, roles: [Roles.Reader, Roles.Manager]));

        _lazySgReaderClient = new Lazy<HttpClient>(() =>
            CreateHttpClient(true, tenant: VotingIamTenantIds.KTSG, roles: [Roles.Reader]));

        _lazyTgManagerClient = new Lazy<HttpClient>(() =>
            CreateHttpClient(true, tenant: VotingIamTenantIds.KTTG, roles: [Roles.Reader, Roles.Manager]));

        _lazyTgReaderClient = new Lazy<HttpClient>(() =>
            CreateHttpClient(true, tenant: VotingIamTenantIds.KTTG, roles: [Roles.Reader]));
    }

    protected HttpClient UnauthorizedClient => _lazyUnauthorizedClient.Value;

    protected HttpClient ApiImporterClient => _lazyApiImporterClient.Value;

    protected HttpClient ManualImporterClient => _lazyManualImporterClient.Value;

    protected HttpClient ApiEVotingClient => _lazyApiEVotingClient.Value;

    protected HttpClient SgManualExporterClient => _lazySgManualExporterClient.Value;

    protected HttpClient SgApiExporterClient => _lazySgApiExporterClient.Value;

    protected HttpClient SgManagerClient => _lazySgManagerClient.Value;

    protected HttpClient SgReaderClient => _lazySgReaderClient.Value;

    protected HttpClient TgManagerClient => _lazyTgManagerClient.Value;

    protected HttpClient TgReaderClient => _lazyTgReaderClient.Value;

    protected Task<TResult> RunOnDb<TResult>(Func<DataContext, Task<TResult>> action)
        => RunScoped(action);

    protected Task RunOnDb(Func<DataContext, Task> action)
        => RunScoped(action);
}

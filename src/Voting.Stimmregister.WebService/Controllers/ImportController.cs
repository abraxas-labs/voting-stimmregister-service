// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Voting.Lib.Iam.SecondFactor.Exceptions;
using Voting.Lib.Iam.Store;
using Voting.Lib.Rest.Files;
using Voting.Lib.Rest.Utils;
using Voting.Stimmregister.Abstractions.Core.Queues;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Domain.Authorization;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.WebService.Auth;
using Voting.Stimmregister.WebService.Models;

namespace Voting.Stimmregister.WebService.Controllers;

/// <summary>
/// Rest api controller for imports.
/// </summary>
[Route("v1/import")]
[ApiController]
[DisableFormValueModelBinding]
[RequestSizeLimit(BaseImportConfig.MaxFileSizeBytes)]
[RequestFormLimits(MultipartBodyLengthLimit = BaseImportConfig.MaxFileSizeBytes)]
public class ImportController : ControllerBase
{
    /// <summary>
    /// Header that carries the id of the verified second factor transaction
    /// for manual file imports.
    /// </summary>
    private const string SecondFactorTransactionIdHeader = "X-Second-Factor-Transaction-Id";

    private readonly MultipartRequestHelper _requestHelper;
    private readonly IPersonImportQueue _personImportQueue;
    private readonly IDomainOfInfluenceImportQueue _domainOfInfluenceImportQueue;
    private readonly IManualImportSecondFactorTransactionService _manualImportSecondFactorService;
    private readonly IAuth _auth;

    public ImportController(
        MultipartRequestHelper requestHelper,
        IPersonImportQueue personImportQueue,
        IDomainOfInfluenceImportQueue domainOfInfluenceImportQueue,
        IManualImportSecondFactorTransactionService manualImportSecondFactorService,
        IAuth auth)
    {
        _requestHelper = requestHelper;
        _personImportQueue = personImportQueue;
        _domainOfInfluenceImportQueue = domainOfInfluenceImportQueue;
        _manualImportSecondFactorService = manualImportSecondFactorService;
        _auth = auth;
    }

    /// <summary>
    /// Imports a Cobra (SG) person csv passed in the request body.
    /// </summary>
    /// <param name="secondFactorTransactionId">The second factor transaction ID for manual import, if applicable.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation resolving to the import result.</returns>
    [AuthorizeApiOrManualImporter]
    [HttpPost("cobra/persons")]
    [Consumes("multipart/form-data")]
    public Task<ImportRestApiResponse> CobraPersons([FromHeader(Name = SecondFactorTransactionIdHeader)] Guid? secondFactorTransactionId = null)
        => EnqueuePersonImport(ImportSourceSystem.Cobra, secondFactorTransactionId, "upload-cobra.csv");

    /// <summary>
    /// Imports a Cobra (TG) person csv passed in the request body.
    /// </summary>
    /// <param name="secondFactorTransactionId">The second factor transaction ID for manual import, if applicable.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation resolving to the import result.</returns>
    [AuthorizeApiOrManualImporter]
    [HttpPost("cobra-tg/persons")]
    [Consumes("multipart/form-data")]
    public Task<ImportRestApiResponse> CobraTgPersons([FromHeader(Name = SecondFactorTransactionIdHeader)] Guid? secondFactorTransactionId = null)
        => EnqueuePersonImport(ImportSourceSystem.CobraTg, secondFactorTransactionId, "upload-cobra_tg.csv");

    /// <summary>
    /// Imports a loganto domain of influence csv passed in the request body.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation resolving to the import result.</returns>
    [AuthorizeApiImporter]
    [HttpPost("loganto/doi")]
    [Consumes("multipart/form-data")]
    public async Task<ImportRestApiResponse> LogantoDoi()
    {
        var import = await _requestHelper.ReadFile(
            Request,
            f => _domainOfInfluenceImportQueue.Enqueue(ImportSourceSystem.Loganto, f.FileName ?? "upload-loganto-doi.csv", f.Content));

        return new ImportRestApiResponse(import.Id);
    }

    /// <summary>
    /// Imports a loganto person csv passed in the request body.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation resolving to the import result.</returns>
    [AuthorizeApiImporter]
    [HttpPost("loganto/persons")]
    [Consumes("multipart/form-data")]
    public async Task<ImportRestApiResponse> LogantoPersons()
    {
        var import = await _requestHelper.ReadFile(
            Request,
            f => _personImportQueue.Enqueue(ImportSourceSystem.Loganto, f.FileName ?? "upload-loganto-person.csv", f.Content));

        return new ImportRestApiResponse(import.Id);
    }

    /// <summary>
    /// Imports a dme person xml passed in the request body.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation resolving to the import result.</returns>
    [AuthorizeApiImporter]
    [HttpPost("innosolv/persons")]
    [Consumes("multipart/form-data")]
    public async Task<ImportRestApiResponse> InnosolvPersons()
    {
        var import = await _requestHelper.ReadFile(
            Request,
            f => _personImportQueue.Enqueue(ImportSourceSystem.Innosolv, f.FileName ?? "upload-dme-person.xml", f.Content));

        return new ImportRestApiResponse(import.Id);
    }

    private async Task<ImportRestApiResponse> EnqueuePersonImport(
        ImportSourceSystem sourceSystem,
        Guid? secondFactorTransactionId,
        string defaultFileName)
    {
        var requiresSecondFactor = !_auth.HasRole(Roles.ApiImporter);

        var import = await _requestHelper.ReadFile(
            Request,
            async f =>
            {
                if (requiresSecondFactor)
                {
                    var transactionId = secondFactorTransactionId ??
                                        throw new SecondFactorTransactionNotVerifiedException();
                    await _manualImportSecondFactorService.EnsureVerifiedAndUse(transactionId, HttpContext.RequestAborted);
                }

                return await _personImportQueue.Enqueue(sourceSystem, f.FileName ?? defaultFileName, f.Content);
            },
            [MediaTypeNames.Text.Csv]);

        return new ImportRestApiResponse(import.Id);
    }
}

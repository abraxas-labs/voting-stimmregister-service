// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Voting.Lib.Rest.Files;
using Voting.Lib.Rest.Utils;
using Voting.Stimmregister.Abstractions.Core.Queues;
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
    private readonly MultipartRequestHelper _requestHelper;
    private readonly IPersonImportQueue _personImportQueue;
    private readonly IDomainOfInfluenceImportQueue _domainOfInfluenceImportQueue;

    public ImportController(
        MultipartRequestHelper requestHelper,
        IPersonImportQueue personImportQueue,
        IDomainOfInfluenceImportQueue domainOfInfluenceImportQueue)
    {
        _requestHelper = requestHelper;
        _personImportQueue = personImportQueue;
        _domainOfInfluenceImportQueue = domainOfInfluenceImportQueue;
    }

    /// <summary>
    /// Imports a cobra person csv passed in the request body.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation resolving to the import result.</returns>
    [AuthorizeApiOrManualImporter]
    [HttpPost("cobra/persons")]
    public async Task<ImportRestApiResponse> CobraPersons()
    {
        var import = await _requestHelper.ReadFile(
            Request,
            f => _personImportQueue.Enqueue(ImportSourceSystem.Cobra, f.FileName ?? "upload-cobra.csv", f.Content));

        return new ImportRestApiResponse(import.Id);
    }

    /// <summary>
    /// Imports a loganto domain of influence csv passed in the request body.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation resolving to the import result.</returns>
    [AuthorizeApiImporter]
    [HttpPost("loganto/doi")]
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
    public async Task<ImportRestApiResponse> InnosolvPersons()
    {
        var import = await _requestHelper.ReadFile(
            Request,
            f => _personImportQueue.Enqueue(ImportSourceSystem.Innosolv, f.FileName ?? "upload-dme-person.xml", f.Content));

        return new ImportRestApiResponse(import.Id);
    }
}

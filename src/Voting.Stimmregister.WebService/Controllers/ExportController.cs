// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Voting.Lib.Iam.Exceptions;
using Voting.Lib.Rest.Files;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.WebService.Auth;
using Voting.Stimmregister.WebService.Exceptions;
using Voting.Stimmregister.WebService.Validators;

namespace Voting.Stimmregister.WebService.Controllers;

/// <summary>
/// Rest api controller for exports.
/// </summary>
[Route("v1/export")]
[ApiController]
public class ExportController : ControllerBase
{
    private readonly IExportCsvService _exportCsvService;
    private readonly IExportEchService _exportEchService;
    private readonly ILogger<ExportController> _logger;
    private readonly IPermissionService _permission;
    private readonly ExportRequestParameterValidator _exportRequestParameterValidator = new();

    public ExportController(
        IExportCsvService exportCsvService,
        IExportEchService exportEchService,
        ILogger<ExportController> logger,
        IPermissionService permission)
    {
        _exportCsvService = exportCsvService;
        _exportEchService = exportEchService;
        _logger = logger;
        _permission = permission;
    }

    /// <summary>
    /// Exports people data based on a filter.
    /// Exports can be done based on the following search criteria:
    ///    - FilterID
    ///    - FilterID and VersionID
    ///    - Filter criteria.
    /// </summary>
    /// <param name="searchParameters">The search parameters.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task wrapping the <see cref="FileResult"/>.</returns>
    [AuthorizeManualExporter]
    [HttpPost]
    [Produces("text/csv")]
    [Route("csv")]
    public async Task<FileResult> ExportCsv([FromBody] PersonSearchParametersExportModel searchParameters, CancellationToken ct)
    {
        var validationResult = _exportRequestParameterValidator.Validate(searchParameters);

        if (validationResult?.IsValid != true)
        {
            _logger.LogError("Input validation failed with errors: {ValidationErrors}", validationResult?.ToString());
            throw new InvalidExportPayloadException();
        }

        var file = searchParameters.VersionId.HasValue
            ? await _exportCsvService.ExportCsvByFilterVersion(searchParameters.VersionId.Value)
            : searchParameters.FilterId.HasValue
                ? await _exportCsvService.ExportCsvByFilter(searchParameters.FilterId.Value)
                : await _exportCsvService.ExportCsv(searchParameters.Criteria);

        return SingleFileResult.Create(file.MimeType, file.Filename, writer => file.Write(writer, ct));
    }

    /// <summary>
    /// Exports people data based on a filter.
    /// Exports can be done based on the following search criteria:
    ///    - FilterID
    ///    - FilterID and VersionID
    ///    - Filter criteria.
    /// </summary>
    /// <param name="searchParameters">The search parameters.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task wrapping the <see cref="FileResult"/>.</returns>
    [AuthorizeApiOrManualExporter]
    [HttpPost]
    [Produces("text/xml")]
    [Route("ech-0045")]
    public async Task<FileResult> ExportEch0045([FromBody] PersonSearchParametersExportModel searchParameters, CancellationToken ct)
    {
        var validationResult = _exportRequestParameterValidator.Validate(searchParameters);

        if (validationResult?.IsValid != true)
        {
            _logger.LogError("Input validation failed with errors: {ValidationErrors}", validationResult?.ToString());
            throw new InvalidExportPayloadException();
        }

        if (!_permission.IsManualExporter() && !searchParameters.VersionId.HasValue)
        {
            _logger.LogError("The role ApiExporter is only allowed for eCH-0045 exports of type filter versions.");
            throw new ForbiddenException();
        }

        var file = searchParameters.VersionId.HasValue
            ? await _exportEchService.ExportEch0045ByFilterVersion(searchParameters.VersionId.Value)
            : searchParameters.FilterId.HasValue
                ? await _exportEchService.ExportEch0045ByFilter(searchParameters.FilterId.Value)
                : await _exportEchService.ExportEch0045(searchParameters.Criteria);

        return SingleFileResult.Create(file.MimeType, file.Filename, writer => file.Write(writer, ct));
    }
}

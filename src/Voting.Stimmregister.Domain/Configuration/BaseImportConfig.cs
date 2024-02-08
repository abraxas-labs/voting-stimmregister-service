// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Configuration;

/// <summary>
/// Configuration model for import settings mapped from appsettings.
/// </summary>
public class BaseImportConfig
{
    /// <summary>
    /// The maximum import size.
    /// </summary>
    public const long MaxFileSizeBytes = 1024 * 1024 * 1024; // 1 GB

    /// <summary>
    /// Gets or sets the maximum allowed record validation errors when an import should be aborted.
    /// </summary>
    public int MaxRecordValidationErrorsThreshold { get; set; } = 10;

    /// <summary>
    /// Gets or sets the maximum string length validation for records.
    /// </summary>
    public int RecordValidationMaxStringLength { get; set; } = 250;

    /// <summary>
    /// Gets or sets the maximum int number validation for records.
    /// </summary>
    public int RecordValidationMaxIntNumber { get; set; } = 100000000;

    /// <summary>
    /// Gets or sets the maximum string length validation for entities.
    /// </summary>
    public int EntityValidationMaxStringLength { get; set; } = 150;

    /// <summary>
    /// Gets or sets the maximum int number validation for entities.
    /// </summary>
    public int EntityValidationMaxIntNumber { get; set; } = 100000000;
}

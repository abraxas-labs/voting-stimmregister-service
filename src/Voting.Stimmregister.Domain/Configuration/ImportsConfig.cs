// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Voting.Lib.Scheduler;
using Voting.Stimmregister.Domain.Enums;

namespace Voting.Stimmregister.Domain.Configuration;

/// <summary>
/// Configuration model for multiple imports settings mapped from appsettings.
/// </summary>
public class ImportsConfig
{
    private IReadOnlyDictionary<int, DateTime>? _municipalityIdBlacklistById;
    private IReadOnlyDictionary<int, AllowedPersonImportSourceSystemConfig>? _allowedPersonImportSourceSystemByMunicipalityId;

    /// <summary>
    /// Gets or sets the municipality blacklist.
    /// </summary>
    public List<MunicipalityBlacklistConfig> MunicipalityIdBlacklist { get; set; } = new();

    /// <summary>
    /// Gets the municipality id starting dates by their id.
    /// </summary>
    public IReadOnlyDictionary<int, DateTime> MunicipalityIdBlacklistById => _municipalityIdBlacklistById ??= MunicipalityIdBlacklist
        .GroupBy(x => x.MunicipalityId)
        .ToDictionary(x => x.Key, x => x.Min(y => y.StartingDate));

    /// <summary>
    /// Gets or sets the swiss foreign municipality id whitelist.
    /// </summary>
    public HashSet<string> SwissAbroadMunicipalityIdWhitelist { get; set; } = new();

    /// <summary>
    /// Gets or sets the domain of influence import config.
    /// </summary>
    public DomainOfInfluenceImportConfig DomainOfInfluence { get; set; } = new();

    /// <summary>
    /// Gets or sets the person import config.
    /// </summary>
    public PersonImportConfig Person { get; set; } = new();

    /// <summary>
    /// Gets or sets the VOTING Basis import config.
    /// </summary>
    public VotingBasisConfig VotingBasis { get; set; } = new();

    /// <summary>
    /// Gets or sets a directory path where import files are temporary stored while in the import queue.
    /// </summary>
    public string ImportFileQueueDirectory { get; set; } = Path.GetFullPath("import-queue-data");

    /// <summary>
    /// Gets or sets the import file csv delimiter to use for the csv configuration.
    /// </summary>
    public string ImportFileCsvDelimiter { get; set; } = ";";

    /// <summary>
    /// Gets or sets the default import file encoding to use as fallback if autodetection isn't possible.
    /// </summary>
    public string ImportFileDefaultEncodingName { get; set; } = "utf-8";

    /// <summary>
    /// Gets the default import file encoding to use as fallback if autodetection isn't possible.
    /// </summary>
    public Encoding ImportFileDefaultEncoding => Encoding.GetEncoding(ImportFileDefaultEncodingName);

    /// <summary>
    /// Gets or sets the job configuration on when to run the import job.
    /// </summary>
    public JobConfig Job { get; set; } = new() { RunOnStart = false, Interval = TimeSpan.FromMinutes(5) };

    /// <summary>
    /// Gets or sets a value indicating whether this process should run imports.
    /// </summary>
    public bool RunImports { get; set; }

    /// <summary>
    /// Gets or sets the name of this import worker.
    /// </summary>
    public string WorkerName { get; set; } = Environment.MachineName;

    /// <summary>
    /// Gets or sets the allowed person import source system.
    /// </summary>
    public List<AllowedPersonImportSourceSystemConfig> AllowedPersonImportSourceSystem { get; set; } = new();

    /// <summary>
    /// Gets the allowed person import source system configuration by municipality id.
    /// </summary>
    public IReadOnlyDictionary<int, AllowedPersonImportSourceSystemConfig> AllowedPersonImportSourceSystemByMunicipalityId => _allowedPersonImportSourceSystemByMunicipalityId ??= AllowedPersonImportSourceSystem
        .GroupBy(x => x.MunicipalityId)
        .ToDictionary(x => x.Key, x => x.Single());

    public BaseImportConfig GetImportConfig(ImportType type)
    {
        return type switch
        {
            ImportType.Person => Person,
            ImportType.DomainOfInfluence => DomainOfInfluence,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "No import config registered for " + type),
        };
    }
}

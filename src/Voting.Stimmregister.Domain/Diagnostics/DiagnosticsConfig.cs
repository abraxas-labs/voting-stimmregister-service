// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Prometheus;

namespace Voting.Stimmregister.Domain.Diagnostics;

/// <summary>
/// A static Diagnostic class holding prometheus metrics instances and provides methods to update them accordingly.
/// </summary>
public static class DiagnosticsConfig
{
    private const string NotAvailable = "n/a";

    private static readonly Histogram _personSearchDuration = Metrics
        .CreateHistogram(
            "voting_stimmregister_person_search_duration_seconds",
            "Histogram of person search query execution duration.",
            new HistogramConfiguration
            {
                LabelNames = new[] { "results_range", "query" },
            });

    private static readonly Gauge _importJobsInQueue = Metrics
        .CreateGauge(
        "voting_stimmregister_import_jobs_queued",
        "Number of import jobs waiting for processing in the queue.");

    private static readonly Gauge _importDatasetsMutatedByBfs = Metrics
        .CreateGauge(
        "voting_stimmregister_import_datasets_mutated_by_bfs",
        "Number of mutated datasets within latest import by bfs.",
        labelNames: new[] { "import_type", "bfs" });

    private static readonly Counter _importJobsProcessed = Metrics
        .CreateCounter(
        "voting_stimmregister_import_jobs_processed",
        "Count of succeeded import jobs.",
        labelNames: new[] { "import_type", "import_status" });

    private static readonly Gauge _importLatestTimestampByBfs = Metrics
        .CreateGauge(
            "voting_stimmregister_import_timestamp_by_bfs",
            "Timestamp of the latest import by bfs.",
            labelNames: new[] { "import_type", "bfs", "municipalityName" });

    /// <summary>
    /// Initializes the diagnostic instances.
    /// </summary>
    public static void Initialize()
    {
        _importJobsInQueue.Set(0);
    }

    /// <summary>
    /// Registers a metrics entry within the person search histogram.
    /// </summary>
    /// <param name="resultsCount">The search results count.</param>
    /// <param name="filtersCount">The filters count.</param>
    /// <param name="elapsed">The time elapsed.</param>
    public static void RegisterPersonSearchQueryingTime(int resultsCount, int filtersCount, TimeSpan elapsed)
    {
        var rangeLabel = resultsCount switch
        {
            0 => "0",
            <= 100 => "100",
            <= 1000 => "1'000",
            <= 10_000 => "10'000",
            <= 100_000 => "100'000",
            <= 1_000_000 => "1'000'000",
            _ => "10'000'000",
        };

        _personSearchDuration.Labels(rangeLabel, filtersCount.ToString()).Observe(elapsed.TotalSeconds);
    }

    /// <summary>
    /// Increase the <see cref="_importJobsInQueue"/> gauge.
    /// </summary>
    public static void IncreaseQueuedJobs()
    {
        _importJobsInQueue.Inc();
    }

    /// <summary>
    /// Increase the <see cref="_importJobsInQueue"/> gauge.
    /// </summary>
    public static void DecreaseQueuedJobs()
    {
        _importJobsInQueue.Dec();
    }

    public static void SetImportDatasetsMutated(string importType, int? bfs, int mutatedDatasetsCount)
    {
        var bfsLabel = GetBfsLabel(bfs);
        _importDatasetsMutatedByBfs.WithLabels(importType, bfsLabel).Set(mutatedDatasetsCount);
    }

    public static void IncreaseProcessedImportJobs(string importType, string importStatus)
    {
        _importJobsProcessed.WithLabels(importType, importStatus).Inc();
    }

    public static void SetImportLatestTimestamp(string importType, int? bfs, string? municipalityName, DateTime? date)
    {
        date ??= DateTime.MinValue;
        municipalityName ??= NotAvailable;

        var bfsLabel = GetBfsLabel(bfs);
        var timestamp = new DateTimeOffset(date.Value).ToUnixTimeSeconds();

        _importLatestTimestampByBfs.WithLabels(importType, bfsLabel, municipalityName).Set(timestamp);
    }

    private static string GetBfsLabel(int? bfs)
    {
        return bfs == null ? NotAvailable : bfs.Value.ToString();
    }
}

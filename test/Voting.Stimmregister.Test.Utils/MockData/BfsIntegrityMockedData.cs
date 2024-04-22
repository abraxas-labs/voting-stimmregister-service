// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Voting.Lib.Testing.Mocks;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Domain.Cryptography;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Test.Utils.MockData;

/// <summary>
/// Mocked data for <see cref="BfsIntegrityEntity"/>.
/// If data is adjusted the signature need to be updated <seealso cref="ApplySignatures"/>.
/// </summary>
public static class BfsIntegrityMockedData
{
    private static readonly IReadOnlyDictionary<Guid, string> _signatures = new Dictionary<Guid, string>
    {
        [Guid.Parse("28da12c1-6e19-4e94-9f68-751014ffbae1")] = "AdHW+k3CyyOTELoY9iuQcW8c6QQtoAr6euk30C5IcF6/wGZCpnNdCuQW2OYy6E1GNRik3zsYmSx0Xtwx4KyHqwFPAGWKCwySr2YU2bUL9KexuAOHlVFn4UV1pEA6AujZuVal6E6fAAfwzBVcSpUwlt2cYz6uDDFd3Ml6tDDQVExxgVvU",
        [Guid.Parse("f04d2243-d71a-4dea-931d-871e4f6fa4eb")] = "AFhGYOEvNFb8tGGs3DWycsiz0lUuWBTRH30xxYLqopM9v/+zF/Er1FeGvDYF9iiTUn0lam3QMSwYihiVFT7rcHmfABE37aBzbZSraLCZSmkQfcXJntZr/8GopOnXkdvie9/Siy4oweTxfbBtl3XPvcnPSnvgXWHy4WOi3/Ts4+YgerlT",
    };

    public static BfsIntegrityEntity Person_3203_UpToDate
        => new()
        {
            Id = Guid.Parse("28da12c1-6e19-4e94-9f68-751014ffbae1"),
            Bfs = "3203",
            ImportType = ImportType.Person,
            LastUpdated = MockedClock.UtcNowDate,
            AuditInfo = MockedAuditInfo.Get(),
        };

    public static BfsIntegrityEntity Person_9170_OutOfDate
        => new()
        {
            Id = Guid.Parse("f04d2243-d71a-4dea-931d-871e4f6fa4eb"),
            Bfs = "9170",
            ImportType = ImportType.Person,
            LastUpdated = MockedClock.UtcNowDate.AddDays(-5),
            AuditInfo = MockedAuditInfo.Get(-5),
        };

    public static IEnumerable<BfsIntegrityEntity> All
    {
        get
        {
            yield return Person_3203_UpToDate;
            yield return Person_9170_OutOfDate;
        }
    }

    /// <summary>
    /// Seeds mock data defined in this task.
    /// </summary>
    /// <param name="runScoped">The run scoped action.</param>
    /// <returns>A <see cref="Task"/> from the run scoped action where data is seeded async.</returns>
    public static Task Seed(Func<Func<IServiceProvider, Task>, Task> runScoped)
    {
        return runScoped(async sp =>
        {
            var db = sp.GetRequiredService<IDataContext>();
            var all = All.ToList();
            await ApplySignatures(sp, all);

            db.BfsIntegrities.AddRange(all);
            await db.SaveChangesAsync();
        });
    }

    private static async Task ApplySignatures(IServiceProvider sp, IReadOnlyCollection<BfsIntegrityEntity> all)
    {
        // apply stored signatures
        foreach (var integrity in all)
        {
            if (!_signatures.TryGetValue(integrity.Id, out var signatureB64))
            {
                continue;
            }

            integrity.Signature = Convert.FromBase64String(signatureB64);
            integrity.SignatureVersion = 1;
            integrity.SignatureKeyId = "VOSR_ECDSA_PUBLIC_KEY_PRE";
        }

        var db = sp.GetRequiredService<IDataContext>();
        var signer = sp.GetRequiredService<ICreateSignatureService>();
        var signatureVerifier = sp.GetRequiredService<IVerifySigningService>();

        var toSign = all.Where(x => x.Signature.Length == 0).ToList();
        var bfs = all.Select(x => x.Bfs).ToHashSet();
        var persons = await db.Persons
            .IgnoreQueryFilters()
            .Where(x => x.IsLatest && !x.IsDeleted && bfs.Contains(x.MunicipalityId.ToString()))
            .Include(x => x.PersonDois.OrderBy(y => y.Id))
            .ToListAsync();
        var personsByBfs = persons
            .GroupBy(x => x.MunicipalityId)
            .ToDictionary(x => x.Key.ToString(), x => x.OrderBy(y => y.Id).ToList());

        foreach (var integrity in toSign)
        {
            signer.SignIntegrity(integrity, personsByBfs.GetValueOrDefault(integrity.Bfs) ?? new List<PersonEntity>());
        }

        // all integrity entities should have a valid signature
        // otherwise the mock data is not valid!
        // If a bfs integrity entity or dependent data is added/modified,
        // delete its signature from the mock data,
        // add a breakpoint here,
        // copy the exception dictionary initializer text
        // and update the _signatures dictionary.
        foreach (var integrity in all)
        {
            signatureVerifier.EnsureBfsIntegritySignatureValid(integrity, personsByBfs.GetValueOrDefault(integrity.Bfs) ?? new List<PersonEntity>());
        }

        if (toSign.Count > 0)
        {
            var missingSignatures = string.Join(Environment.NewLine, toSign.Select(x => $"[Guid.Parse(\"{x.Id}\")] = \"{Convert.ToBase64String(x.Signature)}\","));
            throw new InvalidOperationException("Not all entities have a signature, add these missing signatures: " + Environment.NewLine + missingSignatures);
        }
    }
}

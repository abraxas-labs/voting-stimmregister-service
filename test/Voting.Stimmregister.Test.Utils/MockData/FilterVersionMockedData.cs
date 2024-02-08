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
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Test.Utils.MockData;

/// <summary>
/// ImportAcl mock data seeder. Use this class to add some static seeding data.
/// </summary>
public static class FilterVersionMockedData
{
    private static readonly IReadOnlyDictionary<Guid, string> _signatures = new Dictionary<Guid, string>
    {
        [Guid.Parse("2ddf8718-be9e-4730-a3ad-81253a16f5f7")] = "ARk6miSYfdJGRPBIEXw3W53x9FXLowpYQQQt+F65/batiSr/M7q+LtyK9F68V12EfbbIY8selZBgn39JahPTdzuKAVUPrhCIEbdkYpbWIWRAwtlFHS3/eyhtuKhs7GKVBIIV/enlRSkKGIiLoBOt+H4LxVN2ybzbD/3uoENBxzfAdVTQ",
        [Guid.Parse("3333d8dd-057c-4449-9630-12dacc12ba05")] = "AAqKjJEvv0Q6CO3UiyZDDK7DYXkAVsrrV3ETCCaznhRqTCHsGb8j4oTnk/CagvTN0eudCUs+XXoIJnP9Spk54E7MAVSfEle63MeuyJYn+3fh1DQPFbZtAj5GZwmGDDFX7GYMylwjOcWZWGpdfSBbIWZflqEq7dttvyjBKNygRa48bOrE",
        [Guid.Parse("5ee3d8dd-057c-4449-9630-7cdacc12ba04")] = "AVGW1KfMgCPO11FkwVO5TsDfXJUiWXdRM63dn4URYB/Jbd+j8DbzmlV7t2kze/0ELmtGM1aQifEToGtPXvtW1YXkAVytOKN9gOO5fB1aKdqQ2Ss4vKAB+0LnTBVp/XxsZHBkOnQp+4YSJNUABsn2KFabNGyBqoH1LeXF/ek9G64U8Hxx",
        [Guid.Parse("82a3d8dd-057c-4449-9630-12dacc12ba05")] = "APxBPwHQxz75lPwdN1gi0zlITcO7h3Bymm+UnwAUOFvYk0813nXxej7ej8x55K8GdNbbcpKmMKWoU9DzQsbD+NjGARnLSoEex8wCZLlZDbMjkA++Dp/nvlNsKIU+ZY+g7FDG/QIqZJtCc6DldHgoISr9LewSOf6NHJmZZX5AZanu5TFC",
    };

    public static int MunicipalityId => FilterMockedData.MunicipalityId;

    public static int MunicipalityIdOther => FilterMockedData.MunicipalityIdOther;

    public static FilterVersionEntity SomeFilterVersion_MunicipalityId
        => new()
        {
            Id = Guid.Parse("2DDF8718-BE9E-4730-A3AD-81253A16F5F7"),
            AuditInfo = MockedAuditInfo.Get(-5),
            Name = "My Filter Version",
            FilterId = FilterMockedData.SomeFilter_MunicipalityId.Id,
            Deadline = MockedClock.GetDate().Date,
            Count = 10,
            CountOfInvalidPersons = 1,
        };

    public static FilterVersionEntity SomeFilterVersion_MunicipalityIdOther
        => new()
        {
            Id = Guid.Parse("5EE3D8DD-057C-4449-9630-7CDACC12BA04"),
            AuditInfo = MockedAuditInfo.Get(-4),
            Name = "Other Filter Version",
            FilterId = FilterMockedData.SomeFilter_MunicipalityIdOther.Id,
            Deadline = MockedClock.GetDate().Date,
            Count = 20,
        };

    public static FilterVersionEntity SomeFilterVersion_MunicipalityIdOther2
        => new()
        {
            Id = Guid.Parse("3333D8DD-057C-4449-9630-12DACC12BA05"),
            AuditInfo = MockedAuditInfo.Get(-2),
            Name = "Other Filter Version 2",
            FilterId = FilterMockedData.SomeFilter_MunicipalityIdOther2.Id,
            Deadline = MockedClock.GetDate().Date,
            Count = 30,
        };

    public static FilterVersionEntity SomeFilterVersion_WithFilterVersionPersons
        => new()
        {
            Id = Guid.Parse("82A3D8DD-057C-4449-9630-12DACC12BA05"),
            AuditInfo = MockedAuditInfo.Get(-1),
            Name = "Filter Version With Filter Version Persons",
            FilterId = FilterMockedData.SomeFilter_MunicipalityIdOther2.Id,
            Deadline = MockedClock.GetDate().Date,
            Count = 40,
        };

    public static IEnumerable<FilterVersionEntity> All
    {
        get
        {
            yield return SomeFilterVersion_MunicipalityId;
            yield return SomeFilterVersion_MunicipalityIdOther;
            yield return SomeFilterVersion_MunicipalityIdOther2;
            yield return SomeFilterVersion_WithFilterVersionPersons;
        }
    }

    /// <summary>
    /// Seeds mock data defined in this task.
    /// </summary>
    /// <param name="runScoped">The run scoped action.</param>
    /// <param name="seedWithFilterMockedData">seed process should include seeding of filters.</param>
    /// <returns>A <see cref="Task"/> from the run scoped action where data is seeded async.</returns>
    public static async Task Seed(Func<Func<IServiceProvider, Task>, Task> runScoped, bool seedWithFilterMockedData = true)
    {
        await runScoped(async sp =>
        {
            if (seedWithFilterMockedData)
            {
                await FilterMockedData.Seed(runScoped);
            }

            var db = sp.GetRequiredService<IDataContext>();
            var all = All.ToList();
            ApplyStoredSignatures(all);
            db.FilterVersions.AddRange(all);
            await db.SaveChangesAsync();
        });

        await FilterVersionPersonMockedData.Seed(runScoped);
        await runScoped(EnsureValidSignaturesAndApplyMissing);
    }

    private static void ApplyStoredSignatures(IEnumerable<FilterVersionEntity> all)
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
    }

    private static async Task EnsureValidSignaturesAndApplyMissing(IServiceProvider sp)
    {
        var db = sp.GetRequiredService<IDataContext>();
        var signer = sp.GetRequiredService<ICreateSignatureService>();
        var signatureVerifier = sp.GetRequiredService<IVerifySigningService>();

        var all = await db.FilterVersions.IgnoreQueryFilters().ToListAsync();
        var toSign = all.Where(x => x.Signature.Length == 0).ToList();
        var persons = await db.FilterVersionPersons
            .IgnoreQueryFilters()
            .Include(x => x.Person!)
            .ThenInclude(x => x.PersonDois)
            .OrderBy(p => p.Person!.MunicipalityId)
            .ThenBy(p => p.Person!.Id)
            .ToListAsync();

        var personsByFilterId = persons
            .GroupBy(x => x.FilterVersionId)
            .ToDictionary(x => x.Key, x => x.Select(y => y.Person!).ToList());

        foreach (var filterVersion in toSign)
        {
            signer.SignFilterVersion(filterVersion, personsByFilterId.GetValueOrDefault(filterVersion.Id) ?? new List<PersonEntity>());
        }

        // all integrity entities should have a valid signature
        // otherwise the mock data is not valid!
        // If a bfs integrity entity or dependent data is added/modified,
        // delete its signature from the mock data,
        // add a breakpoint here,
        // copy the exception dictionary initializer text
        // and update the _signatures dictionary.
        foreach (var filterVersion in all)
        {
            signatureVerifier.EnsureFilterVersionSignatureValid(filterVersion, personsByFilterId.GetValueOrDefault(filterVersion.Id) ?? new List<PersonEntity>());
        }

        if (toSign.Count > 0)
        {
            var missingSignatures = string.Join(Environment.NewLine, toSign.Select(x => $"[Guid.Parse(\"{x.Id}\")] = \"{Convert.ToBase64String(x.Signature)}\","));
            throw new InvalidOperationException("Not all entities have a signature, add these missing signatures: " + Environment.NewLine + missingSignatures);
        }
    }
}

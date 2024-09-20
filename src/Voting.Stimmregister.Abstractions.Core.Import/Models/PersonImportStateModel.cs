// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Core.Import.Models;

public class PersonImportStateModel : ImportStateModel<PersonEntity>
{
    /// <summary>
    /// Gets or sets a set of enabled eVoting vn.
    /// </summary>
    public HashSet<long> EVotingEnabledVns { get; set; } = new();

    /// <summary>
    /// Gets or sets a list of person DOI entities for a given BFS number from the
    /// access control list. Represents all associated parent nodes of type
    /// CH, CT, BZ and MU from the ACL hierarchy. Primary used for DOI enrichment.
    /// </summary>
    public IReadOnlySet<PersonDoiEntity> PersonDoisFromAclByBfs { get; set; } = new HashSet<PersonDoiEntity>();

    /// <summary>
    /// Gets or sets a dictionary of existing unprocessed person entities by person id.
    /// </summary>
    private Dictionary<Guid, PersonEntity> ExistingPersonsById { get; set; } = new();

    /// <summary>
    /// Gets or sets a dictionary of existing unprocessed person entities by ahv number.
    /// </summary>
    private Dictionary<long, Guid> PersonIdsByAhvNumber { get; set; } = new();

    /// <summary>
    /// Gets or sets a dictionary of existing unprocessed person entities by source system id.
    /// </summary>
    private Dictionary<string, Guid> PersonIdsBySourceSystemId { get; set; } = new();

    /// <summary>
    /// Gets or sets a dictionary containing the existing domain of influence entities by DomainOfInfluenceId.
    /// </summary>
    private IReadOnlyDictionary<int, DomainOfInfluenceEntity> ExistingDoisByDoiId { get; set; } = new Dictionary<int, DomainOfInfluenceEntity>();

    private IMapper? Mapper { get; set; }

    /// <summary>
    /// Initializes the existing person dictionaries.
    /// </summary>
    /// <param name="existingPersonsById">A dictionary containing the existing person entities by id.</param>
    /// <param name="existingDoisByDoiId">A dictionary containing the existing domain of influence entities by DomainOfInfluenceId.</param>
    /// <param name="mapper">Person mapper.</param>
    public void InitializeExistingPersons(
        Dictionary<Guid, PersonEntity> existingPersonsById,
        Dictionary<int, DomainOfInfluenceEntity> existingDoisByDoiId,
        IMapper mapper)
    {
        ExistingPersonsById = existingPersonsById;

        PersonIdsByAhvNumber = existingPersonsById.Values
            .Where(p => p.Vn != null)
            .ToDictionary(p => p.Vn!.Value, p => p.Id);

        PersonIdsBySourceSystemId = existingPersonsById.Values
            .Where(p => p.SourceSystemId != null)
            .GroupBy(p => p.SourceSystemId)
            .Select(g => g.Count() > 1 ? g.FirstOrDefault(p => !p.IsDeleted) ?? g.First() : g.First())
            .ToDictionary(p => p.SourceSystemId!, p => p.Id);

        ExistingDoisByDoiId = existingDoisByDoiId;

        Mapper = mapper;
    }

    /// <summary>
    /// Finds an existing person according to the following criterias:
    /// <list type="number">
    ///     <item>AHV number if available</item>
    ///     <item>AHV number if available</item>
    /// </list>
    /// </summary>
    /// <param name="ahvNumber">The ahv number to lookup.</param>
    /// <param name="sourceSystemId">The source system id to lookup.</param>
    /// <returns>A <see cref="PersonEntity"/> if already existing, or null if not.</returns>
    public PersonEntity? FindExistingPerson(long? ahvNumber, string? sourceSystemId)
    {
        return FindPersonByAhvNumber(ahvNumber)
            ?? FindPersonBySourceSystemId(sourceSystemId);
    }

    /// <summary>
    /// Finds a domain of influence by the passed id number.
    /// </summary>
    /// <param name="domainOfInfluenceId">The domain of influence id number.</param>
    /// <returns>A <see cref="DomainOfInfluenceEntity"/> if existing, or null if not.</returns>
    public DomainOfInfluenceEntity? FindDomainOfInfluence(int? domainOfInfluenceId)
    {
        if (domainOfInfluenceId == null || !ExistingDoisByDoiId.TryGetValue(domainOfInfluenceId.Value, out var domainOfInfluence))
        {
            return null;
        }

        return domainOfInfluence;
    }

    /// <summary>
    /// Updates a person by adding the new updated verison to the new persons list,
    /// and updating the old version by adding it to the updated list.
    /// </summary>
    /// <param name="updatedEntity">The new updated person entity to create.</param>
    /// <param name="existingPerson">The old person entity to be replaced.</param>
    public override void Update(PersonEntity updatedEntity, PersonEntity existingPerson)
    {
        updatedEntity.RegisterId = existingPerson.RegisterId;
        updatedEntity.VersionCount = existingPerson.VersionCount + 1;
        updatedEntity.IsLatest = true;
        existingPerson.IsLatest = false;

        EntitiesToUpdate.Add(existingPerson);
        EntitiesToCreate.Add(updatedEntity);

        if (existingPerson.IsDeleted)
        {
            CreateCount++;
        }
        else if (updatedEntity.IsDeleted)
        {
            DeleteCount++;
        }
        else
        {
            UpdateCount++;
        }
    }

    /// <summary>
    /// Soft deletes persons, who are not part of the import records anymore.
    /// </summary>
    /// <param name="deletedDate">The deleted date time.</param>
    public void SoftDeleteUnprocessed(DateTime deletedDate)
    {
        if (Mapper == null)
        {
            throw new InvalidOperationException("Person mapper not initialized");
        }

        var toDelete = ExistingPersonsById
            .Values
            .Where(p => !p.IsDeleted);

        foreach (var person in toDelete)
        {
            // For all navigation properties of the person entity, the id must be ignored in the mapping profile
            // to prevent original foreign-key constraints from being manipulated and thus, to preserve data integrity.
            var newPerson = Mapper.Map<PersonEntity>(person);
            newPerson.DeletedDate = deletedDate;
            newPerson.ImportStatisticId = ImportStatisticId;
            Update(newPerson, person);
        }
    }

    /// <summary>
    /// Removes the person from the import state index dictionaries.
    /// </summary>
    /// <param name="entity">The person to remove.</param>
    public override void SetProcessed(PersonEntity entity)
    {
        ExistingPersonsById.Remove(entity.Id);

        if (entity.Vn != null)
        {
            PersonIdsByAhvNumber.Remove(entity.Vn.Value);
        }

        if (entity.SourceSystemId != null)
        {
            PersonIdsBySourceSystemId.Remove(entity.SourceSystemId);
        }
    }

    /// <summary>
    /// Finds a person by ahv number within the existing persons dictionary.
    /// </summary>
    /// <param name="ahvNumber">The ahv number to lookup.</param>
    /// <returns>A <see cref="PersonEntity"/> if existing, otherwise null.</returns>
    private PersonEntity? FindPersonByAhvNumber(long? ahvNumber)
    {
        if (ahvNumber == null)
        {
            return null;
        }

        if (PersonIdsByAhvNumber.TryGetValue(ahvNumber.Value, out var id) &&
            ExistingPersonsById.TryGetValue(id, out var person))
        {
            return person;
        }

        return null;
    }

    /// <summary>
    /// Finds a person by source system id within the existing persons dictionary.
    /// </summary>
    /// <param name="sourceSystemId">The source system id to lookup.</param>
    /// <returns>A <see cref="PersonEntity"/> if existing, otherwise null.</returns>
    private PersonEntity? FindPersonBySourceSystemId(string? sourceSystemId)
    {
        if (string.IsNullOrEmpty(sourceSystemId))
        {
            return null;
        }

        if (PersonIdsBySourceSystemId.TryGetValue(sourceSystemId, out var id) &&
            ExistingPersonsById.TryGetValue(id, out var person))
        {
            return person;
        }

        return null;
    }
}

# ✨ Changelog (`v2.6.8`)

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Version Info

```text
This version -------- v2.6.8
Previous version ---- v2.6.7
Initial version ----- v1.121.5
Total commits ------- 3
```

## [v2.6.8] - 2024-06-03

### 🔄 Changed

- update link to code of conduct

## [v2.6.7] - 2024-04-16

### 🔄 Changed

- change ech-0045 address mapping fallback logic

## [v2.6.6] - 2024-04-15

### 🔄 Changed

- assert duplicated persons with same ahvn13 in person import

## [v2.6.5] - 2024-04-15

### 🔄 Changed

- update ech-0045 mapping and alliance name for loganto subsystem.

## [v2.6.4] - 2024-04-08

### 🔄 Changed

- import doi address for "no correspondence" persons

## [v2.6.3] - 2024-04-03

### 🔄 Changed

- add whitespace validation for person entity

## [v2.6.2] - 2024-04-02

### 🔄 Changed

- prevent mapping of invalid circles to a person

## [v2.6.1] - 2024-03-28

### 🔄 Changed

- enable e-voter flag in ech-0045 export for Swiss abroad
- update test data to add support for Swiss abroad e-voters

## [v2.6.0] - 2024-03-28

### 🔄 Changed

- don't write bfs statistics during cobra import

## [v2.5.2] - 2024-03-28

### 🔄 Changed

- extend entity mapper to apply lifecycle data

## [v2.5.1] - 2024-03-28

### 🔄 Changed

- update district category mapping in innosolv importer.

## [v2.5.0] - 2024-03-28

### 🔄 Changed

- refactor import

## [v2.4.0] - 2024-03-26

### 🆕 Added

- evoting registration dashboard

## [v2.3.0] - 2024-03-19

### 🆕 Added

- add support for foreign delivery addresses

## [v2.2.8] - 2024-03-19

### 🔄 Changed

- allow to skip forwarding of e-voter flag by municipality id configuration

### 🔄 Changed

- extend filter input validation

### 🔄 Changed

- add relegion for swiss person type in ech-0045 export

### ❌ Removed

- remove deprecated person signature

### 🔄 Changed

- extend person identification criteria in e-voting registration

### :lock: Security

- dependency and runtime patch policy
- use latest dotnet runtime v8.0.3

### 🔄 Changed

- re-introduce 'Ki' doi type for acl layer.

### 🔄 Changed

- remove unused doi type 'Ki'

### 🆕 Added

- check for dublicated SourceSystemId in the importfile for all import types.
- unique constraint for SourceSystemId, SourceSystemName and VersionCount.

### 🆕 Added

- API Authorization Integration Tests

### 🔄 Changed

- remove expression based implementation for calculated filters

### 🆕 Added

- Update voting library to register dll import resolver for pkcs11 driver

BREAKING CHANGE: Updated service to .NET 8 LTS.

### :arrows_counterclockwise: Changed

- update to dotnet 8

### :lock: Security

- apply patch policy

### 🆕 Added

- database query monitoring

### 🆕 Added

- add filter tenant name

### 🔄 Changed

- add missing district mappings in innosolv import.

### 🔄 Changed

- update entity validation for doi type 'OG' to max. length 50 according to eCH-0011 v8.1

### 🔄 Changed

- update license copyright

### 🔄 Changed

- Random Webservice with Mockdatagenerator at loganto and cobra

### ❌ Removed

- remove deprecated canton address attribute

### :x: Removed

- remove deprecated house number suplement from kewr adapter.

### 🔄 Changed

- csv doi and person import errors

### 🆕 Added

- add eCH from voting lib

### 🔄 Changed

- adjust log level for abraxas authentication values

### :arrows_counterclockwise: Changed

- remove required statements in model builder, update parameter constraints.

### :arrows_counterclockwise: Changed

- use separate port for metrics endpoint provisioning

### 🔄 Changed

- extended anonymization tool for test data

### :arrows_counterclockwise: Changed

- static code analysis refinment

### 🆕 Added

- add tests for all filter criteria

### :arrows_counterclockwise: Changed

- update solution and projects to align with hexagonal architecture requirements
- update architecture tests (ArchUnit) to align with solution design refactoring.

### :arrows_counterclockwise: Changed

- exclude ACL children in query to prevent conflicts in entity framework changetracker when updating entities that may have 0..\* children set in the navigation property.

### :lock: Security

- decouple port for metrics endpoint from the default application port mapping

### :new: Added

- apply service account scopes client credential flow

### 🔄 Changed

- import file tests

### 🆕 Added

- add cobra mock generator

### :arrows_counterclockwise: Changed

- exclude tools from bbt deployment

### :arrows_counterclockwise: Changed

- map innosolv contact address from residence if no contact address is available

### 🆕 Added

- add allowed person import source system config

### 🔄 Changed

- stream persons during rename filter version

### 🔄 Changed

- change evoting person nationality to country name short

### 🔄 Changed

- ignore acl for evoting person search

### :arrows_counterclockwise: Changed

- extend continuous deployment pipeline with BBT and UAT jobs.
- updated ci-cd templates to v1.8.8

### 🔄 Changed

- extend filter version signature and verify before renaming

### ❌ Removed

- remove enum check constraints

### 🆕 Added

- add setting to disabled loganto and kewr for eVoting

### 🔄 Changed

- replace filter reference id with enum

### 🔄 Changed

- deleting a person creates a new version of the person

### 🆕 Added

- add residence permit dates to innosolv mock generator

### 🔄 Changed

- is voting allowed only for valid move in arrival dates

### ❌ Removed

- remove age from person model

### 🆕 Added

- add contact address post office box number

### 🔄 Changed

- mock data generator target file for swiss abroads

### 🆕 Added

- compress queued files with gzip due to huge size of xml files

### 🆕 Added

- add innosolv file verify command

### 🆕 Added

- innosolv mock data generator

### 🔄 Changed

- pre-assign person-doi ids to calculate correct signature on first import

### 🆕 Added

- add innosolv support

### 🔄 Changed

- verify csv export signature of persons without a filter version

### 🔄 Changed

- verify ech export signature of persons without a filter version

### 🔄 Changed

- stream csv exports
- verify signature if exporting a filter version as csv

### 🔄 Changed

- Verify filter version signature in eCH exports

### 🔄 Changed

- update ci-cd templates to v1.8.1
- use postgres host for db resolution since issue <https://gitlab.com/gitlab-org/gitlab-runner/issues/2229> is fixed

### ❌ Removed

- remove deprecated job extensions

### 🔄 Changed

- updated ci-cd template version

### ❌ Removed

- removed vulnerable library `System.Linq.Dynamic.Core`

### 🔒 Security

- activate strict policy for dependency check

### 🔄 Changed

- set started date same as created date for acl import
- set modified date when updated for acl import
- set service user name as default when no user scope is available

### 🔄 Changed

- extend 'nicht zustellen' rule n°3 with additional municipality exception behavior.

### 🔄 Changed

- extend e-voting exception messages with additional subsystem details.

### 🔄 Changed

- Update lib dependency to deterministic version

### 🔄 Changed

- exclude deleted persons from bfs integrity

### 🔄 Changed

- secure code review security refinments

### 🔄 Changed

- adjust cryptographic naming conventions

### 🔄 Changed

- stream persons when creating filter versions

### 🔄 Changed

- map cobra house number addition to entities dwelling number
- validate dwelling number to max length of 10 according to eCH
- validate house number to max length of 12 according to eCH
- reduce lenght of contact address zip code for foreign codes to 15 according to eCH

### 🆕 Added

- extend import statistics with AES cipher metadata crypto parameters.

### 🔄 Changed

- adjust naming for security operations (classes, methods, etc.).
- make AES cipher metadata independent from the cryptographic implementation itself.

### 🔒 Security

- store AES and MAC attributes as separate cryptographic parameters on database.
- generate cryptographic attributes IV, AES key and MAC key using save random number generator.
- the MAC key is now encrypted by the HSM with AES before being saved on the database.

### 🔄 Changed

- Set SendVotingCardsToDomainOfInfluenceReturnAddress flag according to updated specification

### 🆕 Added

- check dateofbirth DateOnly.MinValue for voting rights

### 🔄 Changed

- changed RulesForDates (RuleForDateOfBirthType)

### ❌ Removed

- remove contact address house number addition attribute and filter

### 🔒 Security

- signature breaking change

### 🔄 Changed

- add fallback strategy for eCh-0045 address assignment

### 🔄 Changed

- map residency valid from/to only for foreigners

### 🔄 Changed

- map alliance name and call name in ech export

### 🔄 Changed

- evaluate actual number of registered e-voters

### 🔄 Changed

- compare entities during import instead of comparing record against entity
- remove person reactivate logic
- if a person was soft deleted but reoccurs in the import, create a new revision instead of reactivating all old revisions
- store dois on person during import

### 🔄 Changed

- abort request when ech file streaming fails to notify the client/browser of the failure

### 🔄 Changed

- add voting place to ech exports

### 🔄 Changed

- add missing school circle filter references to calculated fields evaluation

### 🔄 Changed

- update library with AES-GCM hotfix

### 🔒 Security

- protect against signature payload attribute shift vulnerability

### 🔄 Changed

- update eCH-0045 mapping rules and entity input validators.

### 🔒 Security

- Secure code review refinments

also clean up age related code

also clean up the loganto and kewr services

### 🆕 Added

- add filter metadata endpoint

### 🔄 Changed

- count invalid persons

### 🔄 Changed

- apply fallback mapping for language of correspondence in eCH-0045 export

### 🔄 Changed

- remove voting right criteria for canton

### 🔄 Changed

- use cached canton enabled evoters lookup instead of municipality bfs

### 🔄 Changed

- changed ProtoVersion
- changed integration test

### 🔒 Security

- separate authentication and access control provisioning.

### 🆕 Added

- npgsql command timeout setting

### 🆕 Added

- configure bbt pipeline and quality gates

### 🔄 Changed

- person should only be hashed in bfs integrity

### 🔄 Changed

- disable person signature validation on filter creation

### 🔄 Changed

- server timing

### 🔄 Changed

- eVoting filter and sync

### 🆕 Added

- verify signatures when creating filter versions

### 🔄 Changed

- Output detailed error messages (processing errors) only for the ImportObserver role

### 🆕 Added

- last used parameters api

### 🔄 Changed

- TransactionUtil(deprecated) replaced with DbContext.Database.BeginTransactionAsync

### 🔄 Changed

- PersonLogantoMappingProfile
- PersonCobraMappingProfile
- CobraUtil
- LogantoUtil

### 🔄 Changed

- map contact address lines for swiss abroad according to the eai tool

### 🔄 Changed

- ensure that residence permits with length lower than 2 could be mapped

### ❌ Removed

- RuleForSexType
- RuleForReligionType
- RestrictedVotingAndElectionRightFederation

### 🆕 Added

- source system name added to PersonEntity

### 🔄 Changed

- Cobra Person import, Loganto Person and DOI import with source system name extended

### 🔄 Changed

- return id when creating new filter version id

### 🔄 Changed

- improve filter version creation ressource consumption by loading only required data once

### 🔄 Changed

- additionally validate municipality id against whitelist config in case of swiss foreign source system
- add gauge to monitor mutuaded datasets count

### 🆕 Added

- add get single filter version api endpoint

### 🔄 Changed

- speed up import person query by disabling acl check and adding an index

### 🔄 Changed

- use database transaction instead of ambient transaction to fix weird timing bug where transaction was not yet fully committed after complete call
- don't override import statistics municipality id with null on import failure

### 🔄 Changed

- ensure metrics are initialized at startup to better distinguish between not initialized and no data

### 🔄 Changed

- update last updated on integrity update

### 🔄 Changed

- read person actuality from bfs integrities

### 🔄 Changed

- update SendVotingCardsToDomainOfInfluenceReturnAddress correctly

### 🔄 Changed

- add SendVotingCardsToDomainOfInfluenceReturnAddress field on person

### 🔄 Changed

- fixes a bug when creating and processing imports which occasionally crashes the service due to the transaction not fully completed when fetching and processing the import job

### 🔄 Changed

- optimize import statistics queries

### 🔄 Changed

Fixed four major bugs

- Refactor the containing loop to do more than one iteration. (src/Voting.Stimmregister.Adapter.Loganto/Services/BaseDomainOfInfluenceImportService.cs)
- Refactor the containing loop to do more than one iteration. (src/Voting.Stimmregister.Adapter.Loganto/Services/BasePersonImportService.cs)
- Remove the 'using' statement; it will cause automatic disposal of 'aes'. (src/Voting.Stimmregister.Core/Services/Supporting/Signing/AesStreamEncryption.cs)

### 🆕 Added

- store tenant id on filter entity

### 🔄 Changed

- only store first two chars of the residence permit

### 🆕 Added

- new entity validation for mandatory bfs number in acl import

### 🔄 Changed

- assign bfs identification number from person doi for eCH-0045 export

### ❌ Removed

- remove religion from eCH-0056 export

### 🆕 Added

- cobra import extended with nationality and municipality name

### 🔄 Changed

- extended existing unit tests

### 🔒 Security

- avoid raw records in log when bad data detected

### 🔄 Changed

- include swiss abroad person property in grpc api response

### 🆕 Added

- extend eCH-0045 export with additional authorization rules

### 🔄 Changed

- extended existing integration tests

### ❌ Removed

- cleanup unused snapshots

### 🔄 Changed

- allow search with formatted social security number

### 🔄 Changed

- Various changes due to SonarQube code smell
- Sufix "Enum" removed from all enums

### 🔄 Changed

- fix(VOTING-3088): support formatted social security number filters

### 🔄 Changed

- nationality filter

### 🔄 Changed

- correct age filter operations on date of birth

### 🔄 Changed

- map domain of influences in eCH-0045 export
- change residence permission comparison so that only first two digits are checked
- only include persons from the last revision and not deleted for the eCH-0045 export

### 🔄 Changed

- fix voting right evaluation for filter versions

### 🆕 Added

- add domain of influence entity validation for circle names to report invalid circle references

### 🔄 Changed

- enrich person with additional domain of influences

### 🔄 Changed

- return id when saving a filter

### 🔄 Changed

- include whole import data set in integrity checksum
- iclude import source and type into integrity entity to distinguish persons and dois
- rename integrity table to BfsIntegrities

### 🔄 Changed

- include filter references in query only if reqired for further processing
- ensure transaction safety
- configured detete behavior according to data model definition
- add unique constraint for filter criteria reference id
- implement crud for filter criteria
- ensure filter version can only be saved if user has access to passed filter

### 🔄 Changed

- sort filter versions by deadline descending

### 🔄 Changed

- adjust duplicated filter name

### 🔄 Changed

- replace person streaming methods to use PersonEntity instead of PersonEntityModel

### ❌ Removed

- remove unused model mapping statements

### 🔄 Changed

- OrignName(1-7), OriginCanton(1-7) and all *CircleId und*CircleName fields from PersonDoi

### ❌ Removed

- Remove "Alter"/"Age" from CSV export (decision by customer) (VOTING-2990)

### 🔄 Changed

- map audit info from job to integrity audit info

### ❌ Removed

- remove audit info from doi and acl table since info is available on connected import statistic

### 🔄 Changed

- apply null substitution for nullable audit info attributes

### 🆕 Added

- restrict access to statistic data by access control list and new import observer role

### 🆕 Added

- add signature for domain of influences

### 🔄 Changed

- ensure already soft deleted persons are not deleted again
- set delete count to the grouped register id and not to every person version
- add reactivate count to datasets created count since this would reflect the number of delivered datasets compared to the history

### 🔄 Changed

- use transaction scope to bundle db updates and ensure rollback

### 🔄 Changed

- peek bfs from created file since resetting mulipart request stream position is not possible

### 🔄 Changed

- update voting right evaluation based on nationality

### 🔒 Security

- update api authorization configuration

### 🔄 Changed

- read bfs from import file before queuing
- reject import when bfs not in acl for user if not service user

### 🔄 Changed

- modified audit info migration to cover null value constraints

### 🆕 Added

- store municipality name on the import stats

### 🆕 Added

- add manual importer role for cobra import endpoint

### ❌ Removed

- remove unused role check functions

### 🔒 Security

- Added authentication verification for file encryption

### 🔄 Changed

- add audit info

### 🔄 Changed

- load latest filter version for filters

### 🔄 Changed

- cleanup filter grpc and service implementation

### ❌ Removed

- Unused userId field in FilterEntity

### ❌ Removed

- remove deprecated import configs

### 🔄 Changed

- correct saving of filter criterias

### 🔄 Changed

- fixed ignored filter version and filter criteria mappings

### 🔄 Changed

- compare against mapped domain of influence circle id if doi reference is available

### 🆕 Added

- added unit tests for voting right use cases

### 🔄 Changed

- update evaluation for canton voting right

### 🆕 Added

- extend metrics
- use static diagnostics class for metrics registrations

### 🔄 Changed

- get canton by bfs number filter by doi type bfs.

### 🔄 Changed

- import statistics list only return latest for each municipality id

### 🔄 Changed

- refine and fix open todo's

### 🔄 Changed

- use utf8 as default encoding since import files switched from latin1 to utf8

### ❌ Removed

- remove old import file model with static encoding default set since it was only used within tests

### 🔄 Changed

- Copyright 2022 to 2023

### 🔄 Changed

- Moved Entities from Abstractions.Adapter.Data to Domain

### ❌ Removed

- remove import service since it has been replaced by rest endpoints

### 🔄 Changed

- set abx auth in importers if no authentication is present

### 🔄 Changed

- sort person domain of influences

🆕 Added

- SonarQube rule for exceptions added

❌ Removed

- Removed the Serializable attribute and the
  constructor (SerializationInfo, StreamingContext) for all exceptions.

### 🔄 Changed

- set createdDate/createdBy for integrity entities

### 🆕 Added

- Return domain of influences when querying latest single person

### 🆕 Added

- check municipality id against acl on import

### 🔄 Changed

- fix import statistics table on data view

### 🔄 Changed

- change enum column to be stored as a string
- regenerate InitTables migration
- create InitConstraints migration and add existing and new constraints (i.e. CantonEnum, SexTypesEnum, ...)

### 🔄 Changed

- regenerate all db migrations

### 🆕 Added

- canton validation logic

### 🆕 Added

- set manual upload flag

### 🆕 Added

- integration test for deleting filter version with version persons

### 🔄 Changed

- fix municipality id validation in Proto and set municipality id on failed imports

### 🔄 Changed

- rm HasProcessingErrors from ImportStatistics

### 🔄 Changed

- fix Cobra import IsUpdateRequired method for origin information
- problem was when there was no person DOI information in file and DB a new person version was created

### 🔄 Changed

- Use paging from voting.lib

### 🆕 Added

- Person Entity with field countrynameshort extended

### 🔄 Changed

- ech replace residence entry date with move in arrival date

### 🔄 Changed

- split filter save version to create / rename version

### 🔄 Changed

- remove signature of PersonDoiEntity and calculate PersonDoi fields to PersonEntity signature

### 🔄 Changed

- technical debt: strict seperation between id and register id for person

### 🔄 Changed

- add rest endpoint for doi import
- refactor doi import

### 🆕 Added

- add "language of correspondence" to Loganto import

### 🔒 Security

- Added file encryption when saving temp file for import

### 🔄 Changed

- input validation on "origin" and on "on canton"

### 🔄 Changed

- fix concurrency issues in domain of influence to person mapping
- grab domain of influences once and use a dictionary for faster lookup for mapping
- fix ResidencePermitValidTill date comparison to use last day of month
- fix for mapping and comparison of circle information for person doi

### 🔄 Changed

- include filter criterias for filter version to display on the filter version overview

### 🔄 Changed

- version of VotingRegisterProtoVersion adjusted to 1.26.4

### ❌ Removed

- remove unused religions

### 🔄 Changed

- ensure test result ordering

### 🆕 Added

- integration tests for the changes

### 🔄 Changed

- normalize DOI information (circle id's & circle names & origin & oncanton)
- reimplement filters, mappings for origin, oncanton, circle ids and circle names
- adapt validations, mappings, import, export, change detection

### 🆕 Added

- setup hsm configuration and drivers

### 🔄 Changed

- change image runtime from alpine to bullseye slim

### 🔄 Changed

- fix findings out of business rule and database review
- adjust `PostOfficeBox` naming convention according to eCH-0010 naming convention
- update cobra mapping for `PostOfficeBox`

### 🆕 Added

- country-iso-helper-service implemention by loganto import
- loganto unittest (PersonImportServiceUnitTest; "WhenUserWithCountryImported_ShouldCreateNewVersion")

### 🔄 Changed

- PersonEntity with the field country short name extended

### 🔒 Security

- apply acl post-processing impersonation during person and doi import

### 🔄 Changed

- check for control characters in record validation for loganto for one specific field which has no validation in source system

### 🔄 Changed

- filter operator and types as enums

### 🔄 Changed

- Enum columns

### 🔄 Changed

- Minor changes in voting rules

### 🆕 Added

- Add Rules for voting right

### 🆕 Added

- new rules for Swisscitizenship filter, test ResidencePermitValid date range when SwissCitizenship = No
- integration tests for Filter rules (<https://confluence.abraxas-tools.ch/confluence/display/SREC/Filter+Rules+-+SSR>)

### 🔄 Changed

- snapshots

### 🆕 Added

- add filter save version integration tests

### 🔄 Changed

- only save name, modified date, modified by for update

### 🔄 Changed

- delete filter version persons on delete filter version

### 🆕 Added

- add cobra person import
- add cobra import tests
- set swiss abroad flag statically for imports from cobra

### 🔄 Changed

- refactor person import common parts

### 🔄 Changed

- replace singleton MemoryCacheEntryOptions with config object

### 🆕 Added

- Flag for canton is valid for voting rights

### 🔄 Changed

- Calculate age by reference key date

### 🆕 Added

- add Bfs country iso helper service (BFSCountryList.xml)
  - The BFS number of the country name is used to search for the ISO2 code, and returned.
- add Loganto country iso helper service (LogantoCountryList.xml)
  - With the Loganto country code the ISO2 code is searched and returned with the short name.

### 🆕 Added

- add filter count to cache

### ❌ Removed

- remove filter count from database

### 🔄 Changed

- REST API to upload loganto person imports
- File System queue of loganto person imports

### 🔄 Changed

- change fluent validations for better validation messages
- adapt tests

### 🆕 Added

- Add new mapping in EK import for canton abbreviation

### 🆕 Added

- add indicator of whether the date of birth is valid for the right to vote

### 🔒 Security

- Added signing of entities Person, Integrity, FilterVersions

### 🆕 Added

- unit tests for PersonEntityValidator

### 🆕 Added

- person import ech validation

### 🆕 Added

- Return person validation errors

### 🆕 Added

- add record identifier to RecordValidationErrorModel

### 🆕 Added

- DB index and constraint added on (RegisterId, IsLatest) combination on table Persons

### 🆕 Added

- add filter reference id for "HasValidationErrors" mapped to !IsValid

### 🆕 Added

- unittest for person search (GetAll)
- integrationstest for person search

### 🔄 Changed

- rm unused classes
- use ef fluent api configurations

### 🔄 Changed

- set import user context on import statistics instead of static source system identifier

### ❌ Removed

- remove unused field from import statistics table
- remove created, modified, deleted by fields from person entity since the reference to the latest importstatistic is sufficient

### 🔄 Changed

- set filter grpc service roles and adapt Integration tests

### 🔄 Changed

- read csv records async instead of reading them all into a list first
- fix typo in method signature
- move state modification operations to state model
- add dicionaries for faster existing person lookups
- expose state modification methods for privatly accesable state lists on the state class
- add transaction for db modifications
- only load latest versions for processing and soft delete persons versions at the end

### 🔄 Changed

- fix FK problem when deleting filter version
- fix query filter for filter criteria

### 🔄 Changed

- eCH contact address town as residenc address town fallback

### 🔄 Changed

- eCH should support foreigners

### 🔄 Changed

- add HasUtcConversion in FilterVersionBuilder and fix snapshots

### 🆕 Added

- ech 0045 export

### 🔄 Changed

- Get always the latest people by person repository

### 🔄 Changed

- aligned import type id to proto exact enum
- update proto including circle id type changes from int to string
- disable test parallelization for tests writing into database

### 🆕 Added

- Integration tests for filter grpc service

### 🔄 Changed

- fix unittests

### 🔄 Changed

- Result model for person by ID. Gets only the latest person data

### 🆕 Added

- add person import historization

### 🔄 Changed

- modified Person Entity model according to new requirements
- updated Snapper and Newtonsoft to get DateOnly support

### 🔄 Changed

- Fixed SonarQube issues in Core, Adapter and Tests

### 🆕 Added

- add municipality id to filter save and duplicate

### 🔄 Changed

- add pre calculated count field for filter version and populate it on save

### 🔄 Changed

- combined filter service / filter repository / person service / person repository for a working version

### 🆕 Added

- add filter operator seed data

### 🔄 Changed

- fixed unit tests for FilterProfile and PersonProfile

### 🔄 Changed

- Calculate person search age always with day date

### 🆕 Added

- Workaround logic for case insensitive person search string comparison

### 🆕 Added

- added count to Filter table / entity / model

### 🆕 Added

- add icu libs to runtime image to get culture support

### 🔄 Changed

- use valid culture modifier

### ❌ Removed

- removed dependencies to Voting.Lib.Rest and VotingExports from domain layer

### 🔄 Changed

- fix error that filter version were not saved and loaded properly

### 🆕 Added

- person search pagination validation

### 🆕 Added

- add csv export for person filter versions
- add new exporter role

### 🔄 Changed

- update csv helper

### 🆕 Added

- CRUD for filter version
- CRUD for filter criteria
- CRUD for filter

### 🆕 Added

- added additional filter for people search

### 🔒 Security

- Added query filters

### 🆕 Added

Add latest version id to PersonModel for filter features.

### 🔄 Changed

- Clean SonarQube issues

### 🆕 Added

Add GetByFilterId to IPersonService interface

### 🆕 Added

- person search filter for age
- person search GetByFilterId

### 🔄 Changed

- some generic expression logic

### 🔄 Changed

- make acl synchronization configurable for grpc web or grpc

### 🆕 Added

- handle further data types and operators for person search

### 🆕 Added

- add optional evoter limitation on municipality level

### 🆕 Added

- add single person search service with prepared version support

### 🆕 Added

- add minimal person search logic

### 🔄 Changed

- resolve issue in e-voting unregistraiton model assignment.

### 🆕 Added

- integrate eService into stimmregister service

### 🔄 Changed

- ignore path handling

### 🔄 Changed

- fix default cron-tab schedule syntax.

### 🔒 Security

- add domain of influence based access control list importer.
- add access control list DOI synchronization with VOTING Basis.
- add access control list DOI security layer for domain of influence and person data sets.

### 🆕 Added

- add request logging

### 🔄 Changed

- refactor import

### 🔒 Security

- update nuget packages

### 🆕 Added

- add person service including mock data
- introduce auto mapper
- fixed authorization
- fixed cert pinning

### 🆕 Added

- person grpc service
- person service draft
- person grpc service test

### 🆕 Added

- added database entities for person import

### ❌ Removed

- removed coding examples

### 🆕 Added

- architecture test project
- google protobuf package ref

### ❌ Removed

- architecture tests

### ❌ Removed

- remove unused packages

### 🔄 Changed

- try build twice

### 🔄 Changed

- update test package references

### 🔄 Changed

- update Google.Protobuf version to 3.19.1

### 🆕 Added

- assembly Google.Protobuf explicitly

### 🆕 Added

- additional swagger gen settings

### 🆕 Added

- Integration test setup

### 🆕 Added

- Unit tests for core module
- Renamed integration tests project

### 🆕 Added

- Solution architecture tests

### 🔄 Changed

- Resolved SonarQube code smells

### 🔒 Security

- use connection string builder for dummy connection string
- reviewed hard-coded credential which is okay here since it is only a dummy one used at design time

### 🆕 Added

- CORS configuration support

### 🆕 Added

- Loganto Adapter Init

## [v2.2.7] - 2024-03-15

### 🔄 Changed

- extend filter input validation

## [v2.2.6] - 2024-03-14

### 🔄 Changed

- add relegion for swiss person type in ech-0045 export

## [v2.2.5] - 2024-03-14

### ❌ Removed

- remove deprecated person signature

## [v2.2.4] - 2024-03-14

### 🔄 Changed

- extend person identification criteria in e-voting registration

## [v2.2.3] - 2024-03-13

### :lock: Security

- dependency and runtime patch policy
- use latest dotnet runtime v8.0.3

## [v2.2.2] - 2024-03-01

### 🔄 Changed

- re-introduce 'Ki' doi type for acl layer.

## [v2.2.1] - 2024-03-01

### 🔄 Changed

- remove unused doi type 'Ki'

## [v2.2.0] - 2024-02-28

### 🆕 Added

- check for dublicated SourceSystemId in the importfile for all import types.
- unique constraint for SourceSystemId, SourceSystemName and VersionCount.

## [v2.1.0] - 2024-02-20

### 🆕 Added

- API Authorization Integration Tests

### 🔄 Changed

- remove expression based implementation for calculated filters

## [v2.0.1] - 2024-02-14

### 🆕 Added

- Update voting library to register dll import resolver for pkcs11 driver

## [v2.0.0] - 2024-02-12

BREAKING CHANGE: Updated service to .NET 8 LTS.

### :arrows_counterclockwise: Changed

- update to dotnet 8

### :lock: Security

- apply patch policy

## [v1.123.0] - 2024-02-06

### 🆕 Added

- database query monitoring

## [v1.122.0] - 2024-02-06

### 🆕 Added

- add filter tenant name

## [v1.121.7] - 2024-02-05

### 🔄 Changed

- add missing district mappings in innosolv import.

## [v1.121.6] - 2024-02-01

### 🔄 Changed

- update entity validation for doi type 'OG' to max. length 50 according to eCH-0011 v8.1

## [v1.121.5] - 2024-01-31

### 🎉 Initial release for Bug Bounty

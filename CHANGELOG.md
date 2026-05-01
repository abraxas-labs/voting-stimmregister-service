# ✨ Changelog (`v2.31.0`)

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Version Info

```text
This version -------- v2.31.0
Previous version ---- v2.28.2
Initial version ----- v1.121.5
Total commits ------- 10
```

## [v2.31.0] - 2026-04-28

### 🆕 Added

- add elementary and upper school circle filter

## [v2.30.4] - 2026-04-28

### 🔄 Changed

- extend persons sort order

## [v2.30.3] - 2026-04-28

### 🆕 Added

- add origins to person

## [v2.30.2] - 2026-02-18

### 🔄 Changed

- reorder metrics middleware calls in Startup configuration to catch final response status.

## [v2.30.1] - 2026-02-16

### :new: Added

- Test for foreigner without country

### :arrows_counterclockwise: Changed

- PersonVoterMapping in Ech adapter to handle foreigner without country

## [v2.30.0] - 2026-02-13

### 🔄 Changed

- feat(VOTING-6891): anonymizer improvements

## [v2.29.1] - 2026-02-12

### 🔄 Changed

- fix: use latest payload builder versions in mocked data

## [v2.29.0] - 2026-02-06

### 🆕 Added

- add eCH-0045 v6 export

## [v2.28.2] - 2026-02-06

### 🔄 Changed

- extend CD pipeline with enhanced bug bounty publication workflow

## [v2.28.1] - 2026-02-03

### :arrows_counterclockwise: Changed

- update input validation length from `100` to `500` characters for official name filter value.

## [v2.28.0] - 2026-01-26

### 🆕 Added

- feat(VOTING-5827): innosolv anonymizer

## [v2.27.0] - 2026-01-19

### 🆕 Added

- add support for canton AR during ACL import

## [v2.26.0] - 2026-01-16

### 🆕 Added

- feat(VOTING-6761): add registerid to csv

## [v2.25.2] - 2025-12-21

### ❌ Removed

- remove deprecated kewr and loganto adapter

## [v2.25.1] - 2025-11-25

### 🔄 Changed

- return locally stored email for e-voting information

## [v2.25.0] - 2025-11-25

### 🆕 Added

- add email to e-voting information response

## [v2.24.3] - 2025-11-06

### 🔄 Changed

- update the list of countries and territories to the latest BFS publication from `15.05.2025`.
- unified and normalized country helper interfaces and models for BFS and Loganto-specific country mappings.

### ❌ Removed

- removed deprecated models that have been replaced by the official eCH-0072 standard.

## [v2.24.2] - 2025-10-29

### 🔄 Changed

- update innosolv mapping for houshold assignment for undefined state

## [v2.24.1] - 2025-10-23

### 🆕 Added

- extend anonymizer tool to support cobra-tg input format

## [v2.24.0] - 2025-10-21

### 🆕 Added

- add new import source system for cobra-tg.
- add new setting to configure supported import source systems per canton.

### 🔄 Changed

- updated import statistics service to only deliver supported import source systems based on the request context.
- updated import service only accept supported import source systems based on the request context.

## [v2.23.0] - 2025-10-14

### 🆕 Added

- add e-voting email

## [v2.22.2] - 2025-10-06

### 🔄 Changed

- ensure consistent results for GetPersonIdByAhvn13

## [v2.22.1] - 2025-10-06

### 🔄 Changed

- exclude origin name from filters which allow multiple values

## [v2.22.0] - 2025-09-08

### 🆕 Added

- feat(VOTING-6287): add ecollecting byname paging and voting right info

## [v2.21.0] - 2025-08-15

### 🔄 Changed

- filter originName and originCanton combined if both are specified

## [v2.20.1] - 2025-08-13

### 🔒 Security

- fix(VOTING-6158): use new voting lib version and allow ob tokens

## [v2.20.0] - 2025-08-12

### 🆕 Added

- add innosolv Abraxas 1.5 eCH version

## [v2.19.1] - 2025-08-06

### 🔄 Changed

- fix(VOTING-5938): upgrade lib

## [v2.19.0] - 2025-07-15

### 🆕 Added

- add on behalf token handling

## [v2.18.1] - 2025-07-01

### 🔄 Changed

- bump pkcs11 driver from 4.45 to 4.51.0.1

## [v2.18.0] - 2025-06-19

### 🔄 Changed

- feat(VOTING-5920): lax ecollecting search

## [v2.17.2] - 2025-06-05

### 🆕 Added

- add religion data to foreigners in eCH export

## [v2.17.1] - 2025-05-28

### 🔄 Changed

- fix(VOTING-5688): fix ecollecting endpoints by supporting mutliple islatest persons and either canton or municipality bfs

## [v2.17.0] - 2025-05-22

### 🆕 Added

- E-Collecting endpoints

## [v2.15.0] - 2025-04-02

### 🆕 Added

- add option to add multiple values in a string filter

## [v2.14.1] - 2025-03-11

### 🔄 Changed

- avoid index out of range exception in server timing middleware

## [v2.14.0] - 2025-01-29

### 🆕 Added

- added STISTAT e-voting export

## [v2.13.0] - 2025-01-29

### 🆕 Added

- add householder fields

### 🔄 Changed

- default integrity signature has been changed to v2

## [v2.12.3] - 2025-01-24

### ❌ Removed

- removed obsolete fields: RegisteredEVotersInCanton, RegisteredEVotersInMunicipality

## [v2.12.2] - 2025-01-20

### 🔄 Changed

- write metrics only for latest imports

## [v2.12.1] - 2025-01-16

### 🔄 Changed

- check for multiple acl entries with the same bfs during import
- match only acl entries of type "MU" during import

## [v2.12.0] - 2025-01-13

### 🔄 Changed

- consider live e-voting limits

## [v2.11.1] - 2025-01-10

### 🔄 Changed

- update voting library from 12.20.0 to 12.22.3

### 🔒 Security

- use updated Pkcs11Interop library version 5.2.0

## [v2.11.0] - 2025-01-10

### 🆕 Added

- clean up filter versions and person versions

## [v2.10.0] - 2024-12-19

### 🆕 Added

- add sorting to import statistics

## [v2.9.1] - 2024-12-18

### 🔄 Changed

- restrict import statistics metrics to chosen source systems

## [v2.9.0] - 2024-12-16

### 🆕 Added

- include user id in log output

## [v2.8.4] - 2024-12-05

### 🔄 Changed

- exclude disabled import statistics in read operations

## [v2.8.3] - 2024-11-25

### 🔄 Changed

- optimize SourceLink integration and use new ci/cd versioning capabilities
- prevent duplicated commit ids in product version, only use SourceLink plugin.
- extend .dockerignore file with additional exclusions

## [v2.8.2] - 2024-11-22

### 🔄 Changed

- skip writing metrics for imports from a disabled source system

## [v2.8.1] - 2024-11-22

### 🔄 Changed

- consider only writing metrics for valid imports

## [v2.8.0] - 2024-11-20

### 🆕 Added

- Add new metric: timestamp of the latest import per bfs

## [v2.7.9] - 2024-10-28

### 🔄 Changed

- Enhance address component validation by adding checks for address line 1 and address line 2.

## [v2.7.8] - 2024-10-11

### 🔄 Changed

- enrich service model with PostOfficeBoxText

## [v2.7.7] - 2024-10-10

### 🔄 Changed

- enrich e-voting person address with post office box text.

## [v2.7.6] - 2024-09-22

### ❌ Removed

- remove e-voter registration count columns

## [v2.7.5] - 2024-09-05

### 🔄 Changed

- update municipality exception list for sending voting cards with away addresses

## [v2.7.4] - 2024-09-03

### 🔄 Changed

- migrate from gcr to harbor

## [v2.7.3] - 2024-08-27

### 🔄 Changed

- update bug bounty template reference
- patch ci-cd template version, align with new defaults

## [v2.7.2] - 2024-08-22

### 🔄 Changed

- move environment specific app settings out of default file

## [v2.7.1] - 2024-08-21

### 🔄 Changed

- ensure swagger generator can be disabled completely

## [v2.7.0] - 2024-08-19

### 🔄 Changed

- apply CORS allowed origin least privilege

## [v2.6.19] - 2024-08-14

### 🔄 Changed

- update cobra import to retain duplicate values in address line extension.

## [v2.6.18] - 2024-08-14

### 🔄 Changed

- fix voter total count in bfs statistics

## [v2.6.17] - 2024-08-08

### 🔄 Changed

- Updated the VotingLibVersion property in the Common.props file from 12.10.4 to 12.10.5. This update includes improvements for the proto string validation for better error reporting.

## [v2.6.16] - 2024-08-07

### 🔒 Security

- add content-type restriction for multipart import APIs and form data content.

## [v2.6.15] - 2024-07-15

### 🔒 Security

- upgrade npgsql to fix vulnerability CVE-2024-0057

## [v2.6.14] - 2024-07-11

### 🔄 Changed

- fix voter total count in bfs statistics

## [v2.6.13] - 2024-07-11

### 🆕 Added

- extend filter metadata with person actuality indicators

## [v2.6.12] - 2024-07-04

### 🔄 Changed

- update voting library to implement case-insensitivity for headers as per RFC-2616

## [v2.6.11] - 2024-07-02

### 🔄 Changed

- Method SoftDeleteUnprocessed in PersonImportStateModel set ImportStatisticId for deleted person record

## [v2.6.10] - 2024-06-26

### 🔄 Changed

- ignore navigation property ids when cloning a person entity

## [v2.6.9] - 2024-06-21

### 🆕 Added

- Tests for special cases with SourceSystemId and Vn.

### 🔄 Changed

- Initialization of dictionary PersonIdsBySourceSystemId in PersonImportStateModel. Choose latest with deletet flag fals if dublicated SourceSystemId's exist.

### ❌ Removed

- Unique constraint for sourceSystemId, sourceSystemName, municipalityId, versionCount.

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

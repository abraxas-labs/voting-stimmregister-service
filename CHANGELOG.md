# ✨ Changelog (`v2.25.1`)

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Version Info

```text
This version -------- v2.25.1
Previous version ---- v2.14.0
Initial version ----- v1.121.5
Total commits ------- 466
```

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

### 🔄 Changed

- return locally stored email for e-voting information

### 🆕 Added

- add email to e-voting information response

### 🔄 Changed

- update the list of countries and territories to the latest BFS publication from `15.05.2025`.
- unified and normalized country helper interfaces and models for BFS and Loganto-specific country mappings.

### ❌ Removed

- removed deprecated models that have been replaced by the official eCH-0072 standard.

### 🔄 Changed

- update innosolv mapping for houshold assignment for undefined state

### 🆕 Added

- extend anonymizer tool to support cobra-tg input format

### 🆕 Added

- add new import source system for cobra-tg.
- add new setting to configure supported import source systems per canton.

### 🔄 Changed

- updated import statistics service to only deliver supported import source systems based on the request context.
- updated import service only accept supported import source systems based on the request context.

### 🆕 Added

- add e-voting email

### 🔄 Changed

- ensure consistent results for GetPersonIdByAhvn13

### 🔄 Changed

- exclude origin name from filters which allow multiple values

### 🆕 Added

- feat(VOTING-6287): add ecollecting byname paging and voting right info

### 🔄 Changed

- filter originName and originCanton combined if both are specified

### 🔒 Security

- fix(VOTING-6158): use new voting lib version and allow ob tokens

### 🆕 Added

- add innosolv Abraxas 1.5 eCH version

### 🔄 Changed

- fix(VOTING-5938): upgrade lib

### 🆕 Added

- add on behalf token handling

### 🔄 Changed

- bump pkcs11 driver from 4.45 to 4.51.0.1

### 🔄 Changed

- feat(VOTING-5920): lax ecollecting search

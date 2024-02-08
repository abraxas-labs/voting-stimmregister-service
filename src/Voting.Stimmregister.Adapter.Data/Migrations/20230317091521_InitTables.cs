// (c) Copyright 2023 by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    public partial class InitTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EVoterAudits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BfsCanton = table.Column<short>(type: "smallint", nullable: true),
                    BfsMunicipality = table.Column<short>(type: "smallint", nullable: true),
                    EVoterFlag = table.Column<bool>(type: "boolean", nullable: true),
                    ContextId = table.Column<string>(type: "text", nullable: true),
                    StatusMessage = table.Column<string>(type: "text", nullable: true),
                    StatusCode = table.Column<short>(type: "smallint", nullable: true),
                    EVoterId = table.Column<Guid>(type: "uuid", nullable: false),
                    EVoterCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EVoterCreatedBy = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    EVoterModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EVoterModifiedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EVoterAudits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EVoters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ahvn13 = table.Column<string>(type: "text", nullable: false),
                    BfsCanton = table.Column<short>(type: "smallint", nullable: false),
                    BfsMunicipality = table.Column<short>(type: "smallint", nullable: true),
                    EVoterFlag = table.Column<bool>(type: "boolean", nullable: true),
                    ContextId = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EVoters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Filters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    MunicipalityId = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImportStatistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    QueuedFileName = table.Column<string>(type: "text", nullable: false),
                    SourceSystem = table.Column<string>(type: "text", nullable: false),
                    IsManualUpload = table.Column<bool>(type: "boolean", nullable: false),
                    ImportRecordsCountTotal = table.Column<int>(type: "integer", nullable: false),
                    DatasetsCountCreated = table.Column<int>(type: "integer", nullable: false),
                    DatasetsCountUpdated = table.Column<int>(type: "integer", nullable: false),
                    DatasetsCountDeleted = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FinishedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StartedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TotalElapsedMilliseconds = table.Column<double>(type: "double precision", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    MunicipalityId = table.Column<int>(type: "integer", nullable: true),
                    ProcessingErrors = table.Column<string>(type: "text", nullable: true),
                    HasValidationErrors = table.Column<bool>(type: "boolean", nullable: false),
                    EntitiesWithValidationErrors = table.Column<List<Guid>>(type: "uuid[]", nullable: false),
                    RecordNumbersWithValidationErrors = table.Column<List<int>>(type: "integer[]", nullable: false),
                    RecordValidationErrors = table.Column<string>(type: "text", nullable: true),
                    ImportStatus = table.Column<string>(type: "text", nullable: false),
                    ImportType = table.Column<string>(type: "text", nullable: false),
                    WorkerName = table.Column<string>(type: "text", nullable: false),
                    EncryptedKey = table.Column<byte[]>(type: "bytea", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportStatistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Integrities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Bfs = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    SignatureVersion = table.Column<byte>(type: "smallint", nullable: false),
                    SignatureKeyId = table.Column<string>(type: "text", nullable: false),
                    SignatureHash = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Integrities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FilterVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    FilterId = table.Column<Guid>(type: "uuid", nullable: false),
                    SignatureVersion = table.Column<byte>(type: "smallint", nullable: false),
                    SignatureKeyId = table.Column<string>(type: "text", nullable: false),
                    SignatureHash = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilterVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilterVersions_Filters_FilterId",
                        column: x => x.FilterId,
                        principalTable: "Filters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccessControlListDois",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Bfs = table.Column<string>(type: "text", nullable: true),
                    TenantName = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Canton = table.Column<string>(type: "text", nullable: false),
                    IsValid = table.Column<bool>(type: "boolean", nullable: false),
                    ValidationErrors = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    ImportStatisticId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessControlListDois", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessControlListDois_AccessControlListDois_ParentId",
                        column: x => x.ParentId,
                        principalTable: "AccessControlListDois",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessControlListDois_ImportStatistics_ImportStatisticId",
                        column: x => x.ImportStatisticId,
                        principalTable: "ImportStatistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DomainOfInfluences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MunicipalityId = table.Column<int>(type: "integer", nullable: false),
                    DomainOfInfluenceId = table.Column<int>(type: "integer", nullable: false),
                    Street = table.Column<string>(type: "text", nullable: false),
                    HouseNumber = table.Column<string>(type: "text", nullable: false),
                    HouseNumberAddition = table.Column<string>(type: "text", nullable: true),
                    SwissZipCode = table.Column<int>(type: "integer", nullable: false),
                    Town = table.Column<string>(type: "text", nullable: true),
                    IsPartOfPoliticalMunicipality = table.Column<bool>(type: "boolean", nullable: true),
                    PoliticalCircleId = table.Column<string>(type: "text", nullable: true),
                    PoliticalCircleName = table.Column<string>(type: "text", nullable: true),
                    CatholicChurchCircleId = table.Column<string>(type: "text", nullable: true),
                    CatholicChurchCircleName = table.Column<string>(type: "text", nullable: true),
                    EvangelicChurchCircleId = table.Column<string>(type: "text", nullable: true),
                    EvangelicChurchCircleName = table.Column<string>(type: "text", nullable: true),
                    SchoolCircleId = table.Column<string>(type: "text", nullable: true),
                    SchoolCircleName = table.Column<string>(type: "text", nullable: true),
                    TrafficCircleId = table.Column<string>(type: "text", nullable: true),
                    TrafficCircleName = table.Column<string>(type: "text", nullable: true),
                    ResidentialDistrictCircleId = table.Column<string>(type: "text", nullable: true),
                    ResidentialDistrictCircleName = table.Column<string>(type: "text", nullable: true),
                    PeopleCouncilCircleId = table.Column<string>(type: "text", nullable: true),
                    PeopleCouncilCircleName = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsValid = table.Column<bool>(type: "boolean", nullable: false),
                    ValidationErrors = table.Column<string>(type: "text", nullable: true),
                    ImportStatisticId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainOfInfluences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DomainOfInfluences_ImportStatistics_ImportStatisticId",
                        column: x => x.ImportStatisticId,
                        principalTable: "ImportStatistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegisterId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceSystemId = table.Column<string>(type: "text", nullable: true),
                    Vn = table.Column<long>(type: "bigint", nullable: true),
                    OfficialName = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    Sex = table.Column<string>(type: "text", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    DateOfBirthAdjusted = table.Column<bool>(type: "boolean", nullable: false),
                    OriginalName = table.Column<string>(type: "text", nullable: true),
                    AllianceName = table.Column<string>(type: "text", nullable: true),
                    AliasName = table.Column<string>(type: "text", nullable: true),
                    OtherName = table.Column<string>(type: "text", nullable: true),
                    CallName = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    CountryNameShort = table.Column<string>(type: "text", nullable: true),
                    ContactAddressExtensionLine1 = table.Column<string>(type: "text", nullable: true),
                    ContactAddressExtensionLine2 = table.Column<string>(type: "text", nullable: true),
                    ContactAddressStreet = table.Column<string>(type: "text", nullable: true),
                    ContactAddressHouseNumber = table.Column<string>(type: "text", nullable: true),
                    ContactAddressHouseNumberAddition = table.Column<string>(type: "text", nullable: true),
                    ContactAddressDwellingNumber = table.Column<string>(type: "text", nullable: true),
                    ContactAddressPostOfficeBoxText = table.Column<string>(type: "text", nullable: true),
                    ContactAddressTown = table.Column<string>(type: "text", nullable: true),
                    ContactAddressZipCode = table.Column<string>(type: "text", nullable: true),
                    ContactAddressLocality = table.Column<string>(type: "text", nullable: true),
                    ContactAddressLine1 = table.Column<string>(type: "text", nullable: true),
                    ContactAddressLine2 = table.Column<string>(type: "text", nullable: true),
                    ContactAddressLine3 = table.Column<string>(type: "text", nullable: true),
                    ContactAddressLine4 = table.Column<string>(type: "text", nullable: true),
                    ContactAddressLine5 = table.Column<string>(type: "text", nullable: true),
                    ContactAddressLine6 = table.Column<string>(type: "text", nullable: true),
                    ContactAddressLine7 = table.Column<string>(type: "text", nullable: true),
                    ContactCantonAbbreviation = table.Column<string>(type: "text", nullable: true),
                    LanguageOfCorrespondence = table.Column<string>(type: "text", nullable: true),
                    Religion = table.Column<string>(type: "text", nullable: false),
                    ResidencePermit = table.Column<string>(type: "text", nullable: true),
                    ResidencePermitValidFrom = table.Column<DateOnly>(type: "date", nullable: true),
                    ResidencePermitValidTill = table.Column<DateOnly>(type: "date", nullable: true),
                    ResidenceEntryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ResidenceCantonAbbreviation = table.Column<string>(type: "text", nullable: true),
                    MunicipalityName = table.Column<string>(type: "text", nullable: true),
                    MunicipalityId = table.Column<int>(type: "integer", nullable: false),
                    DomainOfInfluenceId = table.Column<int>(type: "integer", nullable: true),
                    MoveInArrivalDate = table.Column<DateOnly>(type: "date", nullable: true),
                    MoveInMunicipalityName = table.Column<string>(type: "text", nullable: true),
                    MoveInCantonAbbreviation = table.Column<string>(type: "text", nullable: true),
                    MoveInComesFrom = table.Column<string>(type: "text", nullable: true),
                    MoveInCountryNameShort = table.Column<string>(type: "text", nullable: true),
                    MoveInUnknown = table.Column<bool>(type: "boolean", nullable: true),
                    ResidenceAddressExtensionLine1 = table.Column<string>(type: "text", nullable: true),
                    ResidenceAddressExtensionLine2 = table.Column<string>(type: "text", nullable: true),
                    ResidenceAddressStreet = table.Column<string>(type: "text", nullable: true),
                    ResidenceAddressHouseNumber = table.Column<string>(type: "text", nullable: true),
                    ResidenceAddressDwellingNumber = table.Column<string>(type: "text", nullable: true),
                    ResidenceAddressPostOfficeBoxText = table.Column<string>(type: "text", nullable: true),
                    ResidenceAddressTown = table.Column<string>(type: "text", nullable: true),
                    ResidenceCountry = table.Column<string>(type: "text", nullable: true),
                    ResidenceAddressZipCode = table.Column<string>(type: "text", nullable: false),
                    TypeOfResidence = table.Column<int>(type: "integer", nullable: false),
                    RestrictedVotingAndElectionRightFederation = table.Column<bool>(type: "boolean", nullable: false),
                    SwissAbroadEvotingFlag = table.Column<bool>(type: "boolean", nullable: true),
                    IsSwissAbroad = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsLatest = table.Column<bool>(type: "boolean", nullable: false),
                    VersionCount = table.Column<int>(type: "integer", nullable: false),
                    IsValid = table.Column<bool>(type: "boolean", nullable: false),
                    ValidationErrors = table.Column<string>(type: "text", nullable: true),
                    ImportStatisticId = table.Column<Guid>(type: "uuid", nullable: true),
                    SignatureVersion = table.Column<byte>(type: "smallint", nullable: false),
                    SignatureKeyId = table.Column<string>(type: "text", nullable: false),
                    SignatureHash = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Persons_ImportStatistics_ImportStatisticId",
                        column: x => x.ImportStatisticId,
                        principalTable: "ImportStatistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "FilterCriteria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ReferenceId = table.Column<string>(type: "text", nullable: false),
                    FilterValue = table.Column<string>(type: "text", nullable: false),
                    FilterId = table.Column<Guid>(type: "uuid", nullable: true),
                    FilterVersionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FilterOperator = table.Column<string>(type: "text", nullable: false),
                    FilterType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilterCriteria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilterCriteria_Filters_FilterId",
                        column: x => x.FilterId,
                        principalTable: "Filters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FilterCriteria_FilterVersions_FilterVersionId",
                        column: x => x.FilterVersionId,
                        principalTable: "FilterVersions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FilterVersionPersons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    FilterVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilterVersionPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilterVersionPersons_FilterVersions_FilterVersionId",
                        column: x => x.FilterVersionId,
                        principalTable: "FilterVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilterVersionPersons_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonDois",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Identifier = table.Column<string>(type: "text", nullable: false),
                    Canton = table.Column<string>(type: "text", nullable: false),
                    DomainOfInfluenceType = table.Column<string>(type: "text", nullable: false),
                    PersonId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonDois", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonDois_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessControlListDois_ImportStatisticId",
                table: "AccessControlListDois",
                column: "ImportStatisticId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessControlListDois_ParentId",
                table: "AccessControlListDois",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_DomainOfInfluences_ImportStatisticId",
                table: "DomainOfInfluences",
                column: "ImportStatisticId");

            migrationBuilder.CreateIndex(
                name: "IX_FilterCriteria_FilterId",
                table: "FilterCriteria",
                column: "FilterId");

            migrationBuilder.CreateIndex(
                name: "IX_FilterCriteria_FilterVersionId",
                table: "FilterCriteria",
                column: "FilterVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_FilterVersionPersons_FilterVersionId",
                table: "FilterVersionPersons",
                column: "FilterVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_FilterVersionPersons_PersonId",
                table: "FilterVersionPersons",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_FilterVersions_FilterId",
                table: "FilterVersions",
                column: "FilterId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonDois_PersonId",
                table: "PersonDois",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_ImportStatisticId",
                table: "Persons",
                column: "ImportStatisticId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessControlListDois");

            migrationBuilder.DropTable(
                name: "DomainOfInfluences");

            migrationBuilder.DropTable(
                name: "EVoterAudits");

            migrationBuilder.DropTable(
                name: "EVoters");

            migrationBuilder.DropTable(
                name: "FilterCriteria");

            migrationBuilder.DropTable(
                name: "FilterVersionPersons");

            migrationBuilder.DropTable(
                name: "Integrities");

            migrationBuilder.DropTable(
                name: "PersonDois");

            migrationBuilder.DropTable(
                name: "FilterVersions");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "Filters");

            migrationBuilder.DropTable(
                name: "ImportStatistics");
        }
    }
}

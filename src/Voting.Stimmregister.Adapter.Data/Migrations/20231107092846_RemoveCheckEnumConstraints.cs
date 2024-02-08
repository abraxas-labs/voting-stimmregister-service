using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    public partial class RemoveCheckEnumConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""Persons"" DROP CONSTRAINT CK_PERSON_SOURCESYSTEMNAME");
            migrationBuilder.Sql(@"ALTER TABLE ""Persons"" DROP CONSTRAINT CK_PERSON_RELIGION");
            migrationBuilder.Sql(@"ALTER TABLE ""Persons"" DROP CONSTRAINT CK_PERSON_SEX");

            migrationBuilder.Sql(@"ALTER TABLE ""ImportStatistics"" DROP CONSTRAINT CK_IMPSTATS_IMPORTTYPE");
            migrationBuilder.Sql(@"ALTER TABLE ""ImportStatistics"" DROP CONSTRAINT CK_IMPSTATS_SOURCESYSTEM");
            migrationBuilder.Sql(@"ALTER TABLE ""ImportStatistics"" DROP CONSTRAINT CK_IMPSTATS_IMPORTSTATUS");

            migrationBuilder.Sql(@"ALTER TABLE ""AccessControlListDois"" DROP CONSTRAINT CK_ACLDOI_CANTON");
            migrationBuilder.Sql(@"ALTER TABLE ""AccessControlListDois"" DROP CONSTRAINT CK_ADLDOI_TYPE");

            migrationBuilder.Sql(@"ALTER TABLE ""FilterCriteria"" DROP CONSTRAINT CK_FC_FILTEROPERATOR");
            migrationBuilder.Sql(@"ALTER TABLE ""FilterCriteria"" DROP CONSTRAINT CK_FC_FILTERTYPE");
            migrationBuilder.Sql(@"ALTER TABLE ""FilterCriteria"" DROP CONSTRAINT CK_FC_REFERENCEID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""Persons"" ADD CONSTRAINT CK_PERSON_SOURCESYSTEMNAME CHECK (""SourceSystemName"" in ('Unspecified', 'Loganto', 'Cobra', 'VotingBasis', 'Innosolv'))");
            migrationBuilder.Sql(@"ALTER TABLE ""Persons"" ADD CONSTRAINT CK_PERSON_RELIGION CHECK(""Religion"" in('Unknown', 'Evangelic', 'Catholic', 'ChristCatholic'));");
            migrationBuilder.Sql(@"ALTER TABLE ""Persons"" ADD CONSTRAINT CK_PERSON_SEX CHECK(""Sex"" in('Male', 'Female', 'Undefined'));");

            migrationBuilder.Sql(@"ALTER TABLE ""ImportStatistics"" ADD CONSTRAINT CK_IMPSTATS_IMPORTTYPE CHECK (""ImportType"" in ('Person', 'DomainOfInfluence', 'Acl'))");
            migrationBuilder.Sql(@"ALTER TABLE ""ImportStatistics"" ADD CONSTRAINT CK_IMPSTATS_SOURCESYSTEM CHECK (""SourceSystem"" in ('Loganto', 'Cobra', 'VotingBasis', 'Innosolv'))");
            migrationBuilder.Sql(@"ALTER TABLE ""ImportStatistics"" ADD CONSTRAINT CK_IMPSTATS_IMPORTSTATUS CHECK (""ImportStatus"" in ('Queued', 'Running', 'Aborted', 'FinishedWithErrors', 'FinishedSuccessfully', 'Stale', 'Failed'))");

            migrationBuilder.Sql(@"ALTER TABLE ""AccessControlListDois"" ADD CONSTRAINT CK_ACLDOI_CANTON CHECK(""Canton"" in('Unknown', 'SG', 'TG', 'ZH', 'BE', 'LU', 'UR', 'SZ', 'OW', 'NW', 'GL', 'ZG', 'FR', 'SO', 'BS', 'BL', 'SH', 'AR', 'AI', 'GR', 'AG', 'TI', 'VD', 'VS', 'NE', 'GE', 'JU', 'FL'));");
            migrationBuilder.Sql(@"ALTER TABLE ""AccessControlListDois"" ADD CONSTRAINT CK_ADLDOI_TYPE CHECK(""Type"" in('Unspecified', 'Ch', 'Ct', 'Bz', 'Mu', 'Sk', 'Sc', 'Ki', 'Og', 'Ko', 'An', 'KiKat', 'KiEva', 'AnVek', 'AnWok', 'AnVok'))");

            migrationBuilder.Sql(@"ALTER TABLE ""FilterCriteria"" ADD CONSTRAINT CK_FC_FILTEROPERATOR CHECK (""FilterOperator"" in ('Contains', 'Equals', 'StartsWith', 'EndsWith', 'Less', 'LessEqual', 'Greater', 'GreaterEqual'))");
            migrationBuilder.Sql(@"ALTER TABLE ""FilterCriteria"" ADD CONSTRAINT CK_FC_FILTERTYPE CHECK (""FilterType"" in ('String', 'Date', 'Boolean', 'Numeric', 'Select', 'Multiselect'))");
            migrationBuilder.Sql(@"ALTER TABLE ""FilterCriteria"" ADD CONSTRAINT CK_FC_REFERENCEID CHECK (""ReferenceId"" in (
                'Unspecified',
                'Vn',
                'MunicipalityId',
                'RestrictedVotingAndElectionRightFederation',
                'Age',
                'SwissCitizenship',
                'MunicipalityName',
                'OfficialName',
                'FirstName',
                'Religion',
                'Country',
                'EVoting',
                'Sex',
                'DateOfBirth',
                'DateOfBirthAdjusted',
                'OriginalName',
                'AllianceName',
                'AliasName',
                'OtherName',
                'CallName',
                'TypeOfResidence',
                'OriginName17',
                'OriginOnCanton17',
                'ResidencePermit',
                'ResidencePermitValidFrom',
                'ResidencePermitValidTill',
                'ResidenceEntryDate',
                'ContactAddressExtensionLine1',
                'ContactAddressExtensionLine2',
                'ContactAddressStreet',
                'ContactAddressHouseNumber',
                'ContactAddressPostOfficeBoxText',
                'ContactAddressLine17',
                'ContactAddressTown',
                'ContactAddressLocality',
                'ContactAddressZipCode',
                'ResidenceAddressExtensionLine1',
                'ResidenceAddressExtensionLine2',
                'ResidenceAddressStreet',
                'ResidenceAddressHouseNumber',
                'ResidenceAddressPostOfficeBoxText',
                'ResidenceAddressTown',
                'ResidenceAddressZipCode',
                'MoveInArrivalDate',
                'MoveInMunicipalityName',
                'MoveInCantonAbbreviation',
                'MoveInComesFrom',
                'MoveInCountryNameShort',
                'MoveInUnknown',
                'PoliticalCircleId',
                'PoliticalCircleName',
                'CatholicCircleId',
                'CatholicCircleName',
                'EvangelicCircleId',
                'EvangelicCircleName',
                'SchoolCircleId',
                'SchoolCircleName',
                'TrafficCircleId',
                'TrafficCircleName',
                'ResidentialDistrictCircleId',
                'ResidentialDistrictCircleName',
                'PeopleCircleId',
                'PeopleCircleName',
                'HasValidationErrors',
                'SendVotingCardsToDomainOfInfluenceReturnAddress'))");
        }
    }
}

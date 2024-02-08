using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    public partial class FilterReferenceIdToEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""FilterCriteria"" DROP CONSTRAINT CK_FC_REFERENCEID");
        }
    }
}

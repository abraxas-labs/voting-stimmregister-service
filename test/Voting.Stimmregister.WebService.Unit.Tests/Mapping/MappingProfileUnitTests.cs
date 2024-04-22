// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using Voting.Stimmregister.WebService.Mapping;
using Xunit;

namespace Voting.Stimmregister.WebService.Unit.Tests.Mapping;

public class MappingProfileUnitTests
{
    [Fact]
    public void WhenDomainOfInfluenceProfileConfigured_ShouldBeValid()
    {
        var mapperConfiguration = new MapperConfiguration(configuration =>
        {
            configuration.EnableEnumMappingValidation();
            configuration.AddProfile<DomainOfInfluenceProfile>();
        });

        mapperConfiguration.AssertConfigurationIsValid();
    }

    [Fact]
    public void WhenFilterProfileConfigured_ShouldBeValid()
    {
        var mapperConfiguration = new MapperConfiguration(configuration =>
        {
            configuration.EnableEnumMappingValidation();
            configuration.AddProfile<ConverterProfile>();
            configuration.AddProfile<FilterProfile>();
            configuration.AddProfile<AuditInfoProfile>();
        });

        mapperConfiguration.AssertConfigurationIsValid();
    }

    [Fact]
    public void WhenImportStatisticProfileConfigured_ShouldBeValid()
    {
        var mapperConfiguration = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile<ConverterProfile>();
            configuration.AddProfile<ImportStatisticProfile>();
            configuration.AddProfile<AuditInfoProfile>();
        });

        mapperConfiguration.AssertConfigurationIsValid();
    }

    [Fact]
    public void WhenPersonProfileConfigured_ShouldBeValid()
    {
        var mapperConfiguration = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile<ConverterProfile>();
            configuration.AddProfile<FilterProfile>();
            configuration.AddProfile<PersonProfile>();
            configuration.AddProfile<DomainOfInfluenceProfile>();
            configuration.AddProfile<AuditInfoProfile>();
        });

        mapperConfiguration.AssertConfigurationIsValid();
    }

    [Fact]
    public void WhenRegistrationStatisticConfigured_ShouldBeValid()
    {
        var mapperConfiguration = new MapperConfiguration(configuration =>
        {
            configuration.EnableEnumMappingValidation();
            configuration.AddProfile<RegistrationStatisticProfile>();
        });

        mapperConfiguration.AssertConfigurationIsValid();
    }
}

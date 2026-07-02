// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using Microsoft.Extensions.Logging.Abstractions;
using Voting.Stimmregister.WebService.Mapping;
using Xunit;

namespace Voting.Stimmregister.WebService.Unit.Tests.Mapping;

public class MappingProfileUnitTests
{
    [Fact]
    public void WhenDomainOfInfluenceProfileConfigured_ShouldBeValid()
    {
        var configExpr = new MapperConfigurationExpression();
        configExpr.EnableEnumMappingValidation();
        configExpr.AddProfile<DomainOfInfluenceProfile>();
        var mapperConfiguration = new MapperConfiguration(configExpr, NullLoggerFactory.Instance);

        mapperConfiguration.AssertConfigurationIsValid();
    }

    [Fact]
    public void WhenFilterProfileConfigured_ShouldBeValid()
    {
        var configExpr = new MapperConfigurationExpression();
        configExpr.EnableEnumMappingValidation();
        configExpr.AddProfile<ConverterProfile>();
        configExpr.AddProfile<FilterProfile>();
        configExpr.AddProfile<AuditInfoProfile>();
        var mapperConfiguration = new MapperConfiguration(configExpr, NullLoggerFactory.Instance);

        mapperConfiguration.AssertConfigurationIsValid();
    }

    [Fact]
    public void WhenImportStatisticProfileConfigured_ShouldBeValid()
    {
        var configExpr = new MapperConfigurationExpression();
        configExpr.AddProfile<ConverterProfile>();
        configExpr.AddProfile<ImportStatisticProfile>();
        configExpr.AddProfile<AuditInfoProfile>();
        var mapperConfiguration = new MapperConfiguration(configExpr, NullLoggerFactory.Instance);

        mapperConfiguration.AssertConfigurationIsValid();
    }

    [Fact]
    public void WhenPersonProfileConfigured_ShouldBeValid()
    {
        var configExpr = new MapperConfigurationExpression();
        configExpr.AddProfile<ConverterProfile>();
        configExpr.AddProfile<FilterProfile>();
        configExpr.AddProfile<PersonProfile>();
        configExpr.AddProfile<DomainOfInfluenceProfile>();
        configExpr.AddProfile<AuditInfoProfile>();
        configExpr.AddProfile<ECollectingProfile>();
        var mapperConfiguration = new MapperConfiguration(configExpr, NullLoggerFactory.Instance);

        mapperConfiguration.AssertConfigurationIsValid();
    }

    [Fact]
    public void WhenRegistrationStatisticConfigured_ShouldBeValid()
    {
        var configExpr = new MapperConfigurationExpression();
        configExpr.EnableEnumMappingValidation();
        configExpr.AddProfile<RegistrationStatisticProfile>();
        var mapperConfiguration = new MapperConfiguration(configExpr, NullLoggerFactory.Instance);

        mapperConfiguration.AssertConfigurationIsValid();
    }
}

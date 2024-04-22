// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Models;

/// <summary>
/// Return address of political domain of influence.
/// </summary>
public class AccessControlListDoiReturnAddress
{
    public string? AddressLine1 { get; set; } = string.Empty;

    public string? AddressLine2 { get; set; } = string.Empty;

    public string? Street { get; set; } = string.Empty;

    public string? AddressAddition { get; set; } = string.Empty;

    public string? ZipCode { get; set; } = string.Empty;

    public string? City { get; set; } = string.Empty;

    public string? Country { get; set; } = string.Empty;
}

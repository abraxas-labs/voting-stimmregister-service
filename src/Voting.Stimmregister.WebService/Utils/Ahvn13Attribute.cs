// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.ComponentModel.DataAnnotations;
using Voting.Lib.Common;

namespace Voting.Stimmregister.WebService.Utils;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class Ahvn13Attribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        return value is string ahvn13 && Ahvn13.IsValid(ahvn13);
    }
}

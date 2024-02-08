// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Domain.Exceptions;

public class InvalidSearchFilterCriteriaException : Exception
{
    public InvalidSearchFilterCriteriaException()
        : base("Filter criteria is invalid.")
    {
    }

    public InvalidSearchFilterCriteriaException(string? message)
        : base(message)
    {
    }

    public InvalidSearchFilterCriteriaException(FilterCriteriaEntity filterCriteria)
        : base("Filter criteria is invalid. " +
            $"Id: {filterCriteria.Id}, " +
            $"Reference Field: {filterCriteria.ReferenceId}, " +
            $"Data Type: {filterCriteria.FilterType}, " +
            $"Operator: {filterCriteria.FilterOperator}")
    {
    }

    public InvalidSearchFilterCriteriaException(FilterCriteriaEntity filterCriteria, string message = "")
        : base(message +
            $"Id: {filterCriteria.Id}, " +
            $"Reference Field: {filterCriteria.ReferenceId}, " +
            $"Data Type: {filterCriteria.FilterType}, " +
            $"Operator: {filterCriteria.FilterOperator}")
    {
    }

    public InvalidSearchFilterCriteriaException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}

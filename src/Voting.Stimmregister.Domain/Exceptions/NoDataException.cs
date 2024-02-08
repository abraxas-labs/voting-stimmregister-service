// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmregister.Domain.Exceptions;

public class NoDataException : Exception
{
    public NoDataException()
        : base("No data found.")
    {
    }

    public NoDataException(string? message)
        : base(message)
    {
    }

    public NoDataException(string? message, Exception? innerException)
    : base(message, innerException)
    {
    }
}

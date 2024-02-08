// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Voting.Stimmregister.Domain.Constants.EVoting;

namespace Voting.Stimmregister.Domain.Exceptions;

public class EVotingSubsystemException : Exception
{
    public EVotingSubsystemException(string? message)
    : base(message)
    {
    }

    public EVotingSubsystemException(string? message, ProcessStatusCode statusCode)
    : base(message)
    {
        StatusCode = statusCode;
    }

    public EVotingSubsystemException(string? message, Exception? innerException)
    : base(message, innerException)
    {
    }

    public ProcessStatusCode StatusCode { get; }
}

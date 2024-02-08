// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Voting.Stimmregister.Domain.Constants.EVoting;

namespace Voting.Stimmregister.Domain.Exceptions;

public class EVotingRegistrationException : Exception
{
    public EVotingRegistrationException(string message, ProcessStatusCode statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public ProcessStatusCode StatusCode { get; }
}

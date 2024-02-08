// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmregister.Core.Services.Supporting.Signing.Exceptions;

public class SignatureInvalidException : Exception
{
    public SignatureInvalidException(string entityType, object id)
        : base($"{entityType} with id {id} has an invalid signature")
    {
    }
}

// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmregister.Core.Services.Supporting.Signing.Exceptions;

public class DecryptionException : Exception
{
    public DecryptionException(string? message)
        : base(message)
    {
    }
}

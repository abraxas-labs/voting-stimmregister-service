// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmregister.WebService.Exceptions;

public class InvalidImportPayloadException : Exception
{
    public InvalidImportPayloadException()
        : base("ImportAcl failed due to invalid payload.")
    {
    }

    public InvalidImportPayloadException(string? message)
        : base(message)
    {
    }

    public InvalidImportPayloadException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}

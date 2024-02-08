// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmregister.WebService.Exceptions;

public class InvalidExportPayloadException : Exception
{
    public InvalidExportPayloadException()
        : base("Export failed due to invalid parameters.")
    {
    }

    public InvalidExportPayloadException(string? message)
        : base(message)
    {
    }

    public InvalidExportPayloadException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}

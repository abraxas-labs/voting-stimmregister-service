﻿// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmregister.Import.Loganto.Exceptions;

public class ImportException : Exception
{
    public ImportException(string message)
        : base(message)
    {
    }
}

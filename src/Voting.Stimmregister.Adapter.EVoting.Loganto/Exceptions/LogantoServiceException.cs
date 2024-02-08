// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Stimmregister.Domain.Constants.EVoting;
using Voting.Stimmregister.Domain.Exceptions;

namespace Voting.Stimmregister.Adapter.EVoting.Loganto.Exceptions;

public class LogantoServiceException : EVotingSubsystemException
{
    public LogantoServiceException(string message, ProcessStatusCode statusCode)
        : base(message, statusCode)
    {
    }
}

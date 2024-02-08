// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Stimmregister.Domain.Constants.EVoting;
using Voting.Stimmregister.Domain.Exceptions;

namespace Voting.Stimmregister.Adapter.EVoting.Kewr.Exceptions;

public class KewrServiceException : EVotingSubsystemException
{
    public KewrServiceException(string message, ProcessStatusCode statusCode)
        : base(message, statusCode)
    {
    }
}

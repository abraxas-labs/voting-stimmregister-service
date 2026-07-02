// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Voting.Lib.Common;
using Voting.Lib.Grpc;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Proto.V1.Services;
using Voting.Stimmregister.Proto.V1.Services.Models;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Proto.V1.Services.Responses;
using Voting.Stimmregister.WebService.Auth;

namespace Voting.Stimmregister.WebService.Services;

public class ImportGrpcService(
    IManualImportSecondFactorTransactionService manualImportSecondFactorService,
    IMapper mapper)
    : ImportService.ImportServiceBase
{
    [AuthorizeManualImporter]
    public override async Task<PrepareManualImportResponse> PrepareManualImport(
        Empty request,
        ServerCallContext context)
    {
        var info = await manualImportSecondFactorService.Prepare();

        return new PrepareManualImportResponse
        {
            SecondFactorTransaction = mapper.Map<SecondFactorTransactionModel>(info),
        };
    }

    [AuthorizeManualImporter]
    public override async Task<Empty> VerifyManualImport(
        VerifyManualImportRequest request,
        ServerCallContext context)
    {
        var transactionId = GuidParser.Parse(request.SecondFactorTransactionId);
        await manualImportSecondFactorService.Verify(transactionId, request.OtpCode, context.CancellationToken);
        return ProtobufEmpty.Instance;
    }
}

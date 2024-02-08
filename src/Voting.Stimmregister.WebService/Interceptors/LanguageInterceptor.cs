// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Voting.Stimmregister.Abstractions.Core.Services;

namespace Voting.Stimmregister.WebService.Interceptors;

public class LanguageInterceptor : Interceptor
{
    private const string LangHeader = "x-language";
    private readonly ILanguageService _languageService;

    public LanguageInterceptor(ILanguageService languageService)
    {
        _languageService = languageService;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        SetRequestLanguage(context.RequestHeaders);
        return await base.UnaryServerHandler(request, context, continuation);
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        SetRequestLanguage(context.RequestHeaders);
        await base.ServerStreamingServerHandler(request, responseStream, context, continuation);
    }

    private void SetRequestLanguage(Metadata requestHeaders)
    {
        var language = requestHeaders.GetValue(LangHeader);
        _languageService.SetLanguage(language);
    }
}

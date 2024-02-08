// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Voting.Stimmregister.Domain.Constants.EVoting;
using Voting.Stimmregister.Domain.Exceptions;
using Voting.Stimmregister.WebService.Models.EVoting.Response;

namespace Voting.Stimmregister.WebService.Exceptions;

public class EVotingExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly ILogger<EVotingExceptionFilterAttribute> _logger;

    public EVotingExceptionFilterAttribute(ILogger<EVotingExceptionFilterAttribute> logger)
    {
        _logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {
        ProcessStatusResponseBase response = new();

        // Set default behavior for business exceptions
        context.HttpContext.Response.ContentType = "application/json";
        context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Result = new JsonResult(response);

        if (context.Exception is EVotingSubsystemException ex)
        {
            response.ProcessStatusCode = ex.StatusCode;
            response.ProcessStatusMessage = ex.Message;
        }
        else if (context.Exception is EVotingValidationException exValidation)
        {
            response.ProcessStatusCode = exValidation.StatusCode;
            response.ProcessStatusMessage = exValidation.Message;
        }
        else if (context.Exception is EVotingNotPermittedException exPermission)
        {
            response.ProcessStatusCode = exPermission.StatusCode;
            response.ProcessStatusMessage = exPermission.Message;
        }
        else if (context.Exception is EVotingNotEnabledException exEnabled)
        {
            response.ProcessStatusCode = exEnabled.StatusCode;
            response.ProcessStatusMessage = exEnabled.Message;
        }
        else if (context.Exception is EVotingRegistrationException exRegistration)
        {
            response.ProcessStatusCode = exRegistration.StatusCode;
            response.ProcessStatusMessage = exRegistration.Message;
        }
        else if (context.Exception is InvalidOperationException exOperation)
        {
            response.ProcessStatusCode = ProcessStatusCode.Unknown;
            response.ProcessStatusMessage = exOperation.Message;
        }
        else if (context.Exception is HttpRequestException)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            response.ProcessStatusMessage = nameof(HttpRequestException);
        }
        else if (context.Exception is ArgumentException)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            response.ProcessStatusMessage = nameof(ArgumentException);
        }
        else
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            response.ProcessStatusMessage = context.Exception.GetType().Name;
        }

        _logger.LogError(context.Exception, "[Code:{ProcessStatusCode}] {ProcessStatusMessage}", response.ProcessStatusCode, response.ProcessStatusMessage);
    }
}

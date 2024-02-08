// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Voting.Lib.Iam.Exceptions;
using Voting.Stimmregister.Core.Services.Supporting.Signing.Exceptions;
using Voting.Stimmregister.Domain.Exceptions;

namespace Voting.Stimmregister.WebService.Exceptions;

internal readonly struct ExceptionMapping
{
    private const string EnumMappingErrorSource = "AutoMapper.Extensions.EnumMapping";
    private readonly StatusCode _grpcStatusCode;
    private readonly int _httpStatusCode;
    private readonly bool _exposeExceptionType;

    public ExceptionMapping(StatusCode grpcStatusCode, int httpStatusCode, bool exposeExceptionType = false)
    {
        _grpcStatusCode = grpcStatusCode;
        _httpStatusCode = httpStatusCode;
        _exposeExceptionType = exposeExceptionType;
    }

    public static int MapToHttpStatusCode(Exception ex)
        => Map(ex)._httpStatusCode;

    public static StatusCode MapToGrpcStatusCode(Exception ex)
        => Map(ex)._grpcStatusCode;

    public static bool ExposeExceptionType(Exception ex)
        => Map(ex)._exposeExceptionType;

    private static ExceptionMapping Map(Exception ex)
        => ex switch
        {
            NoDataException _ => new ExceptionMapping(StatusCode.Unavailable, StatusCodes.Status204NoContent),
            NotAuthenticatedException _ => new ExceptionMapping(StatusCode.Unauthenticated, StatusCodes.Status401Unauthorized),
            ForbiddenException _ => new ExceptionMapping(StatusCode.PermissionDenied, StatusCodes.Status403Forbidden),
            FluentValidation.ValidationException _ => new ExceptionMapping(StatusCode.InvalidArgument, StatusCodes.Status400BadRequest),
            EntityNotFoundException _ => new ExceptionMapping(StatusCode.NotFound, StatusCodes.Status404NotFound),
            InvalidSearchFilterCriteriaException _ => new ExceptionMapping(StatusCode.InvalidArgument, StatusCodes.Status400BadRequest),
            InvalidImportPayloadException _ => new ExceptionMapping(StatusCode.InvalidArgument, StatusCodes.Status400BadRequest),
            InvalidExportPayloadException _ => new ExceptionMapping(StatusCode.InvalidArgument, StatusCodes.Status400BadRequest),
            AutoMapperMappingException autoMapperException when autoMapperException.InnerException is not null => Map(autoMapperException.InnerException),
            AutoMapperMappingException autoMapperException when string.Equals(autoMapperException.Source, EnumMappingErrorSource) => new(StatusCode.InvalidArgument, StatusCodes.Status400BadRequest),
            ValidationException _ => new ExceptionMapping(StatusCode.InvalidArgument, StatusCodes.Status400BadRequest),
            SignatureInvalidException _ => new ExceptionMapping(StatusCode.Internal, StatusCodes.Status500InternalServerError, true),
            _ => new ExceptionMapping(StatusCode.Internal, StatusCodes.Status500InternalServerError),
        };
}

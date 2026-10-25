using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using TeamPulse.Api.Responses;
using TeamPulse.Application.Exceptions;

namespace TeamPulse.Api.Middleware;

public static class ErrorHandlingServiceCollectionExtensions
{
    public static IServiceCollection AddTeamPulseErrorHandling(
        this IServiceCollection services)
    {
        services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory =
                    context =>
                        new BadRequestObjectResult(
                            new ApiErrorResponse
                            {
                                Status = 400,
                                Message = "Invalid request."
                            });
            });

        return services;
    }
}

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            if (!context.Response.HasStarted)
            {
                if (context.Response.StatusCode == 401)
                {
                    await WriteErrorAsync(
                        context,
                        401,
                        "Authentication required.");
                }
                else if (context.Response.StatusCode == 403)
                {
                    await WriteErrorAsync(
                        context,
                        403,
                        "Access forbidden.");
                }
                else if (context.Response.StatusCode == 404)
                {
                    await WriteErrorAsync(
                        context,
                        404,
                        "Requested resource not found.");
                }
            }
        }
        catch (AppException ex)
        {
            var error = ex.Code switch
            {
                "ALREADY_ANSWERED" =>
                    new ApiErrorResponse
                    {
                        Status = 409,
                        Message = "Today's response has already been submitted."
                    },

                "INVALID_QUESTION" =>
                    new ApiErrorResponse
                    {
                        Status = 400,
                        Message = "The submitted question is not the expected question."
                    },

                "INVALID_RATING" =>
                    new ApiErrorResponse
                    {
                        Status = 400,
                        Message = "Rating must be between 1 and 5."
                    },

                "NO_ACTIVE_QUESTIONS" =>
                    new ApiErrorResponse
                    {
                        Status = 400,
                        Message = "No active questions are available."
                    },

                _ =>
                    new ApiErrorResponse
                    {
                        Status = 500,
                        Message = "An unexpected server error occurred."
                    }
            };

            await WriteErrorAsync(
                context,
                error.Status,
                error.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An unexpected error occurred.");

            await WriteErrorAsync(
                context,
                500,
                "An unexpected server error occurred.");
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        int status,
        string message)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(
            new ApiErrorResponse
            {
                Status = status,
                Message = message
            });
    }
}
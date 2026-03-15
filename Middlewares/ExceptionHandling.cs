using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TechBlogApi.Exceptions;

namespace TechBlogApi.Middlewares
{
    public class ExceptionHandling
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandling> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandling(RequestDelegate next, ILogger<ExceptionHandling> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured! TraceId : {traceId}", context.TraceIdentifier);

                var problem = CreateProblemDetails(context, ex);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = problem.Status ?? (int)HttpStatusCode.InternalServerError;

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                await context.Response.WriteAsJsonAsync(problem, options);
            }
        }

        private ProblemDetails CreateProblemDetails(HttpContext context, Exception ex)
        {
            ProblemDetails problem = new ProblemDetails
            {
                Instance = context.Request.Path,
                Detail = _env.EnvironmentName == "Development" ? ex.ToString() : "Server Error",
                Title = "Server Error",
                Status = (int)HttpStatusCode.InternalServerError
            };

            problem.Extensions["traceId"] = context.TraceIdentifier;

            if (ex is NotFoundException)
            {
                problem.Status = (int)HttpStatusCode.NotFound;
                problem.Title = "Source not found";
            }
            else if (ex is BadRequestException)
            {
                problem.Status = (int)HttpStatusCode.BadRequest;
                problem.Title = "Bad Request";
            }
            return problem;
        }
    }
}
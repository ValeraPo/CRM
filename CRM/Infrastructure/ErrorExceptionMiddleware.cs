using CRM.APILayer.Models.Response;
using CRM.BusinessLayer.Exceptions;
using NLog;
using System.Net;
using System.Text.Json;
using FluentValidation;

namespace CRM.APILayer.Infrastructure
{
    // Global error handling
    public class ErrorExceptionMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly Logger _logger;

        public ErrorExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ForbiddenException error)
            {
                await ConstructResponse(context, HttpStatusCode.Forbidden, error.Message);
            }
            catch (NotFoundException error)
            {
                await ConstructResponse(context, HttpStatusCode.NotFound, error.Message);
            }
            catch (BadRequestException error)
            {
                await ConstructResponse(context, HttpStatusCode.BadRequest, error.Message);
            }
            catch (ValidationException ex)
            {
                await ConstructResponse(context, HttpStatusCode.UnprocessableEntity, ex.Message);
            }
            catch (DuplicationException error)
            {
                await ConstructResponse(context, HttpStatusCode.Conflict, error.Message);
            }
            catch (RequestTimeoutException error)
            {
                await ConstructResponse(context, HttpStatusCode.GatewayTimeout, error.Message);
            }
            catch (ServiceUnavailableException error)
            {
                await ConstructResponse(context, HttpStatusCode.ServiceUnavailable, error.Message);
            }
            catch (BadGatewayException error)
            {
                await ConstructResponse(context, HttpStatusCode.BadGateway, error.Message);
            }
            catch (InternalServerError error)
            {
                await ConstructResponse(context, HttpStatusCode.InternalServerError, error.Message);
            }
            catch (System.Data.SqlClient.SqlException error)
            {
                _logger.Error(error);
                await ConstructResponse(context, HttpStatusCode.ServiceUnavailable, message: "База данных недоступна");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                await ConstructResponse(context, HttpStatusCode.BadRequest, ex.Message);
            }
        }

        // If we catch error, program will not stop working
        private async Task ConstructResponse(HttpContext context, HttpStatusCode code, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            var updateModel = new ExceptionResponse { Code = (int)code, Message = message };

            var result = JsonSerializer.Serialize(updateModel);
            await context.Response.WriteAsync(result);
        }
    }
}


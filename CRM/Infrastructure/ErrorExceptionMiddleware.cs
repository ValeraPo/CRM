using CRM.APILayer.Models.Response;
using CRM.BusinessLayer.Exceptions;
using System.Net;
using System.Text.Json;

namespace CRM.APILayer.Infrastructure
{
    public class ErrorExceptionMiddleware
    {

        private readonly RequestDelegate _next;


        public ErrorExceptionMiddleware(RequestDelegate next)
        {
            _next = next;

        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AuthorizationException error)
            {
                await ConstructResponse(context, HttpStatusCode.Forbidden, error.Message);
            }
            catch (System.Data.SqlClient.SqlException)
            {
                // ошибка когда БД вообще отсуствует или миграция иная на БД и стобцы не сходятся.
                await ConstructResponse(context, HttpStatusCode.ServiceUnavailable, message: "База данных недоступна");
            }
            catch (NotFoundException error)
            {
                await ConstructResponse(context, HttpStatusCode.Forbidden, error.Message);
            }
            catch (TypeMismatchException error)
            {
                await ConstructResponse(context, HttpStatusCode.Forbidden, error.Message);
            }
            catch (IncorrectPasswordException error)
            {
                await ConstructResponse(context, HttpStatusCode.Forbidden, error.Message);
            }
            catch (DuplicationException error)
            {
                await ConstructResponse(context, HttpStatusCode.Conflict, error.Message);
            }
            catch (BannedException error)
            {
                await ConstructResponse(context, HttpStatusCode.Forbidden, error.Message);
            }
            catch (EntryPointNotFoundException error)
            {
                await ConstructResponse(context, HttpStatusCode.NotFound, error.Message);
            }
            catch (Exception ex)
            {
                await ConstructResponse(context, HttpStatusCode.BadRequest, ex.Message);
            }
        }

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


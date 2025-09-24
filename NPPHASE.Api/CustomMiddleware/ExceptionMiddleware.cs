﻿using NPPHASE.Data.Interface;
using NPPHASE.Data.Model;
using NPPHASE.Services.IRepositories;
using System.Net;

namespace NPPHASE.Apis.CustomMiddleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;



        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {

                
                await HandleExceptionAsync(httpContext, ex);
                throw;
            }
        }
        public async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            using (var scope = context.RequestServices.CreateScope())
            {
                var routeData = context.GetRouteData();
                var _exceptionLoggerServices = scope.ServiceProvider.GetRequiredService<IExceptionLoggerServices>();
                var exception = new Exceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    Timestamp = DateTime.UtcNow,
                    ScreenName = routeData.Values["controller"]?.ToString() + "/" + routeData.Values["action"]?.ToString(),
                };
                _exceptionLoggerServices.InsertErrorLog(exception);
            }
           
        }
    }
}

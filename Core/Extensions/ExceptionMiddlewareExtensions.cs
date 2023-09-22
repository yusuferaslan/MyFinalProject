using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app) //WebAPI' deki IApplicationBuilder'ı extend ediyoruz(genişletiyoruz) hata yakalamayı ekliyoruz.
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}

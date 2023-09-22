using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions
{
    public class ExceptionMiddleware  //HTTP isteklerini işlerken ortaya çıkan istisnaları(exception) ele alır ve uygun yanıtlar oluşturur. 
    {
        private RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext) //HTTP isteği geldiğinde çağrılır
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception e)  //bu işlem sırasında bir istisna (exception) oluşursa, istisna catch bloğunda ele alınır ve HandleExceptionAsync metodu çağrılır.
            {
                await HandleExceptionAsync(httpContext, e); // istisnayı ele alır ve uygun bir HTTP yanıtı oluşturur.
            }
        }

        private Task HandleExceptionAsync(HttpContext httpContext, Exception e)
        {
            httpContext.Response.ContentType = "application/json"; //yanıtın içeriğini "application/json" olarak ayarlar.
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError; //yanıtın durum kodunu "InternalServerError" (HTTP durum kodu 500) olarak ayarlar.

            string message = "Internal Server Error";  //bu varsayılan hata mesajıdır.
            IEnumerable<ValidationFailure> errors;  //errors değişkeni doğrulama işlemlerinde oluşan hata ayrıntılarını tutmak için kullanılır

            if (e.GetType() == typeof(ValidationException))  //catch bloğunda, yakalanan istisna ValidationException türünde mi diye kontrol edilir. bu koşul sağlanıyorsa bir doğrulama hatası olduğunu gösterir.
            {
                message = e.Message;
                errors = ((ValidationException)e).Errors;
                httpContext.Response.StatusCode = 400;

                return httpContext.Response.WriteAsync(new ValidationErrorDetails
                {
                    StatusCode = 400,  //HTTP 400 Bad Request yanıt durum kodu
                    Message = message, //ValidationException dan gelen hata mesajı
                    ValidationErrors = errors  //ValidationException dan gelen hata

                }.ToString());
            }

            //Eğer yakalanan istisna ValidationException türünde değilse yani genel bir hata olduğunda(sistemsel bi hata), ErrorDetails nesnesi oluşturulur ve bu nesne HTTP yanıtında gönderilir.
            return httpContext.Response.WriteAsync(new ErrorDetails
            {
                StatusCode = httpContext.Response.StatusCode,  //InternalServerError (HTTP durum kodu 500)
                Message = message  //varsayılan hata mesajı

            }.ToString());
        }
    }
}

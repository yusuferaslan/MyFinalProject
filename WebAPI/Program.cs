using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Business.Abstract;
using Business.Concrete;
using Business.DependencyResolvers.Autofac;
using Core.DependencyResolvers;
using Core.Extensions;
using Core.Utilities.IoC;
using Core.Utilities.Security.Encryption;
using Core.Utilities.Security.JWT;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors();
            //Autofac, Ninject, CastleWindsor, StructureMap, LightInject, DryInject -- > IoC Container

            //Bana arka planda bir referans olu�tur. IoC bizim yerimize new liyor. Birisi senden Iproductservice isterse ona productmanager olustur onu ver demektir.
            //Eger sen bir bag�ml�l�k g�r�rsen (productservice) bu tipte (productmanager) onun kars�l�g� budur.
            //Biri constructor da Iproductservice isterse ona arkaplanda productmanager newi ver demek.
            //builder.Services.AddSingleton<IProductService, ProductManager>();
            //builder.Services.AddSingleton<IProductDal, EfProductDal>();
            //yukar�daki.Netcore Ioc container yerine busines katman�ndaki autofac kullanacag�m�z icin kodlar kapat�ld�.

            //Asagida diyoruz ki .Net sen kendi alt yap�n� kullanma IoC olarak autofac i kullan 
            // .NetCore IoC container yerine baska bir container (autofac) kullanmak istersem asa��daki metotla onu tan�tmak gereklidir.
            //autofac kod blogu baslang�c�
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterModule(new AutofacBusinessModule());
            });
            // autofac eklentisi kod blogu sonu

            //A�a��daki blok anlam� bu sistemde JWt kullan�lacak haberin olsun demek 
            var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>();            

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = tokenOptions.Issuer,
                        ValidAudience = tokenOptions.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey)
                    };
                });

            builder.Services.AddDependencyResolvers(new ICoreModule[] { new CoreModule() });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.ConfigureCustomExceptionMiddleware();

            app.UseCors(builder=>builder.WithOrigins("http://localhost:4200").AllowAnyHeader());
            app.UseHttpsRedirection();
            
            //Middleware deniliyor. Asp.Net ya�am d�ng�s�nde hangi yap�lar�n s�ras�yla devreye girece�ini buradan belirtiyoruz. Neye ihtiya� varsa s�ras�yla belirtiyoruz.
            app.UseAuthentication(); //eve girmek i�in anahtar
            app.UseAuthorization(); //evin i�inde neler yapabilir yetkisidir


            app.MapControllers();

            app.Run();
        }
    }
}
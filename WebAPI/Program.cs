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

            //Bana arka planda bir referans oluþtur. IoC bizim yerimize new liyor. Birisi senden Iproductservice isterse ona productmanager olustur onu ver demektir.
            //Eger sen bir bagýmlýlýk görürsen (productservice) bu tipte (productmanager) onun karsýlýgý budur.
            //Biri constructor da Iproductservice isterse ona arkaplanda productmanager newi ver demek.
            //builder.Services.AddSingleton<IProductService, ProductManager>();
            //builder.Services.AddSingleton<IProductDal, EfProductDal>();
            //yukarýdaki.Netcore Ioc container yerine busines katmanýndaki autofac kullanacagýmýz icin kodlar kapatýldý.

            //Asagida diyoruz ki .Net sen kendi alt yapýný kullanma IoC olarak autofac i kullan 
            // .NetCore IoC container yerine baska bir container (autofac) kullanmak istersem asaðýdaki metotla onu tanýtmak gereklidir.
            //autofac kod blogu baslangýcý
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterModule(new AutofacBusinessModule());
            });
            // autofac eklentisi kod blogu sonu

            //Aþaðýdaki blok anlamý bu sistemde JWt kullanýlacak haberin olsun demek 
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
            
            //Middleware deniliyor. Asp.Net yaþam döngüsünde hangi yapýlarýn sýrasýyla devreye gireceðini buradan belirtiyoruz. Neye ihtiyaç varsa sýrasýyla belirtiyoruz.
            app.UseAuthentication(); //eve girmek için anahtar
            app.UseAuthorization(); //evin içinde neler yapabilir yetkisidir


            app.MapControllers();

            app.Run();
        }
    }
}
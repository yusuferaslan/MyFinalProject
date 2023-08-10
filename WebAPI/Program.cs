using Autofac;
using Autofac.Extensions.DependencyInjection;
using Business.Abstract;
using Business.Concrete;
using Business.DependencyResolvers.Autofac;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;

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




            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
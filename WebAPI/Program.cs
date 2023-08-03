using Business.Abstract;
using Business.Concrete;
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


            //Bana arka planda bir referans oluþtur. Ioc bizim yerimize new liyor. Birisi senden Iproductservice isterse ona productmanager olustur onu ver demektir.
            //Eger sen bir bagýmlýlýk görürsen (productservice) bu tipte (productmanager) onun karsýlýgý budur.
            //Biri constructor da Iproductservice isterse ona arkaplanda productmanager newi ver demek.
            builder.Services.AddSingleton<IProductService, ProductManager>();
            builder.Services.AddSingleton<IProductDal, EfProductDal>();



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
using Entities.Concrete;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ValidationRules.FluentValidation
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(p => p.ProductName).NotEmpty();
            RuleFor(p => p.ProductName).MinimumLength(2);
            RuleFor(p => p.UnitPrice).NotEmpty();
            RuleFor(p => p.UnitPrice).GreaterThan(0);
            RuleFor(p => p.UnitPrice).GreaterThanOrEqualTo(10).When(p => p.CategoryId == 1);
            RuleFor(p => p.ProductName).Must(StartWithA).WithMessage("Ürünler A harfi ile başlamalı");

        }
        //FluentValidation tarayıcımızın diline gore farklı dillerde hatamesaji destegi veriyor. Haricinde vermek istersek WithMessage ile verilebilir.


        //Must sart demek, uymalı demek (StartWithA)'da kendi yazacagımız metot. Bu metota uymalı.
        //Asagidaki (string arg),  "RuleFor(p => p.ProductName).Must(StartWithA);" bu satırdaki gonderilen productname demek.
        private bool StartWithA(string arg)
        {
            return arg.StartsWith("A");
           
        }
        //return arg.StartsWith("A"); buradaki arg C# icindeki string fonksiyonu eger A ile basliyorsa true donuyor degilse false
    }
}

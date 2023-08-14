using Castle.DynamicProxy;
using Core.CrossCuttingConcerns.Validation;
using Core.Utilities.Interceptors;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Aspects.Autofac.Validation
{
    public class ValidationAspect : MethodInterception
    {
        private Type _validatorType;
        public ValidationAspect(Type validatorType)
        {
            //defensive coding >> savunma odakli kodlama
            //[ValidationAspect(typeof(ProductValidator))] >> Product managerde kullanirken yazilan kod
            //biz asagidaki if blogunu yazmasakta sistem calisir ama attributelar typeof ile calistigi icin kullanici ustteki ProductValidator yerine kafasina gore herseyi yazabilir (product vs..)
            //instance gondermiyoruz type gonderiyoruz o yuzden hersey yazilabilir
            //bunu kullanacak programci yanlis type atmasin diye burada uyariyoruz
            //gondermeye calistigin validatorType bir IValidator mu kafasina gore bambaska classlar(product,category vs) yollanmasin
            //isassignablefrom >> atanabiliyor mu, yani IValidator mu demek

            if (!typeof(IValidator).IsAssignableFrom(validatorType))
            {
                throw new System.Exception("Bu bir doğrulama sınıfı değil");
            }

            //if blogunda gonderilen bir IValidator mu emin olduktan sonra asagida validatorType esitleniyor.

            _validatorType = validatorType;

        }

        //Asagida de ne yapiyoruz [ValidationAspect(typeof(ProductValidator))] da gonderdigimiz, new (ProductValidator) degil dolayisiyla bellekte bir instance yok
        //Activator.CreateInstance  <<< Reflection kodu. Calisma aninda instance olusturmak istersek bunu kullaniriz
        //Product p = new Product(); bi instance olusturuyor bunun gibi calısma anında instance olusturmak icin Activator.CrateInstance kullanılır.
        
        protected override void OnBefore(IInvocation invocation)
        {
            var validator = (IValidator)Activator.CreateInstance(_validatorType);      // Bu kod ProductValidator demek 
            var entityType = _validatorType.BaseType.GetGenericArguments()[0];         // Prodcutvalidatorun basetypende generic argumanlarin cinsinin tipini yakala demek yani bu bir Prodcut tipi oldu
            var entities = invocation.Arguments.Where(t => t.GetType() == entityType);  // Metodun (IResult Add)'nin argumanlarrini gez entityType ise yani Product turunde ise onlari Validate et
            foreach (var entity in entities)
            {
                ValidationTool.Validate(validator, entity);
            }
        }
    }
}

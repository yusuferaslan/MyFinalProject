using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{   //category ile ilgili dış dünyaya neyi servis etmek istiyorsak o operasyonları yazıyoruz

    public interface ICategoryService
    {
        List<Category> GetAll();
        Category GetById(int categoryId);
    }
}

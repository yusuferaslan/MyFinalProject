using Entities.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstract
{
    //generic constraint
    //where'den sonra yapılan class eklemesi referans tip olmalı demek
    //where'den sonra yapılan IEntity eklemesi IEntity olabilir veya IEntity implemente eden bir nesne olabilir demek
    //where'den sonra yapılan new() eklemesi sadece new'lenebilir olmalı demek


    public interface IEntityRepository<T> where T : class, IEntity, new()
    {
        List<T> GetAll(Expression<Func<T, bool>> filter = null);
        T Get(Expression<Func<T, bool>> filter);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);

    }
}

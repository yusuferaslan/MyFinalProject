using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfUserDal : EfEntityRepositoryBase<User, NorthwindContext>, IUserDal
    {
        public List<OperationClaim> GetClaims(User user)
        {
            using (var context = new NorthwindContext())
            {
                var result = from operationClaim in context.OperationClaims //context'deki OperationClaim'lerle
                             join userOperationClaim in context.UserOperationClaims //context'deki UserOperationClaim'leri birleştir
                                 on operationClaim.Id equals userOperationClaim.OperationClaimId //hangi koşula göre: Id
                             where userOperationClaim.UserId == user.Id //useroperationclaim'deki userId'yi parametre olarak gönderilen user'ın Id'sine göre filtrele
                             select new OperationClaim { Id = operationClaim.Id, Name = operationClaim.Name }; 
                return result.ToList();
                
            }
        }
    }
}

using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.JWT;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IAuthService
    {
        IDataResult<User> Register(UserForRegisterDto userForRegisterDto, string password); //Kullanıcının sisteme kayıt operasyonu
        IDataResult<User> Login(UserForLoginDto userForLoginDto); //Kullanıcının sisteme giriş operasyonu
        IResult UserExists(string email); //Kullanıcı daha önce kayıt olmuş mu operasyonu
        IDataResult<AccessToken> CreateAccessToken(User user); //Kullanıcı için token oluşturma operasyonu
    }
}

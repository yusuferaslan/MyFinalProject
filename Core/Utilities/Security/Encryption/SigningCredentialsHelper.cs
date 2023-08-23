using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Security.Encryption
{
    public class SigningCredentialsHelper
    {
        //WebApi'nin kullanabileceği Json webtokenlarının oluşturulabilmesi için anahtar        
        public static SigningCredentials CreateSigningCredentials(SecurityKey securityKey)
        {
            //Asp.net'e (WebApi'ye) bir tane hashing işlemi yapacaksın anahtar olarak securityKey'i kullan şifreleme olarakta güvenlik algoritmalarından HmacSha512 yi kullan diyoruz.
            return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

        }
    }
}

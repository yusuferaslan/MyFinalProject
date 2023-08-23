using Core.Entities.Concrete;
using Core.Extensions;
using Core.Utilities.Security.Encryption;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Security.JWT
{
    public class JwtHelper : ITokenHelper
    {
        public IConfiguration Configuration { get; }  //WebApi altında appsettings.json dosyasındaki değerleri okumayı sağlıyor.
        private TokenOptions _tokenOptions;  //apsettings.json'da okuduğumuz değerleri bi nesneye(TokenOptions) atıyoruz.
        private DateTime _accessTokenExpiration; //accesToken ne zaman geçersiz hale gelecekten sorumlu
        public JwtHelper(IConfiguration configuration) //.netCore, biz IConfigurationu enjekte edince bizim için apinin appsetting.json nesnesini enjekte ediyor.
        {
            Configuration = configuration;
            _tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>(); //apsettings.json'da bulunan TokenOptions bölümünü alır TokenOptions sınıfının değerleriyle mappler, eşleştir.

        }
        public AccessToken CreateToken(User user, List<OperationClaim> operationClaims) //User ve Claimlerin bilgisini ver ben ona göre bir token oluşturayım.
        {
            _accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration); //token ne zaman geçersiz olacak. DateTime.Now.AddMinutes >> Şimdiye dakika ekle accestokenexp içindeki (10) değeri.
            var securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey); //bunu oluştururken bir güvenlik anahtarına(securityKey) ihtiyaç var. _tokenOptions.SecurityKey'deki kodu kullanarak, SecurityKeyHelper ile oluştur.
            var signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey); //hangi algoritmayı kullanayım ve anahtar nedir. SigningCredentialsHelper içinde yazdım oradan kullan.
            var jwt = CreateJwtSecurityToken(_tokenOptions, user, signingCredentials, operationClaims); //_tokenOptions'ları kullanarak ilgili kullanıcı(user) için ilgili credentialları kullanarak bu kişiye atanacak claimleri(kimlik kartı,yetkileri) içeren metot.
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler(); 
            var token = jwtSecurityTokenHandler.WriteToken(jwt); //token nesnesini writetoken ile stringe çeviriyoruz

            return new AccessToken
            {
                Token = token,
                Expiration = _accessTokenExpiration
            };

        }

        public JwtSecurityToken CreateJwtSecurityToken(TokenOptions tokenOptions, User user,
            SigningCredentials signingCredentials, List<OperationClaim> operationClaims)
        {
            var jwt = new JwtSecurityToken(
                issuer: tokenOptions.Issuer,
                audience: tokenOptions.Audience,
                expires: _accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: SetClaims(user, operationClaims),
                signingCredentials: signingCredentials
            );
            return jwt;
        }

        private IEnumerable<Claim> SetClaims(User user, List<OperationClaim> operationClaims)
        {
            var claims = new List<Claim>();    //Claim koleysiyonu sınırlıydı ve başka bi clasta extension metot yazarak bize gerekli olan email,name,roles parametrelerini ClaimExtension içine yazarak ekledik.
            claims.AddNameIdentifier(user.Id.ToString()); //Parametreyle gelen user'in Id'sini string olarak ekle.
            claims.AddEmail(user.Email);
            claims.AddName($"{user.FirstName} {user.LastName}");
            claims.AddRoles(operationClaims.Select(c => c.Name).ToArray()); //Select(c => c.Name)>> Claim'in name'ni getir.Gelen ICollectiondur onu array a çevir(ToArray).

            return claims;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions
{
    public static class ClaimExtensions  //Claim clasını extension metodu ile genişletiyoruz.
    {
        public static void AddEmail(this ICollection<Claim> claims, string email) //Bu ICollection türünde bir claims extend edecek, neyi extend edecek string email paratemresini
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, email)); //Extend edilecek operasyona yeni bir claim ekliyoruz. System.IdentityModel.Tokens içindeki kayıtlı hazır isimlerden(email) kullanıyoruz.
        }

        public static void AddName(this ICollection<Claim> claims, string name)
        {
            claims.Add(new Claim(ClaimTypes.Name, name));
        }

        public static void AddNameIdentifier(this ICollection<Claim> claims, string nameIdentifier)
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, nameIdentifier));
        }

        public static void AddRoles(this ICollection<Claim> claims, string[] roles) 
        {
            roles.ToList().ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role))); //roles parametresini listeye çevir(ToList), her birini tek tek dolaş(ForEach), her bir role git ve claime ekle. (bu şekilde de ForEach yazılabilir)
        }

        //İsteğe göre başka alanlar da eklenebilir.
    }
}

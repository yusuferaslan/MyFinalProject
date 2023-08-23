using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Security.Hashing
{
    public class HashingHelper
    {
        //Kullanıcının sisteme kaydolurken verdiği password
        //Asağıdaki metot verilen bir password değerinin salt ve hash değerini oluşturmaya yarıyor
        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) //out keywordu (passwordHash,passwordSalt) parametresi gönderilince içeride değişen nesneyi byte[] aktaracak.
        {
            //disposable pattern
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key; //Salt için hmacin key'ini kullanıyoruz. Burada başka birşeyde kullanılabilir. Tabiki bu bizim için standart olması beklenir çünkü şifreyi çözerken Salt değerine ihtiyaç olur. ilgili kullanılan algoritmanın(HMACSHA512) o an oluştudugu key değeridir. her kullanıcı için bir key oluşturur ve çok da güvenlidir.istersek değiştirilebilir.
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)); //Encoding.UTF8.GetBytes()>> bir stringin byte karşılığını alır.
            }
        }

        //Kullanıcının sisteme tekrar girerken verdiği parola (password)
        //Kullanıcının verdiği gönderdiği Password Hashini doğrulama metodu
        //Kullanıcının yeniden gönderdiği passwordu yine yukarıdaki algoritmayı kullanarak hashleseydin karşına böyle birşey çıkarmıydı kontrolü.
        //Veri tabanındaki hash ile kullanıcının yeniden girişte gönderdiği passwordun oluşacak hashini karşılaşıtırıyoruz.
        //Eğer iki hash değeri birbiriyle eşitse true, değilse false
        //Kullanıcının ilk girerken belirttiği passwordun sonradan girerken verdiği password ile, ilgili Salta göre eşleşip eşleşmediğini denetlediğimiz yer
        public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++) //hesaplanan hashin bütün değerlerini tek tek dolaş
                {
                    if (computedHash[i] != passwordHash[i]) //computedHashin [i] değeri veritabanından gönderilen passwordHashin [i] değeriyle karşılaştırması
                    {
                        return false; // eşitdeğilse != false >> (buraya hiç girmezse zaten hepsi aynı demektir aşağıda return true der operasyonu bitirir.)
                    }

                }
                return true; //eşleşirse true

            }

        }
    }
}

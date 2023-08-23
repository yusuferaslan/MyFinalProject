using Business.Abstract;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public ActionResult Login(UserForLoginDto userForLoginDto)
        {
            var userToLogin = _authService.Login(userForLoginDto); //_authService >> önce maili kontrol ediyor mail kayıtlıysa gönderilen şifreyi kontrol ediyor, hatalıysa şifre hatası veriyor.ikisinde de problem yoksa login bilgisi gönderiyor.
            if (!userToLogin.Success) //işlem sonucu başarılı ! değilse
            {
                return BadRequest(userToLogin.Message); // işlemin sonucunda ki mesajı döndür
            }

            var result = _authService.CreateAccessToken(userToLogin.Data); //Access token üretme işlemi için yukarıdaki kullanıcı datasını kullan
            if (result.Success)
            {
                return Ok(result.Data); 
            }

            return BadRequest(result.Message); 
        }

        [HttpPost("register")]
        public ActionResult Register(UserForRegisterDto userForRegisterDto) //kayıt olma durumu
        {
            var userExists = _authService.UserExists(userForRegisterDto.Email); //_authService >> kullanıcı kayıtlı mı diye kontrol ediyor.
            if (!userExists.Success) // >> kullanıcı(mail) mevcut değil olarak ! değilse (kullancı mevcutsa)
            {
                return BadRequest(userExists.Message); //hata mesajı verir
            }

            //kullanıcı yoksa işleme devam eder
            var registerResult = _authService.Register(userForRegisterDto, userForRegisterDto.Password); //kullanıcıyı kayıt eder
            var result = _authService.CreateAccessToken(registerResult.Data); //token oluşturur
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Message);
        }
    }
}

using AspNetCore.Entities;
using AspNetCore.Entities.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AspNetCoreMVCWebAPIUsing.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiAdres;
        private static readonly string Username = "test";
        private static readonly string Password = "test@123";
        public AccountController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiAdres = "http://localhost:5194/Api/Auth/";
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("userToken") is not null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("userToken"));
                var model = await _httpClient.GetFromJsonAsync<User>(_apiAdres + "GetUserByUserGuid/" + HttpContext.Session.GetString("refreshToken"));
                return View(model);
            }
            else
            {
                return NotFound();
            }            
        }
        public IActionResult JsLogin()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginAsync(UserLoginModel userLoginModel)
        {
            try
            {
                //_httpClient.DefaultRequestHeaders.Authorization.Parameter.Insert(0,"test");
                var response = await _httpClient.PostAsJsonAsync(_apiAdres + "Login", userLoginModel);
                if (response.IsSuccessStatusCode)
                {
                    Token jwt = await response.Content.ReadFromJsonAsync<Token>();
                    if (jwt is not null)
                    {
                        HttpContext.Session.SetString("userToken", jwt.AccessToken);
                        HttpContext.Session.SetString("refreshToken", jwt.RefreshToken);

                        var haklar = new List<Claim>() // Claim = Hak
                    {
                        new Claim(ClaimTypes.UserData, jwt.RefreshToken) // kullanıcıya hak tanımladık
                    };
                        //var kullaniciKimligi = new ClaimsIdentity(haklar, "Login");
                        var kullaniciKimligi = new ClaimsIdentity(haklar, ClaimsIdentity.DefaultNameClaimType);
                        var claimsPrincipal = new ClaimsPrincipal(kullaniciKimligi);
                        await HttpContext.SignInAsync(claimsPrincipal);
                    }
                    else
                        HttpContext.Session.SetString("userToken", "jwt.AccessToken is null");
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Giriş Başarısız!");
            }
            catch
            {
                ModelState.AddModelError("", "Hata Oluştu!");
            }
            return View();
        }
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(User appUser)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //_httpClient.DefaultRequestHeaders.Authorization.Parameter.Insert(0,"test");
                    appUser.CreateDate = DateTime.Now;
                    var response = await _httpClient.PostAsJsonAsync(_apiAdres + "CreateAppUser", appUser);
                    if (response.IsSuccessStatusCode)
                        return RedirectToAction(nameof(Index));
                    ModelState.AddModelError("", "Kayıt Başarısız!");
                }
                catch
                {
                    ModelState.AddModelError("", "Hata Oluştu!");
                }
            }
            return View(appUser);
        }
    }
}

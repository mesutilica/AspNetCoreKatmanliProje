using AspNetCore.Entities;
using AspNetCoreMVCWebAPIUsing.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace AspNetCoreMVCWebAPIUsing.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize]
    public class BrandsController : Controller
    {
        static string _apiAdres = "http://localhost:5194/Api/Brands/";
        HttpClient _httpClient = new HttpClient(); // .net framework deki yapıyı kullanarak
        // GET: Admin/Brands
        public async Task<ActionResult> Index()
        {
            if (HttpContext.Session.GetString("userToken") is not null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("userToken"));
                //var model = await _httpClient.GetFromJsonAsync<User>(_apiAdres + "GetUserByUserGuid/" + HttpContext.Session.GetString("refreshToken"));
                //return View(model);
            }
            var response = await _httpClient.GetAsync(_apiAdres);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync(); //JSON verisini oku
                var model = JsonConvert.DeserializeObject<List<Brand>>(data); //JSON verisini Post listesine dönüştür
                return View(model);
            }
            return NotFound(response.StatusCode.ToString());
        }

        // GET: Admin/Brands/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/Brands/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Brands/Create
        [HttpPost]
        public async Task<ActionResult> Create(Brand collection, IFormFile? Logo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    collection.Logo = await FileHelper.FileLoaderAsync(Logo);
                    var json = JsonConvert.SerializeObject(collection);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync(_apiAdres, data);
                    if (response.IsSuccessStatusCode)
                        return RedirectToAction("Index");
                }
                catch
                {
                    ModelState.AddModelError("", "Hata Oluştu!");
                }
            }
            return View(collection);
        }

        // GET: Admin/Brands/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync(_apiAdres + id);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync(); //JSON verisini oku
                var model = JsonConvert.DeserializeObject<Brand>(data); //JSON verisini Post listesine dönüştür
                return View(model);
            }
            return NotFound();
        }

        // POST: Admin/Brands/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, Brand collection, IFormFile? Logo, bool resmiSil = false)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (resmiSil == true)
                        collection.Logo = string.Empty;
                    if (Logo != null)
                        collection.Logo = await FileHelper.FileLoaderAsync(Logo);
                    var json = JsonConvert.SerializeObject(collection);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PutAsync(_apiAdres + id, data);
                    if (response.IsSuccessStatusCode)
                        return RedirectToAction("Index");
                }
                catch
                {
                    ModelState.AddModelError("", "Hata Oluştu!");
                }
            }
            return View(collection);
        }

        // GET: Admin/Brands/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var response = await _httpClient.GetAsync(_apiAdres + id);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync(); //JSON verisini oku
                var model = JsonConvert.DeserializeObject<Brand>(data); //JSON verisini Post listesine dönüştür
                return View(model);
            }
            return NotFound();
        }

        // POST: Admin/Brands/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int id, Brand collection)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(_apiAdres + id);
                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError("", "Hata Oluştu!");
            }
            return View(collection);
        }
    }
}

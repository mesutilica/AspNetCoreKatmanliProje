﻿using AspNetCore.Entities;
using AspNetCoreMVCWebAPIUsing.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreMVCWebAPIUsing.Controllers
{
    public class ProductsController : Controller
    {
        private readonly HttpClient _httpClient;

        public ProductsController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private readonly string _apiAdres = "http://localhost:5194/api/Products";
        [Route("tum-urunlerimiz")]
        public async Task<IActionResult> Index()
        {
            var model = await _httpClient.GetFromJsonAsync<List<Product>>(_apiAdres);
            return View(model);
        }
        public async Task<IActionResult> Search(string q)
        {
            // Yöntem 1 
            //var products = await _httpClient.GetFromJsonAsync<List<Product>>(_apiAdres);
            //var model = products.Where(p => p.IsActive && p.Name.ToLower().Contains(q.ToLower()));
            // Yöntem 2
            if (q is null)
            {
                return BadRequest();
            }
            var model2 = await _httpClient.GetFromJsonAsync<List<Product>>(_apiAdres + "/GetSearch/" + q);
            return View(model2);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var model = new ProductDetailViewModel();
            var products = await _httpClient.GetFromJsonAsync<List<Product>>(_apiAdres);
            var product = await _httpClient.GetFromJsonAsync<Product>(_apiAdres + "/" + id);
            model.Product = product;
            model.RelatedProducts = products.Where(p => p.CategoryId == product.CategoryId && p.Id != id).ToList();
            if (model is null)
            {
                return NotFound();
            }
            return View(model);
        }
        public IActionResult FetchCrud()
        {
            return View();
        }
        public IActionResult ProductDetail(int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }
            return View();
        }
        public IActionResult JqueryCrud()
        {
            return View();
        }
    }
}

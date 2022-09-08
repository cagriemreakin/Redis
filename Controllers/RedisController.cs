using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using RedisPoc.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RedisPoc.Controllers
{
    public class RedisController : Controller
    {
        private IDistributedCache _distributeCache;
        public RedisController(IDistributedCache distributedCache)
        {
            _distributeCache = distributedCache;
        }


        // GET: /<controller>/
        public  IActionResult Index()
        {
            
            return View();
        }

        [HttpPost]
        [Route("Add")]

        public async Task<IActionResult> Add()
        {
            DistributedCacheEntryOptions distributedCache = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(2),
                SlidingExpiration = TimeSpan.FromSeconds(1)
            };

            //Basic Data Type
            await _distributeCache.SetStringAsync("product", "robot cleaner", distributedCache);
            await _distributeCache.SetStringAsync("producer", "roidme");


            // Complex Types

            // store json as string
            Product product = new() {
                  Id = 1,
                  Name = "Computer"
            };

            string jsonObj = JsonConvert.SerializeObject(product);
            await _distributeCache.SetStringAsync("product:1", jsonObj);

            //store complext type as byte
            Product product2= new()
            {
                Id = 2,
                Name = "Keyboard"
            };

            string jsonObj2 = JsonConvert.SerializeObject(product2);

            Byte[] byteProduct = Encoding.UTF8.GetBytes(jsonObj2);

            await _distributeCache.SetAsync("product:2", byteProduct);

            return View();
        }

        [HttpPost]
        [Route("Show")]
        public IActionResult Show()
        {
            ViewBag.product = _distributeCache.GetString("product");
            ViewBag.producer = _distributeCache.GetString("producer");

            ViewBag.product1 = JsonConvert.DeserializeObject<Product>( _distributeCache.GetString("product:1"));

            Byte[] bytes = _distributeCache.Get("product:2");

            string jsonProduct2 = Encoding.UTF8.GetString(bytes);
            ViewBag.product2 = JsonConvert.DeserializeObject<Product>(jsonProduct2);

            return View();
        }
        [HttpPost]
        [Route("Delete")]

        public IActionResult Delete()
        {
            _distributeCache.Remove("product");
            _distributeCache.Remove("producer");
            _distributeCache.Remove("product:1");
            _distributeCache.Remove("product:2");
            return View();
        }

        [HttpPost]
        [Route("CacheImage")]

        public IActionResult CacheImage()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/images/home.png");

            byte[] img= System.IO.File.ReadAllBytes(path);
            _distributeCache.Set("image", img);
            return View();
        }


        public IActionResult ImageUrl()
        {
            var imgByte = _distributeCache.Get("image");

            return File(imgByte, "image/png");
          
        }
    }
}


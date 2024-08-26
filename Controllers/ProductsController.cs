using Microsoft.AspNetCore.Mvc;
using SamStoreMVC.Models;
using SamStoreMVC.Services;

namespace SamStoreMVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext context;

        public IWebHostEnvironment Environment { get; }

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment Environment)
        {
            this.context = context;
            this.Environment = Environment;
        }
        public IActionResult Index()
        {
            var products = context.Products.OrderByDescending(p => p.Id).ToList();

            return View(products);
        }
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {
            if (productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "File is Required!");
            }
            if (!ModelState.IsValid)
            {
                return View(productDto);
            }
            // save the image file
            string newfileName = DateTime.Now.ToString("MMMddyyyyHHmmssffff");
            newfileName += Path.GetExtension(productDto.ImageFile!.FileName);

            string imageFullPath = Environment.WebRootPath + "/products/" + newfileName;

            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            // save the new product 
            Products product = new Products()
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Category = productDto.Category,
                Price = productDto.Price,
                Description = productDto.Description,
                ImageFileName = newfileName,
                CreatedAt = DateTime.Now
            };
            context.Products.Add(product);
            context.SaveChanges();
            return RedirectToAction("Index", "Products");
        }
        public IActionResult Edit(int ID)
        {

            var product = context.Products.Find(ID);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }
            // create product form ProdcutDto

            var productDto = new ProductDto()
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description

            };
            ViewData["ID"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreatedAt;

            return View(productDto);
        }
        public IActionResult Delete() { return View(); }

        [HttpPost]
        public IActionResult Edit(int ID, ProductDto productDto)
        {
            var product = context.Products.Find(ID);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }
            if (!ModelState.IsValid)
            {
                ViewData["ID"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedAt"] = product.CreatedAt;
                return View(productDto);
            }

            // update the image file if we have new file

            string newFileName = DateTime.Now.ToString("MMMddyyyyHHmmssffff");

            if (product.ImageFileName != null)
            {
                newFileName += Path.GetExtension(productDto.ImageFile!.FileName);

                string imageFullPath = Environment.WebRootPath + "/products/" + newFileName;


                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productDto.ImageFile.CopyTo(stream);
                }
                // delete the old image

                string oldImageFilePath = Environment.WebRootPath + "/products/" + newFileName;
                System.IO.File.Delete(oldImageFilePath);
            }

            // update the product in the database

            product.Name = productDto.Name;
            product.Brand = productDto.Brand;
            product.Category = productDto.Category;
            product.Description = productDto.Description;
            product.ImageFileName = newFileName;
            product.Price = productDto.Price;

            context.SaveChanges();
            return RedirectToAction("Index", "Products");
        }
    }
}

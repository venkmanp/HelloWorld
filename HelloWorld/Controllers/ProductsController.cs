using AutoMapper;
using HelloWorld.Entities;
using HelloWorld.Models;
using HelloWorld.Repositories;
using HelloWorld.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HelloWorld.Controllers
{
    [Route("api/categories/{categoryID}/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private ILogger<ProductsController> _logger;
        private IMailService _mailService;
        private readonly IProductRepository _repo;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public ProductsController(ILogger<ProductsController> logger, IMailService mailService, IProductRepository productRepository, ICategoryRepository categoryRepository,IMapper mapper) 
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService;
            _repo = productRepository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllProductsAsync() 
        {
            IEnumerable<Product> products = await _repo.GetProductsAsync();

            //Use automapper to convert List<product> to List<ProductDTO>
            List<ProductDTO> result = _mapper.Map<List<ProductDTO>>(products);

            return Ok(result);
        }


        [HttpGet]
        public async Task<ActionResult<List<ProductDTO>>> GetProducts(int categoryID)
        {
            if (!await _categoryRepository.CategoryExistsAsync(categoryID))
            {
                _logger.LogWarning("Category not found");
                return NotFound("Category not found");
            }

            IEnumerable<Product> products = await _repo.GetProductsForCategoryAsync(categoryID);

            return Ok(_mapper.Map<List<ProductDTO>>(products));

                       
            
            //if (!CategoryExists(categoryID, out CategoryDTO category ))
            //{
            //    _logger.LogWarning($"Someone was looking for category with id: {categoryID}");
            //    return NotFound();
            //}
            
            //return Ok(category.Products);
        }


        [HttpGet("{productID}", Name = "GetProduct")]
        public ActionResult<ProductDTO> GetProduct(int categoryID, int productID)
        {
            if (!CategoryExists(categoryID, out CategoryDTO category))
            {
                _logger.LogWarning($"Someone was looking for category with id: {categoryID}");
                return NotFound();
            }
            ProductDTO product = category.Products.FirstOrDefault(p => p.ID == productID);

            if (product == null)
            {
                _logger.LogWarning($"Someone was looking for product with id: {productID}");
                return NotFound();
            }
            return Ok(product);
        }

        private bool CategoryExists(int categoryID, out CategoryDTO category)
        {
            category = MyDataStore.Categories.FirstOrDefault(c => c.ID == categoryID);
            
            return category != null;
        }

        [HttpPost]
        public ActionResult CreateProduct(int categoryID, ProductForCreationDTO product)
        {
            if (!CategoryExists(categoryID, out CategoryDTO category))
            {
                return NotFound();
            }

            int maxID = MyDataStore.Categories.SelectMany(c => c.Products).Max(p => p.ID);

            ProductDTO newProduct = new ProductDTO
            {
                ID = ++maxID,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price
            };
              
            category.Products.Add(newProduct);

            return CreatedAtRoute("", new {
                categoryID,
                productID = newProduct.ID
            }, newProduct);
        }

        [HttpPut("{productID}")]
        public ActionResult UpdateProduct(int categoryID, int productID, ProductForUpdateDTO product)
        {
            if (!CategoryExists(categoryID, out CategoryDTO category))
            {
                return NotFound();
            }

            ProductDTO productFromStore = category.Products.FirstOrDefault(p=>p.ID == productID);

            if (productFromStore == null) {
                return NotFound();
            }

            productFromStore.Name = product.Name;
            productFromStore.Description = product.Description;
            productFromStore.Price = product.Price;

            return NoContent();

        }

        [HttpDelete("{productID}")]
        public ActionResult DeleteProduct (int categoryID, int productID)
        {
            if (!CategoryExists(categoryID, out CategoryDTO category))
            {
                return NotFound();
            }

            ProductDTO productFromStore = category.Products.FirstOrDefault(p => p.ID == productID);

            if (productFromStore == null)
            {
                return NotFound();
            }

            category.Products.Remove(productFromStore);

            _mailService.Send("Product deleted", $"a user deleted the products {productFromStore.Name}");

            return NoContent();
        }
    }


}

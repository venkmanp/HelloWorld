using AutoMapper;
using HelloWorld.Entities;
using HelloWorld.Models;
using HelloWorld.Repositories;
using HelloWorld.Services;
using Microsoft.AspNetCore.Mvc;

namespace HelloWorld.Controllers
{
    [ApiController]
    [Route("api/categories")]
    
    public class CategoriesController : ControllerBase
    {
        private ILogger<CategoriesController> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoriesController(ILogger<CategoriesController> logger, ICategoryRepository categoryRepository, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }



        [HttpGet]
        public async Task<ActionResult<List<CategoryDTO>>> GetCategories(string? name, string? searchQuery)
        {
            IEnumerable<Category> categories;
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(searchQuery))
            {
                categories = await _categoryRepository.GetCategoriesAsync();
                
            } else
            {
                categories = await _categoryRepository.GetCategoriesAsync(name, searchQuery);
            }

            return Ok(_mapper.Map<List<CategoryDTO>>(categories));

        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id, bool includeProducts)
        {
            Category? category = await _categoryRepository.GetCategoryAsync(id, includeProducts);

            if (category == null)
            {
                return NotFound();
            }

            if (includeProducts)
            {
                return Ok(_mapper.Map<CategoryDTO>(category));
               
            }
            return Ok(_mapper.Map<CategoryWithNoProductsDTO>(category));
            //TODO: return a DTO with no products


            //CategoryDTO category = MyDataStore.Categories.FirstOrDefault(c => c.ID == id);
            //if (category == null)
            //{
            //    return NotFound();
            //}
            //return Ok(category);
        }
    }
}

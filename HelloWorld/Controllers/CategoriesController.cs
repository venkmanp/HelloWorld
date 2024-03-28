using AutoMapper;
using HelloWorld.Entities;
using HelloWorld.Models;
using HelloWorld.Repositories;
using HelloWorld.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelloWorld.Controllers
{
    [ApiController]
    [Route("api/categories")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private ILogger<CategoriesController> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        const int maxPageSize = 10;

        public CategoriesController(ILogger<CategoriesController> logger, ICategoryRepository categoryRepository, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }


        //This example uses paging to return only the 10 results... or number specified of results per page
        [HttpGet]
        [Authorize(Policy = "IsAdmin")]
        public async Task<ActionResult<List<CategoryDTO>>> GetCategories(string? name, string? searchQuery, int pageNumber=1, int pageSize = 10)
        {
            //Don't allow more than 10 results per page:
            if (pageSize > maxPageSize)
            {
                pageSize = maxPageSize;
            }
            
            /*
            IEnumerable<Category> categories;
            
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(searchQuery))
            {
                categories = await _categoryRepository.GetCategoriesAsync();
                
            } else
            {
                categories = await _categoryRepository.GetCategoriesAsync(name, searchQuery, pageNumber, pageSize);
            }
            */

            var (categories, meta) = await _categoryRepository.GetCategoriesAsync(name, searchQuery, pageNumber, pageSize);

            //We should return the paging metadata in the headers of the response, and not in the body:
            Response.Headers.Append("X-PageNumber", meta.PageNumber.ToString());
            Response.Headers.Append("X-PageSize", meta.PageSize.ToString());
            Response.Headers.Append("X-TotalItemCount", meta.TotalItemCount.ToString());

            return Ok(_mapper.Map<List<CategoryDTO>>(categories));

        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id, bool includeProducts)
        {
            //example on for limiting a user only to category ID that is in the user's JWT claims:
            if (User.Claims.FirstOrDefault(c=>c.Type == "allowed_category")?.Value != id.ToString())
            {
                return Forbid();
            }

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

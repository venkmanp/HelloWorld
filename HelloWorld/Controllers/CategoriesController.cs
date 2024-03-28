using Asp.Versioning;
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
    [Route("api/v{version:ApiVersion}/categories")]
    [ApiVersion(1)]
    [ApiVersion(2)]
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


        
        
        /// <summary>
        /// Get all the categories
        /// </summary>
        /// <param name="name">filter the results by name of category</param>
        /// <param name="searchQuery">search the results for a phrase</param>
        /// <param name="pageNumber">the page number to return (default 1)</param>
        /// <param name="pageSize">the size of the page to return (default 10)</param>
        /// <returns>the resulting list of categories by page</returns>
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


        /// <summary>
        /// Get a single category by ID
        /// </summary>
        /// <param name="id">the id of the category to return</param>
        /// <param name="includeProducts">a flag that indicates if we want the products returned in category object</param>
        /// <returns>A category</returns>
        /// <response code="200">A valid category with the given ID</response>
        /// <response code="403">User is forbidden, does not have allowed_category claim</response>
        /// <response code="404">No category with given ID was found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

using HelloWorld.Models;
using Microsoft.AspNetCore.Mvc;

namespace HelloWorld.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<CategoryDTO>> GetCategories()
        {
            return Ok(new List<CategoryDTO>
            {
                new CategoryDTO { ID=1, Name="Books" },
                new CategoryDTO { ID=2, Name="Shoes" }
            });
        }


        [HttpGet("{id}")]
        public ActionResult<CategoryDTO> GetCategory(int id)
        {
            CategoryDTO category = MyDataStore.Categories.FirstOrDefault(c => c.ID == id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }
    }
}

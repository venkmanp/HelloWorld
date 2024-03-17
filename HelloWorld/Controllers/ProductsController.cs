using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HelloWorld.Controllers
{
    [Route("api/categories/[categoryID]/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

    }
}

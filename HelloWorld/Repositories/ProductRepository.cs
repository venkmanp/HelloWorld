using HelloWorld.Contexts;
using HelloWorld.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HelloWorld.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly MainContext _context;

        public ProductRepository(MainContext context) 
        {
            _context = context;
        }
        
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }
    }
}


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

        public async Task AddProductAsync(Product product, bool autosave = true)
        {
            await _context.Products.AddAsync(product);
            if (autosave)
            {
                await _context.SaveChangesAsync(); //Save here to DB? maybe we should save after we add a lot of products...
            }
        }

        public Task DeleteProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsForCategoryAsync(int categoryID)
        {
            return await _context.Products.Where(p => p.CategoryID == categoryID).ToListAsync();
        }

        public async Task<Product?> GetProductForCategoryAsync(int categoryID, int productID)
        {
            return await _context.Products
                .Where(p => p.CategoryID == categoryID && p.ID == productID)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }
    }
}


using HelloWorld.Contexts;
using HelloWorld.Entities;
using HelloWorld.Models;
using Microsoft.EntityFrameworkCore;

namespace HelloWorld.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly MainContext _context;
        
        public CategoryRepository(MainContext context)
        {
            _context = context;
        }

        public Task<bool> CategoryExistsAsync(int categoryID)
        {
            return _context.Categories.AnyAsync(c => c.ID == categoryID);
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<(IEnumerable<Category>, PagingMetaData)> GetCategoriesAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
        {
            IQueryable<Category> categories = _context.Categories as IQueryable<Category>; //Note that we are using an IQueryable and not a IEnumerable.

            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                categories = categories.Where(c => c.Name == name);
            }
            
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                //categories = categories.Where(c => c.Name.Contains(searchQuery, StringComparison.InvariantCultureIgnoreCase)); not supported in SQLite
                categories = categories.Where(c=>c.Name.Contains(searchQuery));
            }

            int totalCount = await categories.CountAsync(); //To return meta-data of how many total results there are.

            PagingMetaData meta = new PagingMetaData
            {
                TotalItemCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            //Limit the query to return only the page requested; This fetches the correct set of results for the specified page from the DB:
            categories = categories.OrderBy(c => c.ID).Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            
            return (await categories.ToListAsync(), meta);
        }

        public async Task<Category?> GetCategoryAsync(int id, bool includeProducts)
        {
            if (includeProducts)
            {
                return await _context.Categories
                    .Include(c=>c.Products)
                    .Where(c=>c.ID == id).FirstOrDefaultAsync();
            }

            return await _context.Categories
                .Where(c => c.ID == id).FirstOrDefaultAsync();
        }
    }
}


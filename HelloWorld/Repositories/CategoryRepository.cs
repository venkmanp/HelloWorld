﻿using HelloWorld.Contexts;
using HelloWorld.Entities;
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

        public Task<IEnumerable<Category>> GetCategoriesAsync(string? name)
        {
            IQueryable<Category> categories = _context.Categories as IQueryable<Category>; //Note that we are using an IQueryable and not a IEnumerable.

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


using Microsoft.EntityFrameworkCore;
using MobileShopAPI.Data;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;

namespace MobileShopAPI.Repositories
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly AppDbContext _context;

        public ProductImageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductImage?> GetByIdAsync(int id)
        {
            return await _context.ProductImages
                .Include(pi => pi.ProductImageAssignments)
                .FirstOrDefaultAsync(pi => pi.Id == id);
        }

        public async Task<List<ProductImage>> GetAllAsync()
        {
            return await _context.ProductImages
                .Include(pi => pi.ProductImageAssignments)
                .ToListAsync();
        }

        public async Task AddAsync(ProductImage productImage)
        {
            await _context.ProductImages.AddAsync(productImage);
        }

        public async Task UpdateAsync(ProductImage productImage)
        {
            _context.ProductImages.Update(productImage);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(ProductImage productImage)
        {
            _context.ProductImages.Remove(productImage);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task AssignImageToProductAsync(int productId, int productImageId, bool isDefault = false)
        {
            var assignment = new ProductImageAssignment
            {
                ProductId = productId,
                ProductImageId = productImageId,
                IsDefault = isDefault
            };

            await _context.ProductImageAssignments.AddAsync(assignment);
        }

        public async Task RemoveImageFromProductAsync(int productId, int productImageId)
        {
            var assignment = await _context.ProductImageAssignments
                .FirstOrDefaultAsync(pia => pia.ProductId == productId && pia.ProductImageId == productImageId);

            if (assignment != null)
            {
                _context.ProductImageAssignments.Remove(assignment);
            }
        }

        public async Task<List<ProductImage>> GetImagesByProductAsync(int productId)
        {
            return await _context.ProductImageAssignments
                .Where(pia => pia.ProductId == productId)
                .Include(pia => pia.ProductImage)
                .Select(pia => pia.ProductImage)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsByImageAsync(int productImageId)
        {
            return await _context.ProductImageAssignments
                .Where(pia => pia.ProductImageId == productImageId)
                .Include(pia => pia.Product)
                .Select(pia => pia.Product)
                .ToListAsync();
        }

        public async Task SetDefaultImageAsync(int productId, int productImageId)
        {
            // Reset all images to non-default for this product
            var assignments = await _context.ProductImageAssignments
                .Where(pia => pia.ProductId == productId)
                .ToListAsync();

            foreach (var assignment in assignments)
            {
                assignment.IsDefault = assignment.ProductImageId == productImageId;
            }
        }
    }
}
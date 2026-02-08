// Repositories/CashOnDeliveryRepository.cs
using Microsoft.EntityFrameworkCore;
using MobileShopAPI.Data;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;

namespace MobileShopAPI.Repositories
{
    public class CashOnDeliveryRepository : ICashOnDeliveryRepository
    {
        private readonly AppDbContext _context;

        public CashOnDeliveryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CashOnDelivery?> GetByIdAsync(int id)
        {
            return await _context.CashOnDeliveries.FindAsync(id);
        }

        public async Task<CashOnDelivery?> GetByTransactionIdAsync(int transactionId)
        {
            return await _context.CashOnDeliveries
                .FirstOrDefaultAsync(cod => cod.TransactionId == transactionId);
        }

        public async Task<CashOnDelivery> CreateAsync(CashOnDelivery cashOnDelivery)
        {
            _context.CashOnDeliveries.Add(cashOnDelivery);
            await _context.SaveChangesAsync();
            return cashOnDelivery;
        }

        public async Task UpdateAsync(CashOnDelivery cashOnDelivery)
        {
            _context.Entry(cashOnDelivery).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var cashOnDelivery = await _context.CashOnDeliveries.FindAsync(id);
            if (cashOnDelivery != null)
            {
                _context.CashOnDeliveries.Remove(cashOnDelivery);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.CashOnDeliveries.AnyAsync(cod => cod.Id == id);
        }
    }
}
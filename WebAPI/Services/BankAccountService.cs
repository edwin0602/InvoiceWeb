using Microsoft.EntityFrameworkCore;
using WebAPI.Common.Constants;
using WebAPI.Data;
using WebAPI.Models;

namespace WebAPI.Services
{
    public interface IBankAccountService
    {
        Task<IEnumerable<BankAccount>> GetBankAccountsAsync();
        Task<BankAccount> GetBankAccountByIdAsync(Guid id);
        Task AddBankAccountAsync(BankAccount vat);
        Task UpdateBankAccountAsync(BankAccount vat);
        Task MarkAsDeleteBankAccountAsync(Guid id);
    }

    public class BankAccountService : IBankAccountService
    {
        private readonly InvoicikaDbContext _context;

        public BankAccountService(InvoicikaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BankAccount>> GetBankAccountsAsync()
        {
            return await _context.BankAccounts.ToListAsync();
        }

        public async Task<BankAccount> GetBankAccountByIdAsync(Guid id)
        {
            return await _context.BankAccounts.FirstOrDefaultAsync(b => b.BankAccountId == id)
                ?? throw new KeyNotFoundException("BankAccount not found");
        }

        public async Task AddBankAccountAsync(BankAccount bankAccount)
        {
            bankAccount.Status = GeneralStatuses.Active;
            
            _context.BankAccounts.Add(bankAccount);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBankAccountAsync(BankAccount bankAccount)
        {
            var existing = await GetBankAccountByIdAsync(bankAccount.BankAccountId)
                ?? throw new KeyNotFoundException("BankAccount not found");

            existing.BankName = bankAccount.BankName;
            existing.AccountNumber = bankAccount.AccountNumber;

            await _context.SaveChangesAsync();
        }

        public async Task MarkAsDeleteBankAccountAsync(Guid id)
        {
            var bankAccount = await GetBankAccountByIdAsync(id);

            _context.BankAccounts.Remove(bankAccount);
            await _context.SaveChangesAsync();
        }
    }
}

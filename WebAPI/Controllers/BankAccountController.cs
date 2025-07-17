using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankAccountController : ControllerBase
    {
        private readonly IBankAccountService _bankAccountService;
        
        public BankAccountController(IBankAccountService bankAccountService)
        {
            _bankAccountService = bankAccountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBankAccounts()
        {
            var bankAccounts = await _bankAccountService.GetBankAccountsAsync();
            return Ok(bankAccounts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBankAccountById(Guid id)
        {
            var bankAccount = await _bankAccountService.GetBankAccountByIdAsync(id);
            if (bankAccount == null)
            {
                return NotFound();
            }
            return Ok(bankAccount);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBankAccount([FromBody] BankAccount bankAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _bankAccountService.AddBankAccountAsync(bankAccount);
            return CreatedAtAction(nameof(GetBankAccountById), new { id = bankAccount.BankAccountId }, bankAccount);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBankAccount(Guid id, [FromBody] BankAccount bankAccount)
        {
            if (id != bankAccount.BankAccountId)
            {
                return BadRequest("Bank Account ID mismatch");
            }

            await _bankAccountService.UpdateBankAccountAsync(bankAccount);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBankAccount(Guid id)
        {
            await _bankAccountService.MarkAsDeleteBankAccountAsync(id);
            return NoContent();
        }
    }
}

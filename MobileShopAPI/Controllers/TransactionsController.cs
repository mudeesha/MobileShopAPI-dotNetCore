using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileShopAPI.DTOs.Transaction;
using MobileShopAPI.Services.Interfaces;
using System.Security.Claims;
using MobileShopAPI.Models;

namespace MobileShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) 
                ?? throw new UnauthorizedAccessException("User not authenticated");
        }

        // POST: api/transactions
        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDto dto)
        {
            try
            {
                var userId = GetUserId();
                var transaction = await _transactionService.CreateTransactionAsync(dto, userId);
                return Ok(transaction);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        // GET: api/transactions
        [HttpGet]
        public async Task<IActionResult> GetMyTransactions()
        {
            try
            {
                var userId = GetUserId();
                var transactions = await _transactionService.GetUserTransactionsAsync(userId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user transactions");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        // GET: api/transactions/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTransaction(int id)
        {
            try
            {
                var userId = GetUserId();
                var transaction = await _transactionService.GetTransactionAsync(id, userId);
                return Ok(transaction);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transaction {TransactionId}", id);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        // ADMIN ENDPOINTS
        // GET: api/transactions/admin/all
        [HttpGet("admin/all")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllTransactions([FromQuery] int? status = null)
        {
            try
            {
                var transactions = await _transactionService.GetAllTransactionsAsync(
                    status.HasValue ? (Enums.TransactionStatus?)status.Value : null);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all transactions");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        // PUT: api/transactions/admin/{id}/collect-cash
        [HttpPut("admin/{id:int}/collect-cash")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CollectCash(int id, [FromBody] UpdateCashCollectionDto dto)
        {
            try
            {
                var adminId = GetUserId();
                var transaction = await _transactionService.UpdateCashCollectionAsync(id, dto, adminId);
                return Ok(transaction);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error collecting cash for transaction {TransactionId}", id);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }
}
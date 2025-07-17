﻿using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VATController : ControllerBase
    {
        private readonly IVATService _vatService;

        public VATController(IVATService vatService)
        {
            _vatService = vatService;
        }

        [HttpGet]
        public async Task<IActionResult> GetVATs()
        {
            var vats = await _vatService.GetVATsAsync();
            return Ok(vats);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVATById(Guid id)
        {
            var vat = await _vatService.GetVATByIdAsync(id);
            if (vat == null)
            {
                return NotFound();
            }
            return Ok(vat);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVAT([FromBody] VAT vat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _vatService.AddVATAsync(vat);
            return CreatedAtAction(nameof(GetVATById), new { id = vat.VatId }, vat);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVAT(Guid id, [FromBody] VAT vat)
        {
            if (id != vat.VatId)
            {
                return BadRequest("VAT ID mismatch");
            }

            var existingVAT = await _vatService.GetVATByIdAsync(id);
            if (existingVAT == null)
            {
                return NotFound();
            }

            existingVAT.Percentage = vat.Percentage;

            await _vatService.UpdateVATAsync(existingVAT);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVAT(Guid id)
        {
            var vat = await _vatService.GetVATByIdAsync(id);
            if (vat == null)
            {
                return NotFound();
            }

            await _vatService.DeleteVATAsync(id);
            return NoContent();
        }
    }
}

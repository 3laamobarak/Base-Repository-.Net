using Company.Project.Application.Contracts;
using Company.Project.Domain.Models;
using Company.Project.Application.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Company.Project.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExampleClassController : Controller
    {
        private readonly IExampleClassService _exampleClassService;

        public ExampleClassController(IExampleClassService exampleClassService)
        {
            _exampleClassService = exampleClassService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int skip = 0, int take = 10)
        {
            var result = await _exampleClassService.GetAllAsync(skip, take);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExampleClassDto createDto)
        {
            if (createDto == null || string.IsNullOrEmpty(createDto.Name))
            {
                return BadRequest("Invalid data.");
            }
            var createdItem = await _exampleClassService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetAll), new { id = createdItem.Id }, createdItem);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _exampleClassService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateExampleClassDto updateDto)
        {
            if (updateDto == null || updateDto.Id != id || string.IsNullOrEmpty(updateDto.Name))
            {
                return BadRequest("Invalid data.");
            }
            try
            {
                var updatedItem = await _exampleClassService.UpdateAsync(updateDto);
                return Ok(updatedItem);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Item not found.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingItem = await _exampleClassService.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound();
            }

            await _exampleClassService.DeleteAsync(id);
            return NoContent();
            
        }

    }
}

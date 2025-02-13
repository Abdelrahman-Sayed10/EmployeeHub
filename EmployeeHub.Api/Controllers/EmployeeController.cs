using EmployeeHub.Application.Contracts.IService;
using EmployeeHub.Dtos.EmployeeDtos;
using EmployeeHub.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeHub.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var list = await this.employeeService.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult?> GetByIdAsync(int id)
        {
            var employee = await this.employeeService.GetByIdAsync(id);
            if (employee == null) return NotFound($"Employee with ID = {id} not found.");
            return Ok(employee);
        }

        [HttpPost]
        public async Task<EmployeeDto> AddAsync(EmployeeAddDto dto)
        {
            var newEmployee = await this.employeeService.AddAsync(dto);
            return newEmployee;
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, EmployeeUpdateDto employeeDto)
        {
            if (employeeDto.Id != id) 
                return BadRequest("ID mismatch");
            await this.employeeService.UpdateAsync(employeeDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await this.employeeService.DeleteAsync(id); 
            return NoContent();
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedAsync([FromQuery] int pageIndex = 1,
                                                [FromQuery] int pageSize = 10)
        {
            var result = await this.employeeService.GetPagedAsync(pageIndex, pageSize); 
            return Ok(result);
        }
    }
}

using EmployeeHub.Application.Contracts.IService;
using EmployeeHub.Dtos.DepartmentDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeHub.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            this.departmentService = departmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDepartmentsAsync()
        {
            var list = await this.departmentService.GetAllDepartmentsAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult?> GetDepartmentByIdAsync(int id)
        {
            var department = await this.departmentService.GetDepartmentByIdAsync(id);
            if (department == null) return NotFound();
            return Ok(department);
        }

        [HttpPost]
        public async Task<DepartmentDto> AddDepartmentAsync(DepartmentAddDto departmentDto)
        {
            var newDepartment = await this.departmentService.AddDepartmentAsync(departmentDto);
            return newDepartment;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartmentAsync(int id, DepartmentDto departmentDto)
        {
            if (departmentDto.Id != id)
                return BadRequest("ID mismatch");

            await this.departmentService.UpdateDepartmentAsync(departmentDto);
            return NoContent(); 
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartmentAsync(int id)
        {
            await departmentService.DeleteDepartmentAsync(id);
            return NoContent();
        }
    }
}

using EmployeeHub.Dtos.EmployeeDtos;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeHub.Web.Controllers
{
    public class EmployeeController : BaseController
    {
        // List employees
        [HttpGet]
        //public async Task<IActionResult> Index()
        //{
        //    var employees = await client.GetFromJsonAsync<List<EmployeeDto>>("/api/Employee/GetAll");

        //    if (employees == null)
        //    {
        //        return NotFound("No employees found");
        //    }

        //    return View(employees);
        //}

        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 5)
        {
            // API endpoint for with the QueryString 
            var apiUrl = $"/api/Employee/GetPaged/paged?pageIndex={pageIndex}&pageSize={pageSize}";

            var pagedResult = await client.GetFromJsonAsync<EmployeePagedDto<EmployeeDto>>(apiUrl);

            if (pagedResult == null || pagedResult.Data.Count == 0)
            {
                return View(new EmployeePagedDto<EmployeeDto>
                {
                    Data = new List<EmployeeDto>(),
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalCount = 0
                });
            }

            return View(pagedResult);
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Add(EmployeeAddDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var response = await client.PostAsJsonAsync("api/Employee/Add", dto);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to add employee");
                return View(dto);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var empDto = await client.GetFromJsonAsync<EmployeeDto>($"/api/Employee/GetById/{id}");

            if (empDto == null)
            {
                return NotFound("Employee not found");
            }

            // Convert EmployeeDto -> EmployeeUpdateDto
            var updateDto = new EmployeeUpdateDto
            {
                Id = empDto.Id.GetValueOrDefault(),
                FirstName = empDto.FullName.Split(' ')[0],
                LastName = empDto.FullName.Split(' ').Length > 1 ? empDto.FullName.Split(' ')[1] : "",
                Email = empDto.Email,
                IsActive = empDto.IsActive,
            };

            return View(updateDto);
        }

        [HttpPost]
        public async Task<IActionResult> Update(EmployeeUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return View(updateDto);
            }

            var response = await client.PutAsJsonAsync(
                $"/api/Employee/Update/{updateDto.Id}",
                updateDto
            );
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to update employee");
                return View(updateDto);
            }

            return RedirectToAction("Index");
        }

        // Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await client.DeleteAsync($"/api/Employee/Delete/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }
            return RedirectToAction("Index");
        }

    }
}

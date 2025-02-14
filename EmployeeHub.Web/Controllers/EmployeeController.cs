using EmployeeHub.Dtos.EmployeeDtos;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json.Serialization;

namespace EmployeeHub.Web.Controllers
{
    public class EmployeeController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var token = HttpContext.Session.GetString("jwtToken");
                if (string.IsNullOrEmpty(token))
                {
                    TempData["UnauthorizedMessage"] = "Session expired. Please login again.";
                    return RedirectToAction("Login", "Account");
                }

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync($"/api/Employee/GetPaged/Paged?pageIndex={pageIndex}&pageSize={pageSize}");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    TempData["UnauthorizedMessage"] = "Session expired. Please login again.";
                    return RedirectToAction("Login", "Account");
                }

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var pagedResult = System.Text.Json.JsonSerializer.Deserialize<EmployeePagedDto<EmployeeDto>>(
                     content,
                     new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(pagedResult);
            }
            catch (Exception)
            {
                TempData["Error"] = "Error loading employees";
                return RedirectToAction("Index", "Home");
            }
        }



        [HttpGet]
        public IActionResult Add()
        {
            if (!ViewBag.CurrentUserIsAdmin)
            {
                TempData["UnauthorizedMessage"] = "Only admins can add employees.";
                return RedirectToAction("Index");
            }
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

using EmployeeHub.Dtos.DepartmentDto;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeHub.Web.Controllers
{
    public class DepartmentController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var departments = await client.GetFromJsonAsync<List<DepartmentDto>>("/api/Department/GetAllDepartments");

            if (departments == null)
            {
                ViewBag.ErrorMessage = "No departments found";
                // Empty list to avoid null
                return View(new List<DepartmentDto>());
            }

            return View(departments);
        }


        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        //Post for adding
        [HttpPost]
        public async Task<IActionResult> Add(DepartmentAddDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var response = await client.PostAsJsonAsync("/api/Department/AddDepartment", dto);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to create department");
                return View(dto);
            }

            // For future logic
            var createdDepartment = await response.Content.ReadFromJsonAsync<DepartmentDto>();

            // Redirect to the Index to see the new department in the list
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var department = await client.GetFromJsonAsync<DepartmentDto>($"/api/Department/GetDepartmentById/{id}");

            if (department == null)
            {
                ViewBag.ErrorMessage = $"Department with ID {id} not found";
                return View("Error");
            }

            return View(department);
        }


        // POST for updating
        [HttpPost]
        public async Task<IActionResult> Update(DepartmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var response = await client.PutAsJsonAsync(
                $"/api/Department/UpdateDepartment/{dto.Id}",
                dto
            );
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = $"Department with ID {dto.Id} not found";
                return View("Error");
            }

            return RedirectToAction("Index");
        }

        // Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await client.DeleteAsync($"/api/Department/DeleteDepartment/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }
            return RedirectToAction("Index");
        }
    }
}

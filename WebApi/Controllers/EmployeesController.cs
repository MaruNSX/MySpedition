using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApi.Database;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(ILogger<EmployeesController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "Get Employees")]
        public IActionResult Get(int? id)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.Employees.AsSingleQuery();
                if (id.HasValue)
                {
                    query = query.Where(x => x.Id == id);
                }
                var employees = query.ToList();
                if (!employees.Any())
                {
                    return new NotFoundResult();
                }
                return new OkObjectResult(employees);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] Employee request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName) ||
                string.IsNullOrWhiteSpace(request.CarRegistrationNumber) ||
                !request.BirthDate.HasValue)
            {
                return new BadRequestObjectResult("All fields have to be filled in!");
            }

            using (var db = new DatabaseContext())
            {
                var employee = new EmployeeEntity
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    CarRegistrationNumber = request.CarRegistrationNumber,
                    BirthDate = request.BirthDate.Value
                };
                db.Employees.Add(employee);
                db.SaveChanges();
            }
            return new OkResult();
        }

        [HttpPut]
        public IActionResult Update([FromBody] Employee request, int id)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            
            using (var db = new DatabaseContext())
            {
                var employee = db.Employees.FirstOrDefault(x => x.Id == id);
                if (employee == null)
                {
                    return new NotFoundResult();
                }

                if (!string.IsNullOrWhiteSpace(request.FirstName))
                {
                    employee.FirstName = request.FirstName; 
                }
                if (!string.IsNullOrWhiteSpace(request.LastName))
                {
                    employee.LastName = request.LastName;
                }
                if (!string.IsNullOrWhiteSpace(request.CarRegistrationNumber))
                {
                    employee.CarRegistrationNumber = request.CarRegistrationNumber;
                }
                if (request.BirthDate.HasValue)
                {
                    employee.BirthDate = request.BirthDate.Value;
                }
                db.Update(employee);
                db.SaveChanges();
            }

            return new OkResult();
        }
    }
}

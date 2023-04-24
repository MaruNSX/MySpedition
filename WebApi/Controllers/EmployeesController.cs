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

        [HttpGet]
        public IActionResult Get(int? id)
        {
            _logger.LogInformation($"Executing fetch employees function. With id equal to {id}.");
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
                    _logger.LogInformation("There was no employees to return.");
                    return new NotFoundResult();
                }
                _logger.LogInformation($"Returning {employees.Count} employees.");
                return new OkObjectResult(employees);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] Employee request)
        {
            if (request == null)
            {
                _logger.LogError("Could not proceed with request as it was empty.");
                throw new ArgumentNullException(nameof(request));
            }
            if (string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName) ||
                string.IsNullOrWhiteSpace(request.CarRegistrationNumber) ||
                !request.BirthDate.HasValue)
            {
                _logger.LogError($"Some of input parameters are not filled in. Parameters:" +
                    $"FirstName: {request.FirstName}, " +
                    $"LastName: {request.LastName}, " +
                    $"CarRegistrationNumber: {request.CarRegistrationNumber}" +
                    $"BirthDate: {request.BirthDate}");
                return new BadRequestObjectResult("All fields have to be filled in!");
            }

            using (var db = new DatabaseContext())
            {
                _logger.LogInformation("Creating new employee.");
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
                _logger.LogError("Could not proceed with request as it was empty.");
                throw new ArgumentNullException(nameof(request));
            }
            
            using (var db = new DatabaseContext())
            {
                _logger.LogInformation($"Fetching employee with id {id}");
                var employee = db.Employees.FirstOrDefault(x => x.Id == id);
                if (employee == null)
                {
                    _logger.LogError($"There was no employee with id: {id}.");
                    return new NotFoundResult();
                }

                if (!string.IsNullOrWhiteSpace(request.FirstName))
                {
                    _logger.LogInformation($"Assigning first name: {request.FirstName}");
                    employee.FirstName = request.FirstName; 
                }
                if (!string.IsNullOrWhiteSpace(request.LastName))
                {
                    _logger.LogInformation($"Assigning last name: {request.LastName}");
                    employee.LastName = request.LastName;
                }
                if (!string.IsNullOrWhiteSpace(request.CarRegistrationNumber))
                {
                    _logger.LogInformation($"Assigning car registration number: {request.CarRegistrationNumber}");
                    employee.CarRegistrationNumber = request.CarRegistrationNumber;
                }
                if (request.BirthDate.HasValue)
                {
                    _logger.LogInformation($"Assigning birth date: {request.BirthDate}");
                    employee.BirthDate = request.BirthDate.Value;
                }
                _logger.LogInformation("Updating employee");
                db.Update(employee);
                db.SaveChanges();
            }

            return new OkResult();
        }
    }
}

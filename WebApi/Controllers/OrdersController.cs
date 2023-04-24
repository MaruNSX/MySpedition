using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Database;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(ILogger<OrdersController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get(int? id)
        {
            using (var db = new DatabaseContext())
            {
                var query = db.Orders.AsSingleQuery();
                if (id.HasValue)
                {
                    query = query.Where(x => x.Id == id);
                }
                var orders = query.AsNoTracking().Include(x => x.Employee).ToList();
                if (!orders.Any())
                {
                    return new NotFoundResult();
                }
                return new OkObjectResult(orders);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] Order request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (!request.StartTime.HasValue ||
                !request.EndTime.HasValue ||
                !request.Price.HasValue ||
                !request.EmployeeId.HasValue)
            {
                return new BadRequestObjectResult("All fields have to be filled in!");
            }

            using (var db = new DatabaseContext())
            {
                var employee = db.Employees.FirstOrDefault(x => x.Id == request.EmployeeId);
                if (employee == null) 
                {
                    return new BadRequestObjectResult($"Could not find employee with id: {request.EmployeeId}.");
                }
                var order = new OrderEntity
                {
                    StartTime = request.StartTime.Value,
                    EndTime = request.EndTime.Value,
                    Price = request.Price.Value,
                    Employee = employee
                };
                db.Orders.Add(order);
                db.SaveChanges();
            }
            return new OkResult();
        }

        [HttpPut]
        public IActionResult Update([FromBody] Order request, int id)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            using (var db = new DatabaseContext())
            {
                var order = db.Orders.FirstOrDefault(x => x.Id == id);
                if (order == null)
                {
                    return new NotFoundObjectResult($"Could not find order with id: {id}.");
                }

                if (request.StartTime.HasValue)
                {
                    order.StartTime = request.StartTime.Value;
                }
                if (request.EndTime.HasValue)
                {
                    order.EndTime = request.EndTime.Value;
                }
                if (request.Price.HasValue)
                {
                    order.Price = request.Price.Value;
                }
                if (request.EmployeeId.HasValue)
                {
                    var employee = db.Employees.FirstOrDefault(x => x.Id == request.EmployeeId);
                    if (employee == null)
                    {
                        return new BadRequestObjectResult($"Could not find employee with id: {request.EmployeeId}.");
                    }

                    order.Employee = employee;
                }
                db.Update(order);
                db.SaveChanges();
            }

            return new OkResult();
        }
    }
}

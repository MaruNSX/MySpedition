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
            _logger.LogInformation($"Fetching orders with id: {id}");
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
                    _logger.LogInformation("There was no orders to return.");
                    return new NotFoundResult();
                }
                _logger.LogInformation($"Returning {orders.Count} orders");
                return new OkObjectResult(orders);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] Order request)
        {
            if (request == null)
            {
                _logger.LogError("Could not proceed with request as it was empty.");
                throw new ArgumentNullException(nameof(request));
            }
            if (!request.StartTime.HasValue ||
                !request.EndTime.HasValue ||
                !request.Price.HasValue ||
                !request.EmployeeId.HasValue ||
                string.IsNullOrWhiteSpace(request.StartCity) ||
                string.IsNullOrWhiteSpace(request.Destination)
            {
                _logger.LogError($"Some of input parameters are not filled in: Parameters:" +
                    $"StartTime: {request.StartTime}," +
                    $"EndTime: {request.EndTime}," +
                    $"Price: {request.Price}," +
                    $"EmployeeId: {request.EmployeeId}," +
                    $"StartCity: {request.StartCity}," +
                    $"Destination: {request.Destination}");
                return new BadRequestObjectResult("All fields have to be filled in!");
            }

            using (var db = new DatabaseContext())
            {
                var employee = db.Employees.FirstOrDefault(x => x.Id == request.EmployeeId);
                if (employee == null) 
                {
                    _logger.LogError($"Could not find employee with id {request.EmployeeId}.");
                    return new BadRequestObjectResult($"Could not find employee with id: {request.EmployeeId}.");
                }
                _logger.LogInformation("Creating new order");
                var order = new OrderEntity
                {
                    StartTime = request.StartTime.Value,
                    EndTime = request.EndTime.Value,
                    Price = request.Price.Value,
                    Employee = employee,
                    StartCity = request.StartCity,
                    Destination = request.Destination,
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
                _logger.LogError("Could not proceed with request as it was empty.");
                throw new ArgumentNullException(nameof(request));
            }

            using (var db = new DatabaseContext())
            {
                _logger.LogInformation($"Fetching order with id {id}");
                var order = db.Orders.FirstOrDefault(x => x.Id == id);
                if (order == null)
                {
                    _logger.LogError($"There is no order with id: {id}.");
                    return new NotFoundObjectResult($"Could not find order with id: {id}.");
                }

                if (request.StartTime.HasValue)
                {
                    _logger.LogInformation($"Assigning start time: {request.StartTime}");
                    order.StartTime = request.StartTime.Value;
                }
                if (request.EndTime.HasValue)
                {
                    _logger.LogInformation($"Assigning end time: {request.EndTime}");
                    order.EndTime = request.EndTime.Value;
                }
                if (request.Price.HasValue)
                {
                    _logger.LogInformation($"Assigning price: {request.Price}");
                    order.Price = request.Price.Value;
                }
                if (!string.IsNullOrWhiteSpace(request.StartCity))
                {
                    _logger.LogInformation($"Assigning start city: {request.StartCity}");
                    order.StartCity = request.StartCity;
                }
                if (!string.IsNullOrWhiteSpace(request.Destination))
                {
                    _logger.LogInformation($"Assigning destination: {request.Destination}");
                    order.Destination = request.Destination;
                }

                if (request.EmployeeId.HasValue)
                {
                    var employee = db.Employees.FirstOrDefault(x => x.Id == request.EmployeeId);
                    if (employee == null)
                    {
                        _logger.LogError($"Could not find employee with id {request.EmployeeId}");
                        return new BadRequestObjectResult($"Could not find employee with id: {request.EmployeeId}.");
                    }
                    _logger.LogInformation("Assigning employee.");
                    order.Employee = employee;
                }
                _logger.LogInformation("Updating employee");
                db.Update(order);
                db.SaveChanges();
            }

            return new OkResult();
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                _logger.LogError("Id has to be filled in.");
                return new BadRequestObjectResult("Order id has to be provided.");
            }

            using (var db = new DatabaseContext())
            {
                _logger.LogInformation($"Trying to fetch order with id: {id.Value}");
                var order = db.Orders.FirstOrDefault(x => x.Id == id.Value);
                if (order == null)
                {
                    _logger.LogError($"Order with id {id.Value} does not exists");
                    return new NotFoundObjectResult($"Order with id {id.Value} could not be found.");
                }

                _logger.LogInformation("Removing order");
                db.Orders.Remove(order);
                db.SaveChanges();
            }

            return new OkResult();
        }
    }
}

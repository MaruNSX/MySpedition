namespace WebApi.Models
{
    public class Order
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set;}
        public decimal? Price { get; set; }
        public int? EmployeeId { get; set; }
        public string StartCity { get; set; }
        public string Destination { get; set; }
    }
}

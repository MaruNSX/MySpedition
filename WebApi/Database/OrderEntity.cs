using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Database
{
    public class OrderEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Price { get; set; }
        public EmployeeEntity Employee { get; set; }
        public string StartCity { get; set; }
        public string Destination { get; set; }
    }
}

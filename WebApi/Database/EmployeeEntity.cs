using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Database
{
    public class EmployeeEntity
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string CarRegistrationNumber { get; set; }
    }
}

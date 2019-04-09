using System;

namespace LogSystem.Models
{
    public class TestModel
    {
        public decimal Amount { get; set; }
        public string ProductName { get; set; }
        public DateTime CreateOnDate { get; set; }
        public Guid Id { get; set; }
    }
}
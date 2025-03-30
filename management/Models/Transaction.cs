using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace management.Models
{
    public class Transaction
    {
        public int code { get; set; }
        [NotMapped]
        public DateTime Date { get; set; }
        [Range(0.04, double.MaxValue)]
        public decimal amount { get; set; }
        public int account_code { get; set; }
        public string? description { get; set; }
        public DateTime transaction_date { get; internal set; } = DateTime.Now;
        public DateTime capture_date { get; internal set; }
    }
}

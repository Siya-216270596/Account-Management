using System.ComponentModel.DataAnnotations;

namespace management.Models
{
    public class Transaction
    {
        public int code { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal amount { get; set; }
        public int account_code { get; set; }
        public string? description { get; set; }
        public DateTime transaction_date { get; internal set; }
        public DateTime capture_date { get; internal set; }
    }
}

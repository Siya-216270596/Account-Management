using System.ComponentModel.DataAnnotations;

namespace management.Models
{
    public class Transaction
    {
        public int Code { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
        public int AccountCode { get; set; }
        public Account Account { get; set; }
        public int personCode { get; set; }
        public string Description { get; set; }
        public DateTime TransactionDate { get; internal set; }
        public DateTime CaptureDate { get; internal set; }
    }
}

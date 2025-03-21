using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace management.Models
{
    public class Account
    {
        [Required]
        public int code { get; set; }
        [Required]
        public string? account_number { get; set; }
        public int PersonCode { get; set; }
        [Required]
        public Person? Person { get; set; }
        [Required]
        public ICollection<Transaction>? Transactions { get; set; }
        public decimal outstanding_balance { get; internal set; }
    }
}

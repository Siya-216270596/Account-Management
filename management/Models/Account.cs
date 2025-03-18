using System.ComponentModel.DataAnnotations;

namespace management.Models
{
    public class Account
    {
        [Required]
        public int Code { get; set; }
        [Required]
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public bool IsClosed { get; set; }
        public int PersonCode { get; set; }
        public Person Person { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
        public decimal OutstandingBalance { get; internal set; }
    }
}

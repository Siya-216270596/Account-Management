using management.Models.Response;
using System.ComponentModel.DataAnnotations;

namespace management.Models
{
    public class Person
    {
        [Required]
        public int Code { get; set; }

        [StringLength(13, ErrorMessage = "Id length can't be more than 13.")]
        public string? id_number { get; set; }

        public string? Name { get; set; }

        public string? Surname { get; set; }
        public List<Account> Account { get; set; } = [];
    }
}

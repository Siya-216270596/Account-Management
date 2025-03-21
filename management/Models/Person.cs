using management.Models.Response;
using System.ComponentModel.DataAnnotations;

namespace management.Models
{
    public class Person
    {
        [Required]
        public int Code { get; set; }

        public string? id_number { get; set; }

        public string? Name { get; set; }

        public string? Surname { get; set; }
        public IEnumerable<Account>? Account { get; set; }
    }
}

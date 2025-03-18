using System.ComponentModel.DataAnnotations;

namespace management.Models
{
    public class Person
    {
        [Required]
        public int Code { get; set; }
        [Required]
        public string id_number { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
    }
}

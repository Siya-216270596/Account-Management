using management.Models;

namespace management.Views
{
    public class PersonAccountViewModel
    {
        public Person Person { get; set; }
        public Account NewAccount { get; set; } = new Account();
    }

}

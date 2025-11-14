using System.ComponentModel;
using Microsoft.Identity.Client;

namespace Egov.Models
{
    public class Citizen
    {
        public int Id { get; set; }
        public string FirstName {  get; set; }  
        public string LastName { get; set; }
        
        public string? MiddleName {  get; set; }
        public string Phone {  get; set; }
        public string Email {  get; set; }
        public string CitizenshipNo { get; set; }
        public string? CitizenF {  get; set; }
        public string? CitizenB { get; set; }
        public bool Status { get; set; }
        public virtual IList<SelectedCoinViewModel>? SelectedCoinViewModels { get; set; }

    }
}

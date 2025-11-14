using Microsoft.EntityFrameworkCore;

namespace Egov.Models
{
    public class SelectedCoinViewModel
    {
      
            public int Id {  get; set; }       
            public string CoinName { get; set; }
            public int Quantity { get; set; }
            public decimal TotalPrice { get; set; }
            public int CitizenId {  get; set; }
            public virtual Citizen? Citizen { get; set; }

    }
}

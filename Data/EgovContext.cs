using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Egov.Models;

namespace Egov.Data
{
    public class EgovContext : DbContext
    {
        public EgovContext (DbContextOptions<EgovContext> options)
            : base(options)
        {
        }

        public DbSet<Egov.Models.Admin> Admin { get; set; } = default!;
        public DbSet<Egov.Models.Citizen> Citizen { get; set; } = default!;
        public DbSet<Egov.Models.Coin> Coin { get; set; } = default!;
        public DbSet<Egov.Models.SelectedCoinViewModel> SelectedCoinViewModel { get; set; } = default!;
    }
}

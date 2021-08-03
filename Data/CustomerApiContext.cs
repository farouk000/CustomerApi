using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CustomerApi.Models;

namespace CustomerApi.Data
{
    public class CustomerApiContext : DbContext
    {
        public CustomerApiContext (DbContextOptions<CustomerApiContext> options)
            : base(options)
        {
        }

        public DbSet<CustomerApi.Models.User> User { get; set; }

        public DbSet<CustomerApi.Models.Version> Version { get; set; }

        public DbSet<CustomerApi.Models.Customer> Customer { get; set; }
    }
}

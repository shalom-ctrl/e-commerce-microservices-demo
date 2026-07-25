using Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Infrastructure.Data
{
        public class AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : DbContext(options)
        {
        public DbSet<AppUser> Users { get; set; }
        }
}

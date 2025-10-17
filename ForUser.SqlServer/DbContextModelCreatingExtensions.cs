using ForUser.Domains.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.SqlServer
{
    public static class DbContextModelCreatingExtensions
    {
        public static void Configure(this ModelBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            builder.Entity<UserEntity>(entity =>
            {
            
            });
        }
    }
}

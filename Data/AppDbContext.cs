using CDNFreelancer.Models;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;

namespace CDNFreelancer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }

}

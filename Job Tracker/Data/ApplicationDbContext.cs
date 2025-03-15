using Job_Tracker.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Job_Tracker.Data
{

    public class ApplicationDbContext : DbContext
    {
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<UserModel> Users { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SamStoreMVC.Models;

namespace SamStoreMVC.Services
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions Options) : base(Options)
        { }
        public DbSet<Products> Products { get; set; }
    }
}

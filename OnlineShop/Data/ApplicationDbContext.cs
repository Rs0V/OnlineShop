using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;

namespace OnlineShop.Data
{
	public class ApplicationDbContext : IdentityDbContext<Utilizator>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		// For Composite Primary Keys
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Exemplar>()
				.HasKey(ex => new { ex.ProdusId, ex.Numar_Produs });

			modelBuilder.Entity<Review>()
				.HasKey(rev => new { rev.UtilizatorId, rev.ProdusId });
		}

		public DbSet<Utilizator> Utilizatori { get; set; }
		public DbSet<Review> Reviewuri { get; set; }
		public DbSet<Produs> Produse { get; set; }
		public DbSet<Exemplar> Exemplare { get; set; }
		public DbSet<Categorie> Categorii { get; set; }
		public DbSet<Comanda> Comenzi { get; set; }
		public DbSet<Cerere> Cereri { get; set; }
	}
}
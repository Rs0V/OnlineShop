using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;

namespace OnlineShop.Models
{
	// Pasul 4 - useri si roluri
	public static class SeedData
	{
		public static void Initialize(IServiceProvider
		serviceProvider)
		{
			using (var context = new ApplicationDbContext(
			serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
			{
				if (context.Roles.Any())
				{
					return; // baza de date contine deja roluri
				}
				
				context.Roles.AddRange( // Roluri
					new IdentityRole
					{
						Id = "2c5e174e-3b0e-446f-86af-483d56fd7210",
						Name = "Administrator",
						NormalizedName = "Administrator".ToUpper()
					},

					new IdentityRole
					{
						Id = "2c5e174e-3b0e-446f-86af-483d56fd7211",
						Name = "Colaborator",
						NormalizedName = "Colaborator".ToUpper()
					},

					new IdentityRole
					{
						Id = "2c5e174e-3b0e-446f-86af-483d56fd7212",
						Name = "Utilizator",
						NormalizedName = "Utilizator".ToUpper()
					}
				);
   
				var hasher = new PasswordHasher<Utilizator>();
				
				context.Users.AddRange( // Useri
					new Utilizator
					{
						Id = "8e445865-a24d-4543-a6c6-9443d048cdb0", // primary key

						Nume = "Admin",
						Prenume = "Admin",

						UserName = "admin@test.com",
						NormalizedUserName = "ADMIN@TEST.COM",

						Email = "admin@test.com",
						EmailConfirmed = true,
						NormalizedEmail = "ADMIN@TEST.COM",

						PasswordHash = hasher.HashPassword(null, "admin")
					},

					new Utilizator
					{
						Id = "8e445865-a24d-4543-a6c6-9443d048cdb1", // primary key

						Nume = "Colab1",
						Prenume = "Colab1",

						UserName = "colab1@test.com",
						NormalizedUserName = "COLAB1@TEST.COM",

						Email = "colab1@test.com",
						EmailConfirmed = true,
						NormalizedEmail = "COLAB1@TEST.COM",

						PasswordHash = hasher.HashPassword(null, "colab1")
					},

					new Utilizator
					{ 
						Id = "8e445865-a24d-4543-a6c6-9443d048cdb2", // primary key

						Nume = "User1",
						Prenume = "User1",

						UserName = "user1@test.com",
						NormalizedUserName = "USER1@TEST.COM",

						Email = "user1@test.com",
						EmailConfirmed = true,
						NormalizedEmail = "USER1@TEST.COM",

						PasswordHash = hasher.HashPassword(null, "user1")
					}
				);

				// ASOCIEREA USER-ROLE
				context.UserRoles.AddRange(
					new IdentityUserRole<string>
					{
						RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",

						UserId = "8e445865-a24d-4543-a6c6-9443d048cdb0"
					},

					new IdentityUserRole<string>
					{
						RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7211",

						UserId = "8e445865-a24d-4543-a6c6-9443d048cdb1"
					},

					new IdentityUserRole<string>
					{ 
						RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7212",

						UserId = "8e445865-a24d-4543-a6c6-9443d048cdb2"
					}
				);

				context.SaveChanges();
			}
		}
	}
}

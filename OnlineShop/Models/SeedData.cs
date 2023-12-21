using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;

namespace OnlineShop.Models
{
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
                    return;
                }
                
                context.Roles.AddRange(
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
                
                context.Users.AddRange(
                    new Utilizator
                    {
                        Id = "8e445865-a24d-4543-a6c6-9443d048cdb0", // primary key
                        Nume = "Admin",
                        Prenume = "Admin",
                        Email = "admin@test.com",
                        // Parola = "admin",
                        PasswordHash = hasher.HashPassword(null, "admin")
                    },

                    new Utilizator
                    {
                        Id = "8e445865-a24d-4543-a6c6-9443d048cdb1", // primary key
                        Nume = "Colab1",
                        Prenume = "Colab1",
                        Email = "colab1@test.com",
                        // Parola = "colab1",
                        PasswordHash = hasher.HashPassword(null, "colab1")
                    },

                    new Utilizator
                    { 
                        Id = "8e445865-a24d-4543-a6c6-9443d048cdb2", // primary key
                        Nume = "Iacovita",
                        Prenume = "Cristian",
                        Email = "user1@test.com",
                        // Parola = "user1",
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

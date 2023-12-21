using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Utilizator : IdentityUser // are implicit un PK string Id
    {
        // [Key]
        // public int Id { get; set; }

        // [Required(ErrorMessage = "Tipul utilizatorului este obligatoriu")]
        // public string Tip { get; set; }

        // [Required(ErrorMessage = "Numele utilizatorului este obligatoriu")]
        public string? Nume { get; set; }
        
        // [Required(ErrorMessage = "Prenumele utilizatorului este obligatoriu")]
        public string? Prenume { get; set; }

        [MinLength(10, ErrorMessage = "Numarul de telefon trebuie sa fie format din exact 10 cifre")]
        [MaxLength(10, ErrorMessage = "Numarul de telefon trebuie sa fie format din exact 10 cifre")]
        public string? Telefon { get; set; }


        public virtual ICollection<Review>? Reviewuri { get; set; }

        public virtual ICollection<Comanda>? Comenzi { get; set; }
    }
}

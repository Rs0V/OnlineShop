using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Utilizator
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tipul utilizatorului este obligatoriu")]
        public string Tip { get; set; }

        [Required(ErrorMessage = "Numele utilizatorului este obligatoriu")]
        public string Nume { get; set; }
        
        [Required(ErrorMessage = "Prenumele utilizatorului este obligatoriu")]
        public string Prenume { get; set; }

        [Required(ErrorMessage = "Email-ul utilizatorului este obligatoriu")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Parola este obligatorie")]
        [MaxLength(48, ErrorMessage = "Titlul nu poate avea mai mult de 48 de caractere")]
        [MinLength(8, ErrorMessage = "Lungimea minima trebuie sa fie de 8 caractere")]
        public string Parola { get; set; }

        [MinLength(10, ErrorMessage = "Numarul de telefon trebuie sa fie format din exact 10 cifre")]
        [MaxLength(10, ErrorMessage = "Numarul de telefon trebuie sa fie format din exact 10 cifre")]
        public string Telefon { get; set; }
    }
}

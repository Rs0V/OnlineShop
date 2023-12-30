using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace OnlineShop.Models
{
	public class Utilizator : IdentityUser // are implicit un PK string Id
	{
		// [Required(ErrorMessage = "Numele utilizatorului este obligatoriu")]
		[StringLength(32, ErrorMessage = "Numele utilizatorului nu poate depasi 32 de caractere")]
		public string? Nume { get; set; }
		
		// [Required(ErrorMessage = "Prenumele utilizatorului este obligatoriu")]
		[StringLength(64, ErrorMessage = "Numele utilizatorului nu poate depasi 64 de caractere")]
		public string? Prenume { get; set; }

		[StringLength(10, MinimumLength = 10, ErrorMessage = "Numarul de telefon al utilizatorului trebuie sa aiba exact 10 cifre")]
		public string? Telefon { get; set; }

		public virtual ICollection<Review>? Reviewuri { get; set; }

		public virtual ICollection<Comanda>? Comenzi { get; set; }
	}
}

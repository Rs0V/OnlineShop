using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
	public class Produs
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "Titlul produsului este obligatoriu")]
		public string Titlu { get; set; }

		[Required(ErrorMessage = "Descrierea produsului este obligatorie")]
        public string Descriere { get; set; }

		[Required(ErrorMessage = "Pretul produsului este obligatoriu")]
        public float Pret { get; set; }

		[Required(ErrorMessage = "Poza produsului este obligatorie")]
        public string Poza { get; set; }

		[Range(1, 5, ErrorMessage = "Rating-ul trebuie sa fie intre 1 si 5 stele")]
        public int? Rating { get; set; }

		[Required(ErrorMessage = "Id-ul categoriei produsului este obligatoriu")]
		public int? Id_Categorie { get; set; }

		[Required(ErrorMessage = "Categoria produsului este obligatorie")]
		public Categorie? Categorie { get; set; }
	}
}


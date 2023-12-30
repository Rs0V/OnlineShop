using System.ComponentModel.DataAnnotations;


namespace OnlineShop.Models
{
	public class Produs
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "Titlul produsului este obligatoriu")]
		[StringLength(64, ErrorMessage = "Titlul produsului nu poate depasi 64 caractere")]
		public string Titlu { get; set; }

		[Required(ErrorMessage = "Descrierea produsului este obligatorie")]
		[StringLength(512, ErrorMessage = "Descrierea produsului nu poate depasi 512 caractere")]
		public string Descriere { get; set; }

		[Required(ErrorMessage = "Pretul produsului este obligatoriu")]
		[Range(0, double.MaxValue, ErrorMessage = "Pretul produsului trebuie sa fie mai mare ca 0")]
		public float Pret { get; set; }

		[Required(ErrorMessage = "Poza produsului este obligatorie")]
		public string? Poza { get; set; }

		[Range(1, 5, ErrorMessage = "Rating-ul trebuie sa fie intre 1 si 5 stele")]
		public int? Rating { get; set; }

		[Required(ErrorMessage = "Id-ul categoriei produsului este obligatoriu")]
		public int? CategorieId { get; set; }

		public virtual Categorie? Categorie { get; set; }

		public virtual ICollection<Exemplar>? Exemplare { get; set; }

		public virtual ICollection<Review>? Reviewuri { get; set; }



		public override string ToString()
		{
			string conv = "";

			conv += Titlu + "╚";
			conv += Descriere + "╚";
			conv += Pret.ToString() + "╚";
			conv += Poza + "╚";
			conv += Rating.ToString() + "╚";
			conv += CategorieId.ToString() + "╚";

			return conv;
		}
	}
}


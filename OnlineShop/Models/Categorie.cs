using System.ComponentModel.DataAnnotations;


namespace OnlineShop.Models
{
	public class Categorie
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "Denumirea categoriei este obligatorie")]
		public string? Denumire { get; set; }

		public virtual ICollection<Produs>? Produse { get; set; }
	}
}

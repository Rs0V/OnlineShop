using System.ComponentModel.DataAnnotations;


namespace OnlineShop.Models
{
	public enum Acceptare
	{
		In_Asteptare,
		Acceptat,
		Respins
	}

	public class Cerere
	{
		[Key]
		public int Id { get; set; }


		public int? ProdusId { get; set; }

		public virtual Produs? Produs { get; set; }

		public string? AuxProd { get; set; } // Produsul auxiliar sub forma de string


		[Required(ErrorMessage = "Informatia este obligatorie")]
		[StringLength(256, ErrorMessage = "Informatia depaseste 256 de caractere")]
		public string? Info { get; set; }


		[Required(ErrorMessage = "Statusul de acceptat este obligatoriu")]
		public Acceptare Acceptat { get; set; }


		[Required(ErrorMessage = "Data cererii este obligatorie")]
		public DateTime Data { get; set; }
	}
}

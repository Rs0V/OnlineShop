using System.ComponentModel.DataAnnotations;


namespace OnlineShop.Models
{
	public enum StareComanda // struct that contains only data types (not methods)
	{
		Preluata,
		Efectuata
	}

	public class Comanda
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "Valoarea este obligatorie")]
		public float Valoare { get; set; }

		[Required(ErrorMessage = "Starea comenzii este obligatorie")]
		public StareComanda Stare { get; set; }

		[Required(ErrorMessage = "Data comenzii este obligatorie")]
		public DateTime Data { get; set; }

		[Required(ErrorMessage = "ID-ul utilizatorului este obligatoriu")]
		public string? UtilizatorId { get; set; }

		public virtual Utilizator? Utilizator { get; set; }

		public virtual ICollection<Exemplar>? Exemplare { get; set; }
	}
}

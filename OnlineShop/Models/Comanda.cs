using System.ComponentModel.DataAnnotations;


namespace OnlineShop.Models
{
	public class StareComandaAttribute : ValidationAttribute
	{
		private readonly string[] stariComanda = { "Efectuata", "Preluata" };

		public override bool IsValid(object? value)
		{
			if (value is string stare)
				return stariComanda.Contains(stare);
			return false;
		}
	}

	public class Comanda
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "Valoarea este obligatorie")]
		public int Valoare { get; set; }

		[Required(ErrorMessage = "Starea comenzii este obligatorie")]
		[StareComanda(ErrorMessage = "Stare invalida")]
		public string? Stare { get; set; }

		[Required(ErrorMessage = "Data comenzii este obligatorie")]
		public DateTime Data { get; set; }

		[Required(ErrorMessage = "ID-ul utilizatorului este obligatoriu")]
		public string? UtilizatorId { get; set; }

		public virtual Utilizator? Utilizator { get; set; }

		public virtual ICollection<Exemplar>? Exemplare { get; set; }
	}
}

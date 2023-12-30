using System.ComponentModel.DataAnnotations;


namespace OnlineShop.Models
{
	public class StareExemplarAttribute : ValidationAttribute
	{
		private readonly string[] stariEx = { "Disponibil", "Cumparat" };

		public override bool IsValid(object? value)
		{
			if (value is string stare)
				return stariEx.Contains(stare);
			return false;
		}
	}

	public class Exemplar
	{
		public int ProdusId { get; set; }

		public virtual Produs? Produs { get; set; }

		public int Numar_Produs { get; set; }

		[Required(ErrorMessage = "Starea exemplarului este obligatorie")]
		[StareExemplar(ErrorMessage = "Stare invalida")]
		public string? Stare { get; set; }

		public int? ComandaId { get; set; }

		public virtual Comanda? Comanda { get; set; }
	}
}


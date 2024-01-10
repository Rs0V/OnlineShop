using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; } // Numar_Exemplar

		public int? ProdusId { get; set; }

		public virtual Produs? Produs { get; set; }
        

		[Required(ErrorMessage = "Starea exemplarului este obligatorie")]
		[StareExemplar(ErrorMessage = "Stare invalida")]
		public string? Stare { get; set; }

		public int? ComandaId { get; set; }

		public virtual Comanda? Comanda { get; set; }
	}
}


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
	public enum StareExemplar
	{
		Disponibil,
		Comandat
	}

	public class Exemplar
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; } // Numar_Exemplar

		[Required(ErrorMessage = "Produsul exemplarului este obligatoriu")]
		public int? ProdusId { get; set; }

		public virtual Produs? Produs { get; set; }


		[Required(ErrorMessage = "Starea exemplarului este obligatorie")]
		public StareExemplar Stare { get; set; }

		public int? ComandaId { get; set; }

		public virtual Comanda? Comanda { get; set; }
	}
}


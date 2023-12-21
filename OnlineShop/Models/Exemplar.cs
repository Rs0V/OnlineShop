using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class StareExAttribute : ValidationAttribute
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
		public int Id_Produs { get; set; }

        public int Numar_Produs { get; set; }

        [Required(ErrorMessage = "Starea exemplarului este obligatorie")]
        [StareEx(ErrorMessage = "Stare invalida")]
		public string Stare { get; set; }

		public int? Id_Comanda { get; set; }

		public Comanda? Comanda { get; set; }
	}
}


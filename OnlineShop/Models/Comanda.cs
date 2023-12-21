using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [StareComanda(ErrorMessage = "Stare invalida")]
        public string Stare { get; set; }

        [Required(ErrorMessage = "Data comenzii este obligatorie")]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "ID-ul utilizatorului este obligatoriu")]
        public int? UtilizatorId { get; set; }

        public virtual Utilizator? Utilizator { get; set; }

        public virtual ICollection<Exemplar>? Exemplare { get; set; }
    }
}

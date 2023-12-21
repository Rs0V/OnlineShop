using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Categorie
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Denumirea categoriei este obligatorie")]
        public string Denumire { get; set; }
    }
}

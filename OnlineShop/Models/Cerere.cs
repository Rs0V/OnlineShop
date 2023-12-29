using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public enum Acceptare
    {
        Acceptat,
        Respins
    }

    public class Cerere
    {
        [Key]
        public int Id { get; set; }


        public int? ProdusId { get; set; }

        [Required(ErrorMessage = "Informatia este obligatorie")]
        [StringLength(256, ErrorMessage = "Informatia depaseste 256 de caractere")]
        public string Info { get; set; }

        // [Required(ErrorMessage = "Statusul de acceptat este obligatoriu")]
        public Acceptare? Acceptat { get; set; }

        [Required(ErrorMessage = "Statusul de respins este obligatoriu")]
        public bool Respins { get; set; }

        [Required(ErrorMessage = "Data cererii este obligatorie")]
        public DateTime Data { get; set; }

        public virtual Produs? Produs { get; set; }
    }

    
}

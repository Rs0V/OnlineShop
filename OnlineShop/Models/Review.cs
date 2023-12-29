using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace OnlineShop.Models
{
    public class Review
    {
        public string UtilizatorId { get; set; }

        public int ProdusId { get; set; }

        [Required(ErrorMessage = "Continutul review-ului este obligatoriu")]
        public string Continut { get; set; }

        [Required(ErrorMessage = "Rating-ul review-ului este obligatoriu")]
        [Range(1, 5, ErrorMessage = "Rating-ul trebuie sa fie intre 1 si 5 stele")]
        public int Rating { get; set; }

        public virtual Utilizator? Utilizator { get; set; }
    }
}

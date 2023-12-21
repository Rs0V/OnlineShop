using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace OnlineShop.Models
{
    public class Review
    {
        // Comentat atribute cheie -> rezolvat in ApplicationDbContext (Package Managerul nu merge altfel)

        // Composite Primary Key
        //[Key, Column(Order = 0)]
        public int UtilizatorId { get; set; }

        //[Key, Column(Order = 1)]
        public int ProdusId { get; set; }

        [Required(ErrorMessage = "Continutul review-ului este obligatoriu")]
        public string Continut { get; set; }
    }
}

﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace OnlineShop.Models
{
    public class Review
    {
        public int UtilizatorId { get; set; }

        public int ProdusId { get; set; }

        [Required(ErrorMessage = "Continutul review-ului este obligatoriu")]
        public string Continut { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Domain.Entities
{
    public class PropertyImage
    {
        [Key]
        public int IdPropertyImage { get; set; }

        public int IdProperty { get; set; }

        [ForeignKey(nameof(IdProperty))]
        public Property Property { get; set; }

        [Required]
        public string File { get; set; }

        public bool Enabled { get; set; }
    }
}

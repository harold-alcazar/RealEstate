using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Domain.Entities
{
    public class PropertyTrace
    {
        [Key]
        public int IdPropertyTrace { get; set; }

        public DateTime DateSale { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; }

        public decimal Value { get; set; }

        public decimal Tax { get; set; }

        public int IdProperty { get; set; }

        [ForeignKey(nameof(IdProperty))]
        public Property Property { get; set; }
    }
}

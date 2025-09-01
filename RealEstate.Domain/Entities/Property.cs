using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Domain.Entities
{
    public class Property
    {
        [Key]
        public int IdProperty { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(250)]
        public string Address { get; set; }

        public decimal Price { get; set; }

        [Required]
        public int CodeInternal { get; set; }

        public int Year { get; set; }

        // FK hacia Owner
        public int IdOwner { get; set; }

        [ForeignKey(nameof(IdOwner))]
        public Owner Owner { get; set; }

        public ICollection<PropertyImage> PropertyImages { get; set; } = new List<PropertyImage>();

        public ICollection<PropertyTrace> PropertyTraces { get; set; } = new List<PropertyTrace>();
    }
}

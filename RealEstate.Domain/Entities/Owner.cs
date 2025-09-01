using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Domain.Entities
{
    public class Owner
    {
        [Key]
        public int IdOwner { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(250)]
        public string Address { get; set; }

        public string Photo { get; set; }

        public DateTime? Birthday { get; set; }

        // Relación uno a muchos: Un Owner tiene muchas Properties
        public ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityConnector.Models
{
    // Model used for DbContext
    // Schema generated from this model
    public class NumberData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public string Key { get; set; }
        [Required]
        public DateTime Time { get; set; }
        [Required]
        public float Value { get; set; }
    }
}
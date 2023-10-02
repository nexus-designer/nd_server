using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexusServer.Data
{
    [Table("conversation")]
    public class Conversation
    {
        [Key,Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        [Key, Required]
        public long userId { get; set; }
        [Required, MaxLength(32)]
        public string title { get; set; }
        [Required]
        public bool waiting { get; set; }

    }
}

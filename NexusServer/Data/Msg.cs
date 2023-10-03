using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace NexusServer.Data
{
    [Table("msg")]
    public class Msg
    {
        [Key,Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        [Required]
        public long conversationId { get; set; } // Foreign key
        [Required]
        public int index { get; set; }
        [Required]
        public bool fromBot { get; set; }
        [Required]
        public string content { get; set; }
        [MaxLength(32)]
        public string? media { get; set; }

    }
}

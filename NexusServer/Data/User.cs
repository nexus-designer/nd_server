using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NexusServer.Data
{
    [Table("user")]
    public class User
    {
        [Required,Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        [Required,MaxLength(32)]
        public string name { get; set; }

        [Required]
        public string email { get; set; }

        [Required,MaxLength(60)]
        public string pwdHash { get; set; }

        [MaxLength(32)]
        public string? sessionToken { get; set; }
    }
}

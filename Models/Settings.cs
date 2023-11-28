using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Settings
    {
        [Required(AllowEmptyStrings = false)]
        public required string? Key { get; set; }

        [Url]
        public required string? Endpoint { get; set; }

        [Required(AllowEmptyStrings = false)]
        public required string? Model { get; set; }
    }
}

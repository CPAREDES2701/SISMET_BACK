using System.ComponentModel.DataAnnotations;

namespace DavisApi.Entidades
{
    public class CredencialesUsuario
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

    }
}

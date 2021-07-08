using System.ComponentModel.DataAnnotations;

namespace Polyclinic
{
    public partial class Пользователи
    {
        [Key]
        public string Логин { get; set; }

        [Required]
        [StringLength(128)]
        public string Пароль { get; set; }

        [Required]
        [StringLength(128)]
        public string Роль { get; set; }
    }
}

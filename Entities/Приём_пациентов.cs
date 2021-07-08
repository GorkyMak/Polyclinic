using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Polyclinic
{
    [Table("Приём пациентов")]
    public partial class Приём_пациентов
    {
        [Key]
        [Column("Код приёма")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Код_приёма { get; set; }

        [Column("Код врача")]
        public int? Код_врача { get; set; }

        [Column("Код пациента")]
        public int? Код_пациента { get; set; }

        [Column("Дата приема", TypeName = "date")]
        public DateTime? Дата_приема { get; set; }

        [Column("Стоимость приема", TypeName = "money")]
        public decimal? Стоимость_приема { get; set; }

        [Column(TypeName = "money")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal? Зарплата { get; set; }

        public virtual Врачи Врачи { get; set; }

        public virtual Пациенты Пациенты { get; set; }
    }
}

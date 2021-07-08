using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Polyclinic
{
    public partial class Пациенты
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Пациенты()
        {
            Приём_пациентов = new HashSet<Приём_пациентов>();
        }

        [Key]
        [Column("Код пациента")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Код_пациента { get; set; }

        [Column("Фамилия пациента")]
        public string Фамилия_пациента { get; set; }

        [Column("Имя пациента")]
        public string Имя_пациента { get; set; }

        [Column("Отчество пациента")]
        public string Отчество_пациента { get; set; }

        [Column("Дата рождения пациента", TypeName = "date")]
        public DateTime? Дата_рождения_пациента { get; set; }

        [Column("Адрес пациента")]
        public string Адрес_пациента { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Приём_пациентов> Приём_пациентов { get; set; }
    }
}

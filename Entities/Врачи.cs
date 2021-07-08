using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Polyclinic
{
    public partial class Врачи
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Врачи()
        {
            Приём_пациентов = new HashSet<Приём_пациентов>();
        }

        [Key]
        [Column("Код врача")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Код_врача { get; set; }

        [Column("Фамилия врача")]
        public string Фамилия_врача { get; set; }

        [Column("Имя врача")]
        public string Имя_врача { get; set; }

        [Column("Отчество врача")]
        public string Отчество_врача { get; set; }

        [Column("Специальность врача")]
        public string Специальность_врача { get; set; }

        [Column("Процент отчисления на зарплату")]
        public int? Процент_отчисления_на_зарплату { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Приём_пациентов> Приём_пациентов { get; set; }
    }
}

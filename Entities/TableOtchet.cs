using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polyclinic.Entities
{
    class TableOtchet
    {
        public int Код_приёма { get; set; }
        public string Фамилия_врача { get; set; }
        public string Имя_врача { get; set; }
        public string Отчество_врача { get; set; }
        public string Специальность_врача { get; set; }
        public int? Процент_отчисления_на_зарплату { get; set; }
        public string Фамилия_пациента { get; set; }
        public string Имя_пациента { get; set; }
        public string Отчество_пациента { get; set; }
        public DateTime? Дата_рождения_пациента { get; set; }
        public string Адрес_пациента { get; set; }
        public DateTime? Дата_приема { get; set; }
        public decimal? Стоимость_приема { get; set; }
        public decimal? Зарплата { get; set; }
    }
}

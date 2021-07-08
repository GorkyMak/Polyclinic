using System.Data.Entity;

namespace Polyclinic
{
    public partial class PolyclinicDBContext : DbContext
    {
        public PolyclinicDBContext()
            : base("name=PolyclinicDBContext")
        {
        }

        public virtual DbSet<Врачи> Врачи { get; set; }
        public virtual DbSet<Пациенты> Пациенты { get; set; }
        public virtual DbSet<Пользователи> Пользователи { get; set; }
        public virtual DbSet<Приём_пациентов> Приём_пациентов { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Приём_пациентов>()
                .Property(e => e.Стоимость_приема)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Приём_пациентов>()
                .Property(e => e.Зарплата)
                .HasPrecision(19, 4);
        }
    }
}

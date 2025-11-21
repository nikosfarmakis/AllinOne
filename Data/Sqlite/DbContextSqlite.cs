using AllinOne.Models.SqliteDatabase;
using AllinOne.Models.SqliteEntities;
using AllinOne.Utils.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AllinOne.Data.Sqlite
{
    public class DbContextSqlite : DbContext
    {
        public DbContextSqlite(DbContextOptions<DbContextSqlite> options) : base(options) 
        { 

        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // converters
            var encryptStringConverter = new ValueConverter<string, string>(
                v => AesGcmEncryptionHelper.Encrypt(v),
                v => AesGcmEncryptionHelper.Decrypt(v)
);

            var encryptIntConverter = new ValueConverter<int, string>(
                v => AesGcmEncryptionHelper.Encrypt(v.ToString()),
                v => int.Parse(AesGcmEncryptionHelper.Decrypt(v))
            );

            var encryptboolConverter = new ValueConverter<bool, string>(
                v => AesGcmEncryptionHelper.Encrypt(v.ToString()),
                v => bool.Parse(AesGcmEncryptionHelper.Decrypt(v))
            );

            #region User

            modelBuilder.Entity<User>().Property(p => p.Role).HasConversion(encryptStringConverter);
            modelBuilder.Entity<User>().Property(p => p.Password).HasConversion(encryptStringConverter);

            #endregion

            #region Person

            modelBuilder.Entity<Person>().Property(p => p.Phone).HasConversion(encryptStringConverter);
            modelBuilder.Entity<Person>().Property(p => p.Email).HasConversion(encryptStringConverter);
            modelBuilder.Entity<Person>().Property(p => p.DateOfBirth).HasConversion(encryptStringConverter);
            // Person.HomeAddress
            modelBuilder.Entity<Person>().OwnsOne(p => p.HomeAddress, ha =>
            {
                ha.Property(ha => ha.Street).HasConversion(encryptStringConverter);
                ha.Property(ha => ha.City).HasConversion(encryptStringConverter);
                ha.Property(ha => ha.PostalCode).HasConversion(encryptStringConverter);
            });

            #endregion

            #region Patient

            modelBuilder.Entity<Patient>().Property(p => p.Notes).HasConversion(encryptStringConverter);

            // Patient.PatientMedicalInfo
            modelBuilder.Entity<Patient>().OwnsOne(p => p.PatientMedicalInfo, pmi =>
            {
                pmi.Property(x => x.DrugAllergiesDescription).HasConversion(encryptStringConverter);
                pmi.Property(x => x.GeneralAllergiesDescription).HasConversion(encryptStringConverter);
                pmi.Property(x => x.ChronicConditions).HasConversion(encryptStringConverter);
                pmi.Property(x => x.PastSurgeries).HasConversion(encryptStringConverter);
                pmi.Property(x => x.AdditionalNotes).HasConversion(encryptStringConverter);
                pmi.Property(x => x.CigarettesPerDay).HasConversion(encryptIntConverter);
                pmi.Property(x => x.DrinksPerWeek).HasConversion(encryptIntConverter);
            });

            #endregion
        }
    }
}

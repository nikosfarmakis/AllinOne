using AllinOne.Models.SqliteDatabase;
using AllinOne.Models.SqliteEntities;
using AllinOne.Utils.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.FeatureManagement;

namespace AllinOne.Data.Sqlite
{
    public class DbContextSqlite : DbContext
    {
        private readonly IFeatureManager _features;
        public DbContextSqlite(DbContextOptions<DbContextSqlite> options,
            IFeatureManager featureManager) : base(options) // EF is synchronous
        {
            _features = featureManager;
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<MedicalInfo> PatientMedicalInfos { get; set; }
        public DbSet<Address> Addresses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            bool encryptionEnabled = _features
                .IsEnabledAsync("DatabaseEncryption")
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            ConfigureRelations(modelBuilder);

            if (encryptionEnabled)
            {
                ConfigureEncryptedModel(modelBuilder);
            }
            else
            {
                ConfigurePlainModel(modelBuilder);
            }
        }
        private void ConfigureRelations(ModelBuilder modelBuilder)
        {
            #region Person
            modelBuilder.Entity<Person>()
                .HasOne(p => p.Address)
                .WithOne(a => a.Person)
                .HasForeignKey<Address>(a => a.PersonId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Patient
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.MedicalInfo)
                .WithOne(a => a.Patient)
                .HasForeignKey<MedicalInfo>(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Address
            modelBuilder.Entity<Address>()
                .HasIndex(a => a.PersonId)
                .IsUnique();
            #endregion

            #region MedicalInfo
            modelBuilder.Entity<MedicalInfo>()
                .HasIndex(m => m.PatientId)
                .IsUnique();
            #endregion
        }
        private void ConfigureEncryptedModel(ModelBuilder modelBuilder)
        {
            #region Person
            modelBuilder.Entity<Person>().Property(p => p.FirstName).HasConversion(CustomValueConverters.EncryptStringConverter);
            modelBuilder.Entity<Person>().Property(p => p.LastName).HasConversion(CustomValueConverters.EncryptStringConverter);
            modelBuilder.Entity<Person>().Property(p => p.Phone).HasConversion(CustomValueConverters.EncryptStringConverter);
            modelBuilder.Entity<Person>().Property(p => p.Email).HasConversion(CustomValueConverters.EncryptStringConverter);
            modelBuilder.Entity<Person>().Property(p => p.DateOfBirth).HasConversion(CustomValueConverters.EncryptDateTimeConverter);
            #endregion

            #region HomeAddress
            modelBuilder.Entity<Address>().Property(ha => ha.Street).HasConversion(CustomValueConverters.EncryptStringConverter);
            modelBuilder.Entity<Address>().Property(ha => ha.City).HasConversion(CustomValueConverters.EncryptStringConverter);
            modelBuilder.Entity<Address>().Property(ha => ha.PostalCode).HasConversion(CustomValueConverters.EncryptStringConverter);
            modelBuilder.Entity<Address>().Property(ha => ha.Country).HasConversion(CustomValueConverters.EncryptStringConverter);
            #endregion

            #region Patient
            modelBuilder.Entity<Patient>().Property(p => p.Notes).HasConversion(CustomValueConverters.EncryptStringConverter);
            modelBuilder.Entity<Patient>().Property(p => p.AMKA).HasConversion(CustomValueConverters.EncryptStringConverter);
            #endregion

            #region PatientMedicalInfo
            //modelBuilder.Entity<MedicalInfo>().Property(x => x.DrugAllergiesDescription).HasConversion(encryptStringConverter);
            //modelBuilder.Entity<MedicalInfo>().Property(x => x.GeneralAllergiesDescription).HasConversion(encryptStringConverter);
            //modelBuilder.Entity<MedicalInfo>().Property(x => x.ChronicConditions).HasConversion(encryptStringConverter);
            //modelBuilder.Entity<MedicalInfo>().Property(x => x.PastSurgeries).HasConversion(encryptStringConverter);
            //modelBuilder.Entity<MedicalInfo>().Property(x => x.AdditionalNotes).HasConversion(encryptStringConverter);
            //modelBuilder.Entity<MedicalInfo>().Property(x => x.CigarettesPerDay).HasConversion(encryptIntConverter);
            //modelBuilder.Entity<MedicalInfo>().Property(x => x.DrinksPerWeek).HasConversion(encryptIntConverter);
            #endregion
        }
        private void ConfigurePlainModel(ModelBuilder modelBuilder)
        {
            //use default non-encrypted mapping
        }

    }
}

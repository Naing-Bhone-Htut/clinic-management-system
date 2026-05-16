using ClinicManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Data;

public class ClinicDbContext : DbContext
{
    public ClinicDbContext(DbContextOptions<ClinicDbContext> options) : base(options) { }

    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Nurse> Nurses => Set<Nurse>();
    public DbSet<OperatorStaff> Operators => Set<OperatorStaff>();
    public DbSet<PatientDocument> PatientDocuments => Set<PatientDocument>();
    public DbSet<PatientCheckIn> PatientCheckIns => Set<PatientCheckIn>();
    public DbSet<VitalSign> VitalSigns => Set<VitalSign>();
    public DbSet<CostRate> CostRates => Set<CostRate>();
    public DbSet<IncomeBill> IncomeBills => Set<IncomeBill>();
    public DbSet<IncomeBillItem> IncomeBillItems => Set<IncomeBillItem>();
    public DbSet<ExpenseBill> ExpenseBills => Set<ExpenseBill>();
    public DbSet<ExpenseBillItem> ExpenseBillItems => Set<ExpenseBillItem>();
    public DbSet<LabRecord> LabRecords => Set<LabRecord>();
    public DbSet<ClinicDocument> ClinicDocuments => Set<ClinicDocument>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<Patient>()
            .HasIndex(p => p.PatientCode)
            .IsUnique();

        modelBuilder.Entity<CostRate>()
            .HasIndex(c => c.CostCode)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasOne(u => u.Person)
            .WithOne(p => p.User)
            .HasForeignKey<User>(u => u.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Patient>()
            .HasOne(p => p.Person)
            .WithOne(p => p.Patient)
            .HasForeignKey<Patient>(p => p.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Doctor>()
            .HasOne(d => d.Person)
            .WithOne(p => p.Doctor)
            .HasForeignKey<Doctor>(d => d.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Nurse>()
            .HasOne(n => n.Person)
            .WithOne(p => p.Nurse)
            .HasForeignKey<Nurse>(n => n.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OperatorStaff>()
            .HasKey(o => o.OperatorId);

        modelBuilder.Entity<OperatorStaff>()
            .HasOne(o => o.Person)
            .WithOne(p => p.Operator)
            .HasForeignKey<OperatorStaff>(o => o.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PatientCheckIn>()
            .HasKey(c => c.CheckInId);

        modelBuilder.Entity<VitalSign>()
            .HasOne(v => v.CheckIn)
            .WithOne(c => c.VitalSign)
            .HasForeignKey<VitalSign>(v => v.CheckInId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Address>()
            .HasOne(a => a.Person)
            .WithMany(p => p.Addresses)
            .HasForeignKey(a => a.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Address>()
            .HasOne(a => a.Patient)
            .WithMany(p => p.Addresses)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

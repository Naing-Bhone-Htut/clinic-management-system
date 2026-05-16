using ClinicManagementSystem.Models;
using ClinicManagementSystem.Models.Enums;
using ClinicManagementSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(ClinicDbContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        // Admin person & user
        var adminPerson = new Person
        {
            FirstName = "System",
            LastName = "Admin",
            Gender = "Male",
            DateOfBirth = new DateTime(1985, 1, 15),
            Phone = "09111111111",
            Email = "admin@clinic.com"
        };

        // Doctor
        var doctorPerson = new Person
        {
            FirstName = "Aung",
            LastName = "Kyaw",
            Gender = "Male",
            DateOfBirth = new DateTime(1980, 5, 20),
            Phone = "09222222222",
            Email = "dr.aung@clinic.com"
        };

        // Nurse
        var nursePerson = new Person
        {
            FirstName = "Su",
            LastName = "Hlaing",
            Gender = "Female",
            DateOfBirth = new DateTime(1990, 8, 10),
            Phone = "09333333333",
            Email = "nurse.su@clinic.com"
        };

        // Operator
        var operatorPerson = new Person
        {
            FirstName = "Min",
            LastName = "Thu",
            Gender = "Male",
            DateOfBirth = new DateTime(1992, 3, 25),
            Phone = "09444444444",
            Email = "operator.min@clinic.com"
        };

        // Patients
        var patient1Person = new Person
        {
            FirstName = "Hla",
            LastName = "Hla",
            Gender = "Female",
            DateOfBirth = new DateTime(1995, 6, 12),
            Phone = "09555555555",
            Email = "hla@email.com"
        };

        var patient2Person = new Person
        {
            FirstName = "Zaw",
            LastName = "Win",
            Gender = "Male",
            DateOfBirth = new DateTime(1988, 11, 3),
            Phone = "09666666666"
        };

        context.Persons.AddRange(adminPerson, doctorPerson, nursePerson, operatorPerson, patient1Person, patient2Person);
        await context.SaveChangesAsync();

        var doctor = new Doctor
        {
            PersonId = doctorPerson.PersonId,
            LicenseNumber = "DOC-001",
            Specialty = "General Medicine",
            Department = "Outpatient"
        };

        var nurse = new Nurse
        {
            PersonId = nursePerson.PersonId,
            LicenseNumber = "NUR-001",
            Department = "Outpatient"
        };

        var op = new OperatorStaff
        {
            PersonId = operatorPerson.PersonId,
            Department = "Reception"
        };

        context.Doctors.Add(doctor);
        context.Nurses.Add(nurse);
        context.Operators.Add(op);

        context.Users.AddRange(
            new User { PersonId = adminPerson.PersonId, Username = "admin", PasswordHash = PasswordHelper.Hash("admin123"), Role = UserRole.Admin },
            new User { PersonId = doctorPerson.PersonId, Username = "doctor", PasswordHash = PasswordHelper.Hash("doctor123"), Role = UserRole.Doctor },
            new User { PersonId = nursePerson.PersonId, Username = "nurse", PasswordHash = PasswordHelper.Hash("nurse123"), Role = UserRole.Nurse },
            new User { PersonId = operatorPerson.PersonId, Username = "operator", PasswordHash = PasswordHelper.Hash("operator123"), Role = UserRole.Operator }
        );

        var patient1 = new Patient
        {
            PersonId = patient1Person.PersonId,
            PatientCode = "PAT-001",
            BloodType = "O+",
            Allergies = "None"
        };

        var patient2 = new Patient
        {
            PersonId = patient2Person.PersonId,
            PatientCode = "PAT-002",
            BloodType = "A+"
        };

        context.Patients.AddRange(patient1, patient2);
        await context.SaveChangesAsync();

        context.Addresses.AddRange(
            new Address { PatientId = patient1.PatientId, AddressLine1 = "No. 10, Main Road", City = "Yangon", State = "Yangon", PostalCode = "11101", Country = "Myanmar" },
            new Address { PatientId = patient2.PatientId, AddressLine1 = "No. 25, University Ave", City = "Mandalay", State = "Mandalay", PostalCode = "05011", Country = "Myanmar" }
        );

        var costRates = new[]
        {
            new CostRate { CostCode = "CONSULT", Description = "Doctor Consultation", CostAmount = 15000, CostType = CostType.Income },
            new CostRate { CostCode = "LAB-BLOOD", Description = "Blood Test", CostAmount = 25000, CostType = CostType.Income },
            new CostRate { CostCode = "MED-SUPPLY", Description = "Medical Supplies", CostAmount = 5000, CostType = CostType.Income },
            new CostRate { CostCode = "RENT", Description = "Clinic Rent", CostAmount = 500000, CostType = CostType.Expense },
            new CostRate { CostCode = "UTIL", Description = "Utilities", CostAmount = 80000, CostType = CostType.Expense },
            new CostRate { CostCode = "SALARY", Description = "Staff Salary", CostAmount = 300000, CostType = CostType.Expense }
        };
        context.CostRates.AddRange(costRates);
        await context.SaveChangesAsync();

        var checkIn = new PatientCheckIn
        {
            PatientId = patient1.PatientId,
            DoctorId = doctor.DoctorId,
            CheckInDateTime = DateTime.Today.AddHours(9),
            IsServed = true,
            Notes = "Regular checkup"
        };
        context.PatientCheckIns.Add(checkIn);
        await context.SaveChangesAsync();

        context.VitalSigns.Add(new VitalSign
        {
            CheckInId = checkIn.CheckInId,
            BloodPressure = "120/80",
            Temperature = 36.6m,
            PulseRate = 72,
            RespiratoryRate = 16,
            Weight = 55,
            Height = 160
        });

        context.LabRecords.Add(new LabRecord
        {
            PatientId = patient1.PatientId,
            CheckInId = checkIn.CheckInId,
            DoctorId = doctor.DoctorId,
            TestName = "Complete Blood Count",
            TestResult = "Normal",
            TestDate = DateTime.Today
        });

        var incomeBill = new IncomeBill
        {
            PatientId = patient1.PatientId,
            CheckInId = checkIn.CheckInId,
            BillDate = DateTime.Today,
            Notes = "Consultation and lab"
        };
        context.IncomeBills.Add(incomeBill);
        await context.SaveChangesAsync();

        var consultRate = costRates[0];
        var labRate = costRates[1];
        context.IncomeBillItems.AddRange(
            new IncomeBillItem { IncomeBillId = incomeBill.IncomeBillId, CostRateId = consultRate.CostRateId, Quantity = 1, UnitPrice = consultRate.CostAmount, LineTotal = consultRate.CostAmount },
            new IncomeBillItem { IncomeBillId = incomeBill.IncomeBillId, CostRateId = labRate.CostRateId, Quantity = 1, UnitPrice = labRate.CostAmount, LineTotal = labRate.CostAmount }
        );
        incomeBill.TotalAmount = consultRate.CostAmount + labRate.CostAmount;

        var expenseBill = new ExpenseBill
        {
            BillDate = DateTime.Today.AddDays(-1),
            Description = "Monthly utilities"
        };
        context.ExpenseBills.Add(expenseBill);
        await context.SaveChangesAsync();

        var utilRate = costRates[4];
        context.ExpenseBillItems.Add(new ExpenseBillItem
        {
            ExpenseBillId = expenseBill.ExpenseBillId,
            CostRateId = utilRate.CostRateId,
            Quantity = 1,
            UnitPrice = utilRate.CostAmount,
            LineTotal = utilRate.CostAmount
        });
        expenseBill.TotalAmount = utilRate.CostAmount;

        await context.SaveChangesAsync();
    }
}

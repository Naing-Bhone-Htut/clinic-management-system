using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagementSystem.Models;

public class Address
{
    public int AddressId { get; set; }

    public int? PersonId { get; set; }
    public Person? Person { get; set; }

    public int? PatientId { get; set; }
    public Patient? Patient { get; set; }

    [Required, StringLength(200)]
    public string AddressLine1 { get; set; } = string.Empty;

    [StringLength(200)]
    public string? AddressLine2 { get; set; }

    [Required, StringLength(100)]
    public string City { get; set; } = string.Empty;

    [StringLength(100)]
    public string? State { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }

    [StringLength(100)]
    public string Country { get; set; } = "Myanmar";

    [StringLength(20)]
    public string AddressType { get; set; } = "Home";
}

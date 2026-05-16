using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models;

public class PatientDocument
{
    public int PatientDocumentId { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    [Required, StringLength(200)]
    public string DocumentName { get; set; } = string.Empty;

    [Required, StringLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [StringLength(50)]
    public string? FileType { get; set; }

    public DateTime UploadedDate { get; set; } = DateTime.UtcNow;

    public int? UploadedByUserId { get; set; }
    public User? UploadedByUser { get; set; }
}

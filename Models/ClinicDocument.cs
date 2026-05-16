using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models;

public class ClinicDocument
{
    public int ClinicDocumentId { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required, StringLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [Required, StringLength(255)]
    public string FileName { get; set; } = string.Empty;

    public DateTime UploadedDate { get; set; } = DateTime.UtcNow;

    public int? UploadedByUserId { get; set; }
    public User? UploadedByUser { get; set; }
}

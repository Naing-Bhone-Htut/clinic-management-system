namespace ClinicManagementSystem.Services;

public class FileUploadService
{
    private readonly IWebHostEnvironment _env;

    public FileUploadService(IWebHostEnvironment env) => _env = env;

    public async Task<string> SaveFileAsync(IFormFile file, string subFolder)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty.");

        var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads", subFolder);
        Directory.CreateDirectory(uploadsRoot);

        var safeName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var physicalPath = Path.Combine(uploadsRoot, safeName);

        await using var stream = new FileStream(physicalPath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/uploads/{subFolder}/{safeName}";
    }
}

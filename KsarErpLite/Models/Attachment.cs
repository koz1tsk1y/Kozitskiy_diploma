namespace KsarErpLite.Models;

public class Attachment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    public required string FilePath { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
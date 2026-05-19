namespace KsarErpLite.Models;

public class Act
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid JobId { get; set; }
    public Job? Job { get; set; }

    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public required string DocumentUrl { get; set; }
}
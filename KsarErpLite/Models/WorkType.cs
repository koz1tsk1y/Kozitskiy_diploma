namespace KsarErpLite.Models;

public class WorkType
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string Unit { get; set; } // Например, км, м2, шт

    public List<Job> Jobs { get; set; } = new();
}
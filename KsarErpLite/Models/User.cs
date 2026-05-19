namespace KsarErpLite.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Login { get; set; }
    public required string PasswordHash { get; set; }
    public required string FullName { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;

    // Навигационные свойства для Entity Framework
    public List<Job> CreatedJobs { get; set; } = new();
    public List<Job> AssignedJobs { get; set; } = new();
}
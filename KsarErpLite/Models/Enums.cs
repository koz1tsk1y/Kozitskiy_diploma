namespace KsarErpLite.Models;

public enum UserRole
{
    Admin,
    Dispatcher,
    Foreman
}

public enum JobStatus
{
    New,
    ProjectLoaded,
    Scheduled,
    InProgress,
    Completed,
    Closed
}
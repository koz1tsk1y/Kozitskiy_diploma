using KsarErpLite.Data;
using KsarErpLite.Models;
using Microsoft.EntityFrameworkCore;

namespace KsarErpLite.Services;

public class JobService : IJobService
{
    private readonly ApplicationDbContext _context;

    public JobService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Job>> GetAllJobsAsync()
    {
        // Подтягиваем связанные данные (тип работы, кто создал, кто исполняет)
        return await _context.Jobs
            .Include(j => j.WorkType)
            .Include(j => j.CreatedBy)
            .Include(j => j.Foreman)
            .AsNoTracking() // Оптимизация для чтения
            .ToListAsync();
    }

    public async Task<Job?> GetJobByIdAsync(Guid id)
    {
        return await _context.Jobs
            .Include(j => j.WorkType)
            .Include(j => j.CreatedBy)
            .Include(j => j.Foreman)
            .Include(j => j.Attachments) // Подтягиваем фотоотчеты
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<Job> CreateJobAsync(Job job)
    {
        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();
        return job;
    }

    public async Task UpdateJobAsync(Job job)
    {
        // Безопасное обновление без конфликтов отслеживания EF Core
        var existingJob = await _context.Jobs.FindAsync(job.Id);
        if (existingJob != null)
        {
            existingJob.PlanDate = job.PlanDate;
            existingJob.Status = job.Status;
            existingJob.TrackLength = job.TrackLength;
            existingJob.KsarXmlRoute = job.KsarXmlRoute;

            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteJobAsync(Guid id)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job != null)
        {
            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<WorkType>> GetWorkTypesAsync()
    {
        return await _context.WorkTypes.AsNoTracking().ToListAsync();
    }

    public async Task<List<User>> GetUsersByRoleAsync(UserRole role)
    {
        return await _context.Users
            .Where(u => u.Role == role)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Job>> GetJobsForPlanningAsync(int year, int month)
    {
        // Создаем границы месяца в UTC (так как PostgreSQL требует UTC)
        var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddMonths(1);

        return await _context.Jobs
            .Include(j => j.WorkType)
            .Include(j => j.Foreman)
            .Where(j => j.PlanDate == null || (j.PlanDate >= startDate && j.PlanDate < endDate))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Job>> GetActiveJobsForForemanAsync()
    {
        return await _context.Jobs
            .Include(j => j.WorkType)
            // Берем только запланированные или уже в работе
            .Where(j => j.Status == JobStatus.Scheduled || j.Status == JobStatus.InProgress)
            .OrderBy(j => j.PlanDate)
            .AsNoTracking()
            .ToListAsync();
    }
}
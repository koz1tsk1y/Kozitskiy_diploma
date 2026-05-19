using KsarErpLite.Models;

namespace KsarErpLite.Services;

public interface IJobService
{
    // Получить все заявки (для списка и календаря)
    Task<List<Job>> GetAllJobsAsync();

    // Получить конкретную заявку со всеми связями
    Task<Job?> GetJobByIdAsync(Guid id);

    // Создать новую заявку
    Task<Job> CreateJobAsync(Job job);

    // Обновить заявку (например, сменить статус или добавить трек КСАР)
    Task UpdateJobAsync(Job job);

    // Удалить заявку
    Task DeleteJobAsync(Guid id);
}
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

    // Получить все типы работ для выпадающего списка
    Task<List<WorkType>> GetWorkTypesAsync();

    // Получить список пользователей по роли (чтобы назначить создателя/бригаду)
    Task<List<User>> GetUsersByRoleAsync(UserRole role);

    // Получить заявки для календаря (без даты + на конкретный месяц)
    Task<List<Job>> GetJobsForPlanningAsync(int year, int month);

    // Получить активные задания для мобильного приложения бригадира
    Task<List<Job>> GetActiveJobsForForemanAsync();

    // Аналитика для главного экрана
    Task<Dictionary<JobStatus, int>> GetJobsStatisticsAsync();
}
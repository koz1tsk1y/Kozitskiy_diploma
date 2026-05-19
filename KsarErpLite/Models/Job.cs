using System.Net.Mail;

namespace KsarErpLite.Models;

public class Job
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Customer { get; set; }

    public Guid WorkTypeId { get; set; }
    public WorkType? WorkType { get; set; }

    public DateTime? PlanDate { get; set; }
    public JobStatus Status { get; set; } = JobStatus.New;

    // Поля для интеграции с КСАР и картами
    public string? KsarXmlRoute { get; set; } // Путь к прикрепленному файлу из Credo
    public string? GeoDataJson { get; set; } // JSON координат для Leaflet
    public double? TrackLength { get; set; } // Расчетная длина трека

    public string? MaterialsFact { get; set; } // Фактический расход

    public Guid CreatedById { get; set; }
    public User? CreatedBy { get; set; }

    public Guid? ForemanId { get; set; }
    public User? Foreman { get; set; }

    // Навигационные свойства
    public List<Attachment> Attachments { get; set; } = new();
    public Act? Act { get; set; }
}
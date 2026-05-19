namespace KsarErpLite.Models;

public class KsarTrackData
{
    // Основная линия (маршрут машины)
    public List<GeoPoint> TrackPoints { get; set; } = new();

    // Дорожные знаки
    public List<GeoPoint> SingleSigns { get; set; } = new();

    // Остановки общественного транспорта
    public List<GeoPoint> TransportStops { get; set; } = new();
}

// Запись для хранения одной координаты (Широта, Долгота)
public record GeoPoint(double Lat, double Lon);
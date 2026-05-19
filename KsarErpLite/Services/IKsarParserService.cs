using KsarErpLite.Models;

namespace KsarErpLite.Services;

public interface IKsarParserService
{
    // Метод принимает поток файла (Stream) и возвращает готовую модель данных
    Task<KsarTrackData> ParseXmlAsync(Stream xmlStream);
}
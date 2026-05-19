using System.Globalization;
using System.Xml.Linq;
using KsarErpLite.Models;

namespace KsarErpLite.Services;

public class KsarParserService : IKsarParserService
{
    public async Task<KsarTrackData> ParseXmlAsync(Stream xmlStream)
    {
        var trackData = new KsarTrackData();

        // Загружаем XML асинхронно
        var doc = await XDocument.LoadAsync(xmlStream, LoadOptions.None, CancellationToken.None);

        var recordingArea = doc.Descendants("RecordingArea").FirstOrDefault();
        if (recordingArea == null) return trackData;

        // 1. Парсинг основного трека (TreckPoints)
        var pointsAttribute = recordingArea.Element("TreckPoints")?.Attribute("Points")?.Value;
        if (!string.IsNullOrEmpty(pointsAttribute))
        {
            // Точки разделены точкой с запятой, а внутри широта и долгота разделены запятой
            var pointPairs = pointsAttribute.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in pointPairs)
            {
                var coords = pair.Split(',');
                if (coords.Length == 2 &&
                    double.TryParse(coords[0], CultureInfo.InvariantCulture, out double lat) &&
                    double.TryParse(coords[1], CultureInfo.InvariantCulture, out double lon))
                {
                    trackData.TrackPoints.Add(new GeoPoint(lat, lon));
                }
            }
        }

        // 2. Парсинг дорожных знаков (SingleSigns)
        var singleSigns = recordingArea.Descendants("SingleSign");
        foreach (var sign in singleSigns)
        {
            var latStr = sign.Attribute("lat")?.Value;
            var lonStr = sign.Attribute("lon")?.Value;

            if (double.TryParse(latStr, CultureInfo.InvariantCulture, out double lat) &&
                double.TryParse(lonStr, CultureInfo.InvariantCulture, out double lon))
            {
                trackData.SingleSigns.Add(new GeoPoint(lat, lon));
            }
        }

        // 3. Парсинг остановок (TransportStops)
        var transportStops = recordingArea.Descendants("TransportStop");
        foreach (var stop in transportStops)
        {
            var latStr = stop.Attribute("lat")?.Value;
            var lonStr = stop.Attribute("lon")?.Value;

            if (double.TryParse(latStr, CultureInfo.InvariantCulture, out double lat) &&
                double.TryParse(lonStr, CultureInfo.InvariantCulture, out double lon))
            {
                trackData.TransportStops.Add(new GeoPoint(lat, lon));
            }
        }

        return trackData;
    }
}
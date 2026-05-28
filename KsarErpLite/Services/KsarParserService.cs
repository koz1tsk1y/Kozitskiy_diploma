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

		// 1. Парсинг основного трека (TreckPoints) — Адаптивный метод
		var treckPointsElement = recordingArea.Element("TreckPoints");
		if (treckPointsElement != null)
		{
			// Поддержка обеих версий прошивок (с большой и маленькой буквы)
			var pointsAttribute = treckPointsElement.Attribute("Points")?.Value
							   ?? treckPointsElement.Attribute("points")?.Value;

			if (!string.IsNullOrEmpty(pointsAttribute))
			{
				// Точки разделены точкой с запятой
				var pointPairs = pointsAttribute.Split(';', StringSplitOptions.RemoveEmptyEntries);
				foreach (var pair in pointPairs)
				{
					// Внутри значения разделены запятой
					var coords = pair.Split(',');

					double lat = 0, lon = 0;
					bool isValid = false;

					// Формат 1 (Старый): "Широта,Долгота" (Например: 52.055228,23.754268)
					if (coords.Length == 2)
					{
						isValid = double.TryParse(coords[0], NumberStyles.Any, CultureInfo.InvariantCulture, out lat) &&
								  double.TryParse(coords[1], NumberStyles.Any, CultureInfo.InvariantCulture, out lon);
					}
					// Формат 2 (Новый): "Дата Время,Широта,Долгота,Высота,Параметр" 
					// (Например: 23.04.2026 12:13:24,52.074517,23.691870,138.2898,4)
					else if (coords.Length >= 4)
					{
						// В новом формате широта идет под индексом 1, а долгота под индексом 2
						isValid = double.TryParse(coords[1], NumberStyles.Any, CultureInfo.InvariantCulture, out lat) &&
								  double.TryParse(coords[2], NumberStyles.Any, CultureInfo.InvariantCulture, out lon);
					}

					if (isValid && lat != 0 && lon != 0)
					{
						trackData.TrackPoints.Add(new GeoPoint(lat, lon));
					}
				}
			}
		}

		// 2. Парсинг дорожных знаков (SingleSigns)
		var singleSigns = recordingArea.Descendants("SingleSign");
		foreach (var sign in singleSigns)
		{
			var latStr = sign.Attribute("lat")?.Value;
			var lonStr = sign.Attribute("lon")?.Value;

			if (double.TryParse(latStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double lat) &&
				double.TryParse(lonStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double lon))
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

			if (double.TryParse(latStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double lat) &&
				double.TryParse(lonStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double lon))
			{
				trackData.TransportStops.Add(new GeoPoint(lat, lon));
			}
		}

		return trackData;
	}
}
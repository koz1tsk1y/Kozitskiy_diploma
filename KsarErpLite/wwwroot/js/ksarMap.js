window.ksarMap = {
    map: null,
    trackLayer: null,
    markersLayer: null,

    // Инициализация карты в конкретном div
    init: function (elementId) {
        if (this.map) {
            this.map.remove(); // Очищаем старую карту, если она была
        }

        // Координаты Бреста по умолчанию
        this.map = L.map(elementId).setView([52.097, 23.734], 11);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '© OpenStreetMap'
        }).addTo(this.map);

        this.trackLayer = L.layerGroup().addTo(this.map);
        this.markersLayer = L.layerGroup().addTo(this.map);
    },

    // Отрисовка распарсенного трека КСАР
    drawKsarTrack: function (trackData) {
        if (!this.map) return;

        // Очищаем старые слои
        this.trackLayer.clearLayers();
        this.markersLayer.clearLayers();

        // 1. Рисуем полилинию (маршрут разметки)
        if (trackData.trackPoints && trackData.trackPoints.length > 0) {
            var latlngs = trackData.trackPoints.map(p => [p.lat, p.lon]);
            var polyline = L.polyline(latlngs, { color: 'blue', weight: 4 }).addTo(this.trackLayer);

            // Фокусируем камеру на нарисованном треке
            this.map.fitBounds(polyline.getBounds());
        }

        // 2. Рисуем дорожные знаки (красные кружки для примера)
        if (trackData.singleSigns && trackData.singleSigns.length > 0) {
            trackData.singleSigns.forEach(p => {
                L.circleMarker([p.lat, p.lon], { color: 'red', radius: 5 })
                    .bindPopup("Дорожный знак")
                    .addTo(this.markersLayer);
            });
        }

        // 3. Рисуем остановки (зеленые кружки)
        if (trackData.transportStops && trackData.transportStops.length > 0) {
            trackData.transportStops.forEach(p => {
                L.circleMarker([p.lat, p.lon], { color: 'green', radius: 5 })
                    .bindPopup("Остановка общественного транспорта")
                    .addTo(this.markersLayer);
            });
        }
    }
};
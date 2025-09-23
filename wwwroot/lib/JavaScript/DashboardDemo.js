
var map = L.map('map').setView([37.8, -96], 4);
var currentInterval = 1;

var LightGrayBase = L.tileLayer('https://services.arcgisonline.com/arcgis/rest/services/Canvas/World_Light_Gray_Base/MapServer/tile/{z}/{y}/{x}', {
    minZoom: 0,
    maxZoom: 20,
    attribution: '&copy; CNES, Distribution Airbus DS, © Airbus DS, © PlanetObserver (Contains Copernicus Data) | &copy; <a href="https://www.stadiamaps.com/" target="_blank">Stadia Maps</a> &copy; <a href="https://openmaptiles.org/" target="_blank">OpenMapTiles</a> &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
    ext: 'jpg'
});

var OpenStreetMap = L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 19,
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
});

var WorldImagery = L.tileLayer('https://services.arcgisonline.com/arcgis/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}', {
    minZoom: 0,
    maxZoom: 20,
    ext: 'png'
}).addTo(map);

var info = L.control();

function getColorCounty(totalDecs, totalDecsWithCrops) {
    return totalDecs > 0 && totalDecsWithCrops > 0 ? 'red' :
        totalDecs > 0 ? '#5E87E8' :
            '#FFFFFF00';
}

function countyStyle(feature) {
    return {
        fillColor: getColorCounty(feature.properties.TotalPresDecs, feature.properties.DecsWithCrops),
        weight: .6,
        opacity: .5,
        color: "black",
        fillOpacity: 0.7
    };
}


function getColor(d) {
    return d >= (4 * currentInterval) ? '#011a08' :
        d > (3 * currentInterval) ? '#3d9137' :
            d > (2 * currentInterval) ? '#88d669' :
                (d > (1 * currentInterval) || d > 0) ? '#b2ed9a' :
                    '#FFFFFF00';
}

function style(feature) {
    return {
        fillColor: getColor(feature.properties.TotalPresDecs),
        weight: 2,
        weight: 1.5,
        opacity: 1,
        color: "black",
        fillOpacity: 0.7
    };
}

var popup = L.popup();



function GetMapData(year, type, typeDesc, disasterInt) {
    $.get("/api/GetCountyGeoJson", { year: year, type: type }, function (apiCountyData, status) {
        $.get("/api/GetStateGeoJson", { year: year, type: type }, function (apiStateData, status) {
            $.get("/api/GetFireData", { year: year }, function (apiFirePointsData, status) {

                statesData = JSON.parse(apiStateData);
                countyData = JSON.parse(apiCountyData);
                fireData = JSON.parse(apiFirePointsData);
                currentInterval = disasterInt;

                geojsonStates = L.geoJson(statesData, {
                    style: style,
                    onEachFeature: function (feature, layer) {
                        if (feature.properties) {
                            var props = feature.properties;
                            var content = '<div style="font-size:12px;padding-bottom:10px;">' + 'US Secretary of Ag' + '</div>' + (props ?
                                '<div><b>State: </b>' + props.name
                                + '<div><b>Total Emergency Declarations: </b>' + props.TotalPresDecs + '</div><div>'
                                + '<div><b>Declared Disasters: </b>' + props.ListOfDisasters + '</div><div>'
                                + (props.DecsWithCrops ? '<div><b>Total crop production types: </b>' + props.DecsWithCrops + '</div><div>'
                                    + '<div><b>Crop Details: </b><small>' + props.CropDetailList + '</small></div><div>'
                                    : '')
                                : feature.properties.name);
                            layer.bindPopup(content);
                        }
                    }
                }).addTo(map);


                geojsonCounties = L.geoJson(countyData, {
                    style: countyStyle
                    , onEachFeature: function (feature, layer) {
                        if (feature.properties) {
                            var props = feature.properties;
                            var content = '<div style="font-size:12px;padding-bottom:10px;">' + 'US Secretary of Ag' + '</div>' + (props ?
                                '<div><b>County/City/Place: </b>' + props.name
                                + '<div><b>Total Emergency Declarations: </b>' + props.TotalPresDecs + '</div><div>'
                                + '<div><b>Declared Disasters: </b>' + props.ListOfDisasters + '</div><div>'
                                + (props.DecsWithCrops ? '<div><b>Total crop production types: </b>' + props.DecsWithCrops + '</div><div>'
                                    + '<div><b>Crop Details: </b><small>' + props.CropDetailList + '</small></div><div>'
                                    : '')
                                : feature.properties.name);
                            layer.bindPopup(content);
                        }
                    }
                }).addTo(map);

                geojsonFirePoints = L.geoJSON(fireData, {
                    onEachFeature: function (feature, layer) {
                        if (feature.properties) {
                            var content = '<div><b>Fire Name: </b>' + feature.properties.FIRENAME + '</div>' +
                                '<div><b>Fire Year: </b>' + feature.properties.FIREYEAR + '</div>' +
                                '<div><b>Fire Discover Date: </b>' + feature.properties.DISCOVERYDATETIME + '</div>' +
                                '<div><b>Fire Out Date: </b>' + feature.properties.FIREOUTDATETIME + '</div>' +
                                '<div><b>Acres Burned: </b>' + feature.properties.TOTALACRES + '</div>' +
                                '<div><b>Fire Cause: </b>' + feature.properties.STATCAUSE + '</div>'; 
                            layer.bindPopup(content);
                        }
                    },
                    pointToLayer: function (feature, latlng) {
                        // You can customize the appearance of the CircleMarker here
                        return new L.CircleMarker(latlng, {
                            radius: 7, // Set the radius in pixels
                            fillColor: "#FDA50F",
                            color: "#000",
                            weight: 1,
                            opacity: 1,
                            fillOpacity: 0.8
                        });
                    },
                }).addTo(map);

                var baseMaps = {
                    "LightGrayBase": LightGrayBase,
                    "World Imagery": WorldImagery,
                    "Open Street Map": OpenStreetMap
                };

                var overlayMaps = {
                    "States": geojsonStates,
                    "Counties": geojsonCounties,
                    "Fire Points": geojsonFirePoints
                };

                var layerControl = L.control.layers(baseMaps, overlayMaps);

                info.onAdd = function (map) {
                    this._div = L.DomUtil.create('div', 'info');
                    this.update();
                    return this._div;
                };

                var legend = L.control({ position: 'bottomleft' });
                legend.onAdd = function (map) {

                    var div = L.DomUtil.create('div', 'info legend'),
                        grades = [(1 * disasterInt), (2 * disasterInt), (3 * disasterInt), (4 * disasterInt)],
                        labels = [];

                    div.innerHTML += '<div>State Disasters</div>'

                    for (var i = 0; i < grades.length; i++) {
                        div.innerHTML +=
                            '<i style="background:' + getColor(grades[i] + 1) + '"></i> ' +
                            grades[i] + (grades[i + 1] ? (disasterInt > 1 ? (' - ' + (grades[i + 1] - 1)) : '') + '<br>' : '+');
                    }

                    div.innerHTML += '<div style="padding-top:10px;">County Level (2022 Census)</div>';
                    div.innerHTML += '<i style="background:red"></i>Crop Production <br>';
                    div.innerHTML += '<i style="background:#5E87E8"></i>No Crop Production<br>';

                    div.innerHTML += '<div class="row mt-2"> <div class="col-2"><div class="circle"></div></div> <div class="col-10">USFS Fire Orgins</div> </div>'

                    return div;
                };

                layerControl.addTo(map);
                legend.addTo(map);
            });

        });
    });
}


function SetChartVisuals(year, type, typeDesc, disasterInt) {
    SetRadarCharts();
    SetLineChart();
}

function SetRadarCharts() {
    SetRadarChartData('ussec', 'myChart');
    SetRadarChartData('pres', 'myChart2');
}



function SetRadarChartData(type, chartName) {
    $.get('/api/GetRadarData', { type: type }, function (apiRadarData, status) {
        radarData = JSON.parse(apiRadarData);
        var wildfires = [];
        var hurricanes = [];
        var storms = [];
        var winter = [];
        var chartLabels = [];

        for (let i = 0; i < radarData.length; i++) {
            if (chartLabels.includes(radarData[i].Year) === false)
                chartLabels[i] = radarData[i].Year;
            if (radarData[i].Type == 'wildfires')
                wildfires[wildfires.length] = radarData[i].Total;
            else if (radarData[i].Type == 'hurricane')
                hurricanes[hurricanes.length] = radarData[i].Total;
            else if (radarData[i].Type == 'tornadoes, hail, flooding, storm, landslides')
                storms[storms.length] = radarData[i].Total;
            else if (radarData[i].Type == 'frost, freeze, snow, ice')
                winter[winter.length] = radarData[i].Total;
        }

        const ctx = document.getElementById(chartName);

        const data = {
            labels: chartLabels,
            datasets: [{
                label: 'Wildfires',
                data: wildfires,
                fill: true,
                backgroundColor: 'rgba(252, 3, 40, 0.5)',
                borderColor: 'rgb(252, 3, 40)',
                pointBackgroundColor: 'rgb(252, 3, 40)',
                pointBorderColor: '#fff',
                pointHoverBackgroundColor: '#fff',
                pointHoverBorderColor: 'rgb(252, 3, 40)'
            },
            {
                label: 'Hurricane',
                data: hurricanes,
                fill: true,
                backgroundColor: 'rgba(31, 171, 24, 0.6)',
                borderColor: 'rgb(31, 171, 24)',
                pointBackgroundColor: 'rgb(31, 171, 24)',
                pointBorderColor: '#fff',
                pointHoverBackgroundColor: '#fff',
                pointHoverBorderColor: 'rgb(31, 171, 24)'
            },
            {
                label: 'Winter',
                data: winter,
                fill: true,
                backgroundColor: 'rgba(24, 105, 171, 0.6)',
                borderColor: 'rgb(24, 105, 171)',
                pointBackgroundColor: 'rgb(24, 105, 171)',
                pointBorderColor: '#fff',
                pointHoverBackgroundColor: '#fff',
                pointHoverBorderColor: 'rgb(24, 105, 171)'
            },
            {
                label: 'Storms',
                data: storms,
                fill: true,
                backgroundColor: 'rgba(235, 174, 52, 0.6)',
                borderColor: 'rgb(235, 174, 52)',
                pointBackgroundColor: 'rgb(235, 174, 52)',
                pointBorderColor: '#fff',
                pointHoverBackgroundColor: '#fff',
                pointHoverBorderColor: 'rgb(235, 174, 52)'
            }
            ]
        };

        var chart1 = new Chart(ctx, {
            type: 'radar',
            data: data,
            options: {
                responsive: true,
                maintainAspectRatio: false,
                elements: {
                    line: {
                        borderWidth: 3
                    }
                },
                scales: {
                    r: {
                        pointLabels: {
                            color: 'white' // Change point label color to red
                        },
                        grid:
                        {
                            color: 'white'
                        },
                        ticks:
                        {
                            color: 'white',
                            backdropColor: '#212529'
                        },
                        angleLines:
                        {
                            color: 'white'
                        }
                    }
                },
                plugins: {
                    legend: {
                        display: true,
                        position: 'bottom',
                        labels: {
                            color: 'white'
                        }
                    }
                }
            }
        });

    });


}

function SetLineChart() {

    $.get('/api/GetLineChartData', function (apiLineData, status) {

        lineData = JSON.parse(apiLineData);
        var ussec = [];
        var pres = [];
        var chartLabels = [];

        for (let i = 0; i < lineData.length; i++) {
            if (chartLabels.includes(lineData[i].Year) === false)
                chartLabels[i] = lineData[i].Year;
            if (lineData[i].Type == 'ussec')
                ussec[ussec.length] = lineData[i].Total;
            else if (lineData[i].Type == 'pres')
                pres[pres.length] = lineData[i].Total;
        }

        const ctx = document.getElementById('myLineChart');
        const data = {
            labels: chartLabels,
            datasets: [
                {
                    label: 'Presidential',
                    data: pres,
                    fill: true,
                    backgroundColor: 'rgba(31, 171, 24, 0.5)',
                    borderColor: 'rgb(31, 171, 24)',
                    tension: 0.3
                },
                {
                    label: 'US Secretary of Ag',
                    data: ussec,
                    fill: true,
                    backgroundColor: 'rgba(24, 105, 171, 0.5)',
                    borderColor: 'rgb(24, 105, 171)',
                    tension: 0.3
                }]
        };

        new Chart(ctx, {
            type: 'line',
            data: data,
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: true,
                        position: 'bottom',
                        labels: {
                            color: 'white'
                        }
                    }
                },
                scales: {
                    x: {
                        grid: {
                            color: 'white',
                        },
                        ticks: {
                            color: 'white'
                        }
                    },
                    y: {
                        grid: {
                            color: 'white',
                        },
                        ticks: {
                            color: 'white'
                        }
                    }
                }
            }
        });
    });
}

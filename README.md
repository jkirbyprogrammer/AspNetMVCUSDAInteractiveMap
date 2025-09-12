# ASP.NET Core 8.0 MVC USDA Interactive Map
Dashboard example using ASP.NET Core, C#, JavaScript, HTML, CSS, Bootstrap, JQuery, Chart.js, custom API calls, Leaflet, GeoJSON, and more. Example uses public data from USDA, since they do not have any interactive maps like this. This uses data from 2022 crop census, Disaster Designation Information Made By the US Secretary of Agriculture, Presidential Emergency Declarations, all linked together in a clean interactive web map at county & state level. 

- Project uses data that was pulled from USDA from 2018 to mid 2025: 
  - https://www.fsa.usda.gov/resources/disaster-assistance-program/disaster-designation-information
  - USDA quick stats
- Public version of this dashboard:
  - [Link to dashboard](https://jordankirbyaspnetcore-dashboard-b4hgewasfycpc0a6.centralus-01.azurewebsites.net/Home/DashboardDemo) 
- Application Details:
  - ASP.NET Core v8
  - C# v12
  - JQuery v3.6.0
  - Bootstrap v5.1.0
  - Leaflet v1.9.4
  - Chart.js v4.5.0
  - GeoJSON.NET v1.4.1
  - NetTopologySuite v2.6.0
  - NetTopologySuite.IO.GeoJSON v4.0.0
  - CsvHelper v33.1.0
  - CSS
  - JavaScript
  - HTML5
- NOTE: The API calls included in this project include ones used to create the jSON for both Chart.js and Leaflet map. In an Enterprise environment these API calls would be best set up in microservice architecture using RESTful API calls.
- I will have 2 more code repositories soon for this same example. One will be a React version and the other will be in Angular. 


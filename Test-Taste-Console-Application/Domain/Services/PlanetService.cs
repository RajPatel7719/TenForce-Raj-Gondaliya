using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using Test_Taste_Console_Application.Constants;
using Test_Taste_Console_Application.Domain.DataTransferObjects;
using Test_Taste_Console_Application.Domain.DataTransferObjects.JsonObjects;
using Test_Taste_Console_Application.Domain.Objects;
using Test_Taste_Console_Application.Domain.Services.Interfaces;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application.Domain.Services
{
    /// <inheritdoc />
    public class PlanetService : IPlanetService
    {
        private readonly HttpClientService _httpClientService;

        // Dependency injection of HttpClientService
        public PlanetService(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        // Method to get all planets with their moons
        public IEnumerable<Planet> GetAllPlanets()
        {
            // Initialize an empty collection to store planets
            var allPlanetsWithTheirMoons = new Collection<Planet>();

            // Send a GET request to the API to retrieve all planets with their moons
            var response = _httpClientService.Client
                .GetAsync(UriPath.GetAllPlanetsWithMoonsQueryParameters)
                .Result;

            //If the status code isn't 200-299, then the function returns an empty collection.
            if (!response.IsSuccessStatusCode)
            {
                // Log a warning if the request failed
                Logger.Instance.Warn($"{LoggerMessage.GetRequestFailed}{response.StatusCode}");
                return allPlanetsWithTheirMoons;
            }

            // Read the response content as a string
            var content = response.Content.ReadAsStringAsync().Result;

            //The JSON converter uses DTO's, that can be found in the DataTransferObjects folder, to deserialize the response content.
            var results = JsonConvert.DeserializeObject<JsonResult<PlanetDto>>(content);

            //The JSON converter can return a null object. 
            if (results == null) return allPlanetsWithTheirMoons;

            //If the planet doesn't have any moons, then it isn't added to the collection.
            // Iterate through each planet in the results
            foreach (var planet in results.Bodies)
            {
                // Check if the planet has moons
                if (planet.Moons != null)
                {
                    // Initialize a new collection to store the moons
                    var newMoonsCollection = new Collection<MoonDto>();

                    // Iterate through each moon of the planet
                    foreach (var moon in planet.Moons)
                    {
                        // Send a GET request to the API to retrieve the moon's details
                        var moonResponse = _httpClientService.Client
                           .GetAsync(UriPath.GetMoonByIdQueryParameters + moon.URLId)
                           .Result;

                        // Read the response content as a string
                        var moonContent = moonResponse.Content.ReadAsStringAsync().Result;

                        // Deserialize the JSON response into a MoonDto object
                        newMoonsCollection.Add(JsonConvert.DeserializeObject<MoonDto>(moonContent));
                    }

                    // Assign the new collection of moons to the planet
                    planet.Moons = newMoonsCollection;
                }

                // Create a new Planet object from the PlanetDto
                var newPlanet = new Planet(planet);

                // Calculate the average moon gravity for the planet
                newPlanet.CalculateAverageMoonGravity();

                // Add the planet to the collection
                allPlanetsWithTheirMoons.Add(newPlanet);
            }

            // Return the collection of planets
            return allPlanetsWithTheirMoons;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }
    }
}

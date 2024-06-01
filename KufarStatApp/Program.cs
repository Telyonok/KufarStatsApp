using KufarStatApp.HelperServices;
using KufarStatApp.Models;
using Newtonsoft.Json;
using System.Globalization;

namespace KufarStatQueries
{
    public class Program
    {
        /// <summary>
        /// Gets a list of flats that are inside of a provided shape defined by points.
        /// </summary>
        public static List<Ad> GetFlatsInsideShape(FlatSearchResponse flatsForSaleResponse, List<Point> shapePoints)
        {
            // Check if the provided figure is valid.
            if (flatsForSaleResponse == null || flatsForSaleResponse.Ads == null || !Geometry.IsValidFigure(shapePoints)) 
                return new List<Ad>();

            var flatsInsideShape = new List<Ad>();

            foreach (Ad flat in flatsForSaleResponse.Ads)
            {
                // Extract flat's coordinates.
                object coordinatesObject = flat.AdParameters.First(x => x.PropertyName == "coordinates").PropertyValue;
                var coordinatesStringList = Convert.ToString(coordinatesObject).Trim('[', ']').Split(',');
                var latitude = Convert.ToDouble(coordinatesStringList[0].Trim(), CultureInfo.InvariantCulture);
                var longitude = Convert.ToDouble(coordinatesStringList[1].Trim(), CultureInfo.InvariantCulture);

                // Check if the coordinates are inside the shape.
                if (Geometry.IsPointInsidePolygon(new Point(latitude, longitude), shapePoints))
                    flatsInsideShape.Add(flat);
            }

            return flatsInsideShape;
        }

        /// <summary>
        /// Gets a dictionary where keys are different unique values of provided criteria (e.g. rooms, metro, floor) and values are an average of prices per square meter.
        /// </summary>
        static Dictionary<string, double> GetPriceCorrelationByProperty(FlatSearchResponse? adsResponse, string propertyName)
        {
            if (adsResponse == null || adsResponse.Ads == null)
                return new Dictionary<string, double>();

            var adsByProperty = adsResponse.Ads.GroupBy(ad => ad.AdParameters.FirstOrDefault(x => x.PropertyName == propertyName)?.Value.ToString());

            var propertyToUsdPerMeter = new Dictionary<string, double>();

            foreach (var propertyGroup in adsByProperty)
            {
                if (propertyGroup.Key == null)
                    continue;

                var propertyValue = propertyGroup.Key.ToString().Trim('[', ']').Trim();

                if (propertyGroup.Any(ad => Convert.ToDouble(ad.AdParameters.FirstOrDefault(x => x.PropertyName == "square_meter")?.PropertyValue) > 100))
                {
                    propertyToUsdPerMeter.Add(propertyValue, propertyGroup
                        .Where(ad => Convert.ToDouble(ad.AdParameters.FirstOrDefault(x => x.PropertyName == "square_meter")?.PropertyValue) > 100)
                        .Average(ad => Convert.ToDouble(ad.AdParameters.FirstOrDefault(x => x.PropertyName == "square_meter")?.PropertyValue)));
                }
            }

            return propertyToUsdPerMeter;
        }

        /// <summary>
        /// Gets a list of flats for available for daily rent that suit provided parameters.
        /// </summary>
        /// <remarks>
        /// For whatever reason booking calendar provided by API and booking calendar as seen on the website itself differ. Confusing.
        /// </remarks>
        public static List<Ad> GetFlatsByBookingParams(FlatSearchResponse flatForRentResponse, PropertyBookingParameters bookingParameters)
        {
            var suitableFlats = new List<Ad>();

            foreach (var flat in flatForRentResponse.Ads)
            {
                // Check if the ad has the required parameters
                if (flat.AdParameters.Any(x => x.PropertyName == "area" && x.Value.ToString() == bookingParameters.Area)
                    && flat.AdParameters.Any(x => x.PropertyName == "booking_enabled" && x.PropertyValue.ToString() == "True")
                    && flat.AdParameters.Any(x => x.PropertyName == "booking_calendar"))
                {
                    // Get the booking calendar and convert it to a list of DateTime objects. Kufar returns dates as the amount of days since 1.1.1970.
                    var bookingCalendar = flat.AdParameters.First(x => x.PropertyName == "booking_calendar").PropertyValue.ToString().Trim('[',']').Split(',').Select(int.Parse).Select(daysSince1970 => new DateTime(1970, 1, 1).AddDays(daysSince1970 + 1)).ToList();

                    // Check if the booking calendar is valid and includes the entry and exit dates.
                    if (bookingCalendar.Any() && bookingCalendar.Contains(bookingParameters.EntryDate) && bookingCalendar.Contains(bookingParameters.ExitDate))
                    {
                        var noDateGaps = false;
                        // Check if the booking calendar also contains all the dates between EntryDate and ExitDate so there are no gaps.
                        for (var date = bookingParameters.EntryDate.AddDays(1); date <= bookingParameters.ExitDate.AddDays(-1); date = date.AddDays(1))
                        {
                            if (!bookingCalendar.Contains(date))
                            {
                                noDateGaps = false;
                                break;
                            }
                            noDateGaps = true;
                        }

                        if (noDateGaps)
                            suitableFlats.Add(flat);
                    }
                }
            }

            return suitableFlats.OrderByDescending(x => Convert.ToDouble(x.PriceUsd)).ToList(); // Descending sort by USD price.
        }

        /// <summary>
        /// Prints out results of GetCorrelationByProperty method.
        /// </summary>
        /// <remarks>
        /// The task to show correlation is a bit vague by itself. So I chose to show an average price for each unique encountered value.
        /// </remarks>
        static void PrintCorrelation(Dictionary<string, double> correlation, string propertyLabel)
        {
            foreach (var property in correlation.Keys)
                Console.WriteLine($"{propertyLabel} {property}: Average USD/m2 = {correlation[property]}");
        }

        /// <summary>
        /// Gets response from Kufar, returns result as FlatSearchResponse object.
        /// </summary>
        /// <remarks>
        /// Urls are usually to be stored in configuration files, but I don't want to overcomplicate it so I left it here.
        /// </remarks>
        public static async Task<FlatSearchResponse> GetFlatSearchResponse(KufarSearchQueryParameters parameters)
        {
            string apiUrl = $"https://api.kufar.by/search-api/v2/search/rendered-paginated?cat={parameters.Category}&cur={parameters.Currency}&gtsy={parameters.GeoTarget}&lang={parameters.Language}&size={parameters.Size}&typ={parameters.Type}";

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(apiUrl);
                string apiResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<FlatSearchResponse>(apiResponse);
                if (result == null)
                    return new FlatSearchResponse();
                return result;
            }
        }

        static async Task Main(string[] args)
        {
            // First we get response from Kufar.
            var flatsForSaleResponse = await GetFlatSearchResponse(new KufarSearchQueryParameters());

            var floorToUsdPerMeter = GetPriceCorrelationByProperty(flatsForSaleResponse, "floor");
            var roomCountToUsdPerMeter = GetPriceCorrelationByProperty(flatsForSaleResponse, "rooms");
            var closestMetroStationToUsdPerMeter = GetPriceCorrelationByProperty(flatsForSaleResponse, "metro");

            Console.WriteLine("Floor to USD per meter:");
            PrintCorrelation(floorToUsdPerMeter, "Floor");
            Console.WriteLine("\nRoom count to USD per meter:");
            PrintCorrelation(roomCountToUsdPerMeter, "Room count");
            Console.WriteLine("\nClosest Metro to USD per meter:");
            PrintCorrelation(closestMetroStationToUsdPerMeter, "Closest Metro station");
            Console.WriteLine();

            var flatsForRentResponse = await GetFlatSearchResponse(new KufarSearchQueryParameters() { Type = "let" });

            var flatsForRent = GetFlatsByBookingParams(flatsForRentResponse, new PropertyBookingParameters() { Area = "Первомайский", EntryDate = DateTime.Today.AddDays(3), ExitDate = DateTime.Today.AddDays(7) });
            Console.WriteLine("\nSuitable flat IDs, prices and short descriptions:");
            foreach (var flat in flatsForRent)
                Console.WriteLine(flat.AdId + " $" + flat.PriceUsd + " " + flat.BodyShort + '\n');
        }
    }
}
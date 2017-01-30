using Android.App;
using Android.Widget;
using Android.OS;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Json;
namespace Weather
{
	[Activity(Label = "Weather", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		int count = 1;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it

			// Get the latitude/longitude EditBox and button resources:
			EditText latitude = FindViewById<EditText>(Resource.Id.latText);
			EditText longitude = FindViewById<EditText>(Resource.Id.longText);
			Button button = FindViewById<Button>(Resource.Id.getWeatherButton);

			button.Click += async (sender, e) =>
			{
				// Get the latitude and longitude entered by the user and create a query.
				string url = "http://api.geonames.org/findNearByWeatherJSON?lat=" +
							 latitude.Text +
							 "&lng=" +
							 longitude.Text +
							 "&username=demo";

				// Fetch the weather information asynchronously, 
				// parse the results, then update the screen:
				JsonValue json = await FetchWeatherAsync(url);
				ParseAndDisplay(json);
			};
		}

		async Task<JsonValue> FetchWeatherAsync(string url)
		{
			HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(new System.Uri(url));
			req.ContentType = "application/json";
			req.Method = "GET";

			using (WebResponse response = await req.GetResponseAsync())
			{
				using (Stream stream = response.GetResponseStream())
				{
					JsonValue Document = await Task.Run(() => JsonObject.Load(stream));
					Console.Out.WriteLine("LOL");

					return Document;
				}
			}
				
		}

		// Parse the weather data, then write temperature, humidity, 
		// conditions, and location to the screen.
		private void ParseAndDisplay(JsonValue json)
		{
			// Get the weather reporting fields from the layout resource:
			TextView location = FindViewById<TextView>(Resource.Id.locationText);
			TextView temperature = FindViewById<TextView>(Resource.Id.tempText);
			TextView humidity = FindViewById<TextView>(Resource.Id.humidText);
			TextView conditions = FindViewById<TextView>(Resource.Id.condText);

			// Extract the array of name/value results for the field name "weatherObservation". 
			JsonValue weatherResults = json["weatherObservation"];

			// Extract the "stationName" (location string) and write it to the location TextBox:
			location.Text = weatherResults["stationName"];

			// The temperature is expressed in Celsius:
			double temp = weatherResults["temperature"];
			// Convert it to Fahrenheit:
			temp = ((9.0 / 5.0) * temp) + 32;
			// Write the temperature (one decimal place) to the temperature TextBox:
			temperature.Text = String.Format("{0:F1}", temp) + "° F";

			// Get the percent humidity and write it to the humidity TextBox:
			double humidPercent = weatherResults["humidity"];
			humidity.Text = humidPercent.ToString() + "%";

			// Get the "clouds" and "weatherConditions" strings and 
			// combine them. Ignore strings that are reported as "n/a":
			string cloudy = weatherResults["clouds"];
			if (cloudy.Equals("n/a"))
				cloudy = "";
			string cond = weatherResults["weatherCondition"];
			if (cond.Equals("n/a"))
				cond = "";

			// Write the result to the conditions TextBox:
			conditions.Text = cloudy + " " + cond;
		}
	}

}


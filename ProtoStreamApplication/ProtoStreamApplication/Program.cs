// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ProtoStreamApplication
{
    public class Movie
    {
        public string Film { get; set; }
        public string Genre { get; set; }
        public string LeadStudio { get; set; }
        public int AudienceScore { get; set; }
        public string Profitability { get; set; }
        public int RottonTomatos { get; set; }
        public int WorldWideGross { get; set; }
        public DateOnly Year { get; set; }
    }

    class Program
    {
        static HttpClient client = new HttpClient();
        static WebClient wc = new WebClient();
        string csvData = wc.DownloadString("https://gist.githubusercontent.com/tiangechen/b68782efa49a16edaf07dc2cdaa855ea/raw/0c794a9717f18b094eabab2cd6a6b9a226903577/movies.csv");

        static void ShowMovie(Movie movie)
        {
            Console.WriteLine(
                $"Name: {movie.Film}\tGenre: " +
                $"{movie.Genre}\tLead Studio: " +
                $"{movie.LeadStudio}\tAudience Score: " +
                $"{movie.Profitability}\tRottonTomatos: " +
                $"{movie.RottonTomatos}\tWorldWideGross: " +
                $"{movie.WorldWideGross}\tYear: " +
                $"{movie.Year}\t");
        }


        static async Task<Uri> CreateMovieAsync(Movie movie)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/products", movie);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }

        static async Task<Movie> GetProductAsync(string path)
        {
            Movie movie = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                movie = await response.Content.ReadAsAsync<Movie>();
            }
            return movie;
        }

        static async Task<Movie> UpdateProductAsync(Movie movie)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(
                $"api/products/{movie.Film}", movie);
            response.EnsureSuccessStatusCode();

            // Deserialize the updated product from the response body.
            movie = await response.Content.ReadAsAsync<Movie>();
            return movie;
        }

        static async Task<HttpStatusCode> DeleteProductAsync(string movie)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                $"api/products/{movie}");
            return response.StatusCode;
        }

        static void Main()
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            // Update port # in the following line.
            //client.BaseAddress = new Uri("http://localhost:64195/");
            client.BaseAddress = new Uri("https://gist.githubusercontent.com/tiangechen/b68782efa49a16edaf07dc2cdaa855ea/raw/0c794a9717f18b094eabab2cd6a6b9a226903577/movies.csv");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                /* Create a new product
                Product product = new Product
                {
                    Name = "Gizmo",
                    Price = 100,
                    Category = "Widgets"
                };

                var url = await CreateProductAsync(product);
                Console.WriteLine($"Created at {url}");
                */
                Movie movie = new Movie();
                var url = await CreateMovieAsync(movie);
                // Get the product
                movie = await GetProductAsync(url.PathAndQuery);
                ShowMovie(movie);

                // Update the product
                Console.WriteLine("Updating price...");
                movie.RottonTomatos = 80;
                await UpdateProductAsync(movie);

                // Get the updated product
                movie = await GetProductAsync(url.PathAndQuery);
                ShowMovie(movie);

                // Delete the product
                var statusCode = await DeleteProductAsync(movie.Film);
                Console.WriteLine($"Deleted (HTTP Status = {(int)statusCode})");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();

        }
    }
}

// See https://aka.ms/new-console-template for more information
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using MonkeyShelter.DemoApp.DTO;
using MonkeyShelter.DTO;
using System.Globalization;

class Program
{
    

    private static readonly HttpClient _client = new HttpClient
    {
        BaseAddress = new Uri("https://localhost:7265/api/") // OVO promeni po potrebi!!!
    };

    static async Task Main(string[] args)
    {
        bool running = true;

        while (running)
        {
            Console.Clear();
            Console.WriteLine("🐒 Monkey Shelter Demo App 🐒");
            Console.WriteLine("1. Add Monkey");
            Console.WriteLine("2. Depart Monkey");
            Console.WriteLine("3. Update Monkey Weight");
            Console.WriteLine("4. Vet Check");
            Console.WriteLine("5. Reports");
            Console.WriteLine("0. Exit");
            Console.Write("\nChoose an option: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    await AddMonkeyAsync();
                    break;
                case "2":
                    await DepartMonkeyAsync();
                    break;
                case "3":
                    await UpdateWeightAsync();
                    break;
                case "4":
                    await VetCheckAsync();
                    break;
                case "5":
                    await ShowReportsAsync();
                    break;
                case "0":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.ReadLine();
        }
    }

    private static async Task AddMonkeyAsync()
    {
        Console.Write("Enter monkey name: ");
        string name = Console.ReadLine();

        Console.Write("Enter species ID: ");
        int speciesId = int.Parse(Console.ReadLine());

        Console.Write("Enter weight: ");
        double weight = double.Parse(Console.ReadLine());

        Console.Write("Enter shelter ID: ");
        int shelterId = int.Parse(Console.ReadLine());

        var monkey = new
        {
            Name = name,
            SpeciesId = speciesId,
            Weight = weight,
            ArrivalDate = DateTime.UtcNow,
            ShelterId = shelterId
        };

        var json = JsonSerializer.Serialize(monkey);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsJsonAsync("Monkey/arrival", monkey);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine("✅ Monkey added successfully!");
            Console.WriteLine($"Response: {result}");
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();

            try
            {
                // Pokušaj da deserijalizuješ grešku u OutputResponse
                var errorDetails = JsonSerializer.Deserialize<OutputResponse<string>>(errorContent);

                if (errorDetails != null && !string.IsNullOrEmpty(errorDetails.ErrorMessage))
                {
                    Console.WriteLine($"❌ Error: {errorDetails.ErrorMessage}");
                }
                else
                {
                    // Ako nije moguće deserializovati grešku ili je prazna poruka
                    Console.WriteLine($"❌ Failed to add monkey. Status code: {response.StatusCode}");
                    Console.WriteLine($"Error: {errorContent}");
                }
            }
            catch (JsonException)
            {
                // Ako dođe do greške prilikom deserializacije, prikaži osnovnu grešku
                Console.WriteLine($"❌ Failed to add monkey. Status code: {response.StatusCode}");
                Console.WriteLine($"Error: {errorContent}");
            }

        }
            
    }

    private static async Task DepartMonkeyAsync()
    {
        Console.Write("Enter monkey ID to depart: ");
        int id = int.Parse(Console.ReadLine());

        var json = JsonSerializer.Serialize(id);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PatchAsync($"Monkey/departure/{id}", null);
        

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine("✅ Monkey departed successfully!");
            Console.WriteLine($"Response: {result}");
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();

            try
            {
                // Pokušaj da deserijalizuješ grešku u OutputResponse
                var errorDetails = JsonSerializer.Deserialize<OutputResponse<List<Monkey>>>(errorContent);

                if (errorDetails != null && !string.IsNullOrEmpty(errorDetails.ErrorMessage))
                {
                    Console.WriteLine($"❌ Error: {errorDetails.ErrorMessage}");
                }
                else
                {
                    // Ako nije moguće deserializovati grešku ili je prazna poruka
                    Console.WriteLine($"❌ Failed to depart monkey. Status code: {response.StatusCode}");
                    Console.WriteLine($"Error: {errorContent}");
                }
            }
            catch (JsonException)
            {
                // Ako dođe do greške prilikom deserializacije, prikaži osnovnu grešku
                Console.WriteLine($"❌ Failed to depart monkey. Status code: {response.StatusCode}");
                Console.WriteLine($"Error: {errorContent}");
            }
        }



    }

    private static async Task UpdateWeightAsync()
    {
        Console.Write("Enter monkey ID to update weight: ");
        int id = int.Parse(Console.ReadLine());

        Console.Write("Enter new weight: ");
        double weight = double.Parse(Console.ReadLine());

        var payload = new UpdateWeightDTO { Weight = weight };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PatchAsJsonAsync($"Monkey/{id}/weight", payload);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response: {result}");
            Console.WriteLine("✅ Monkey weight updated successfully!");
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();

            try
            {
                // Pokušaj da deserijalizuješ grešku u OutputResponse
                var errorDetails = JsonSerializer.Deserialize<OutputResponse<List<Monkey>>>(errorContent);

                if (errorDetails != null && !string.IsNullOrEmpty(errorDetails.ErrorMessage))
                {
                    Console.WriteLine($"❌ Error: {errorDetails.ErrorMessage}");
                }
                else
                {
                    // Ako nije moguće deserializovati grešku ili je prazna poruka
                    Console.WriteLine($"❌ Failed to update monkey's weight. Status code: {response.StatusCode}");
                    Console.WriteLine($"Error: {errorContent}");
                }
            }
            catch (JsonException)
            {
                // Ako dođe do greške prilikom deserializacije, prikaži osnovnu grešku
                Console.WriteLine($"❌ Failed to update monkey's weight. Status code: {response.StatusCode}");
                Console.WriteLine($"Error: {errorContent}");
            }
        }
    }

    private static async Task VetCheckAsync()
    {
        //Console.Write("Enter monkey ID to update vet check date: ");
        //int id = int.Parse(Console.ReadLine());

        //var response = await _client.PatchAsync($"monkeys/{id}/vet-check", null);


        var response = await _client.GetAsync("Monkey/vet-checks");

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine("✅ Vet check date updated successfully!");
            Console.WriteLine($"Response: {result}");
        }

        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();

            try
            {
                // Pokušaj da deserijalizuješ grešku u OutputResponse
                var errorDetails = JsonSerializer.Deserialize<OutputResponse<List<Monkey>>>(errorContent);

                if (errorDetails != null && !string.IsNullOrEmpty(errorDetails.ErrorMessage))
                {
                    Console.WriteLine($"❌ Error: {errorDetails.ErrorMessage}");
                }
                else
                {
                    // Ako nije moguće deserializovati grešku ili je prazna poruka
                    Console.WriteLine($"❌ Failed to depart monkey. Status code: {response.StatusCode}");
                    Console.WriteLine($"Error: {errorContent}");
                }
            }
            catch (JsonException)
            {
                // Ako dođe do greške prilikom deserializacije, prikaži osnovnu grešku
                Console.WriteLine($"❌ Failed to update vet check: {response.StatusCode}");
                Console.WriteLine($"Error: {errorContent}");
            }
           
        }
           
    }

    private static async Task ShowReportsAsync()
    {
        Console.WriteLine("Choose report:");
        Console.WriteLine("1. Count of monkeys per species");
        Console.WriteLine("2. Count of monkeys per species between dates");

        var reportChoice = Console.ReadLine();

        switch (reportChoice)
        {
            case "1":
                var response1 = await _client.GetAsync("Report/monkey-count-per-species");
                if (response1.IsSuccessStatusCode)
                {
                    var result = await response1.Content.ReadAsStringAsync();
                    Console.WriteLine(result);
                }
                break;

            case "2":
                Console.Write("Enter start date (yyyy-MM-dd): ");
                var startInput = Console.ReadLine();
                DateTime startDate;
                if (!DateTime.TryParseExact(startInput, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
                {
                    Console.WriteLine("Invalid start date format.");
                    return;
                }

                Console.Write("Enter end date (yyyy-MM-dd): ");
                var endInput = Console.ReadLine();
                DateTime endDate;
                if (!DateTime.TryParseExact(endInput, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
                {
                    Console.WriteLine("Invalid end date format.");
                    return;
                }

                var response2 = await _client.GetAsync($"Report/monkey-arrivals-between-dates?startDate={startInput}&endDate={endInput}");
                if (response2.IsSuccessStatusCode)
                {
                    var result2 = await response2.Content.ReadAsStringAsync();
                    Console.WriteLine(result2);
                }
                else
                {
                    Console.WriteLine($"❌ Failed to depart monkey. Status code: {response2.StatusCode}");
                  

                }
                break;
            default:
                Console.WriteLine("Invalid report choice.");
                break;
        }
    }
}



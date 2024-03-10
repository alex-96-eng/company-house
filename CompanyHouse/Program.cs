namespace CompanyHouse
{
    using System;
    using System.Threading.Tasks;
    using Npgsql;

    public class Program
    {
        private const string ConnectionString = "Host=localhost;Username=test_user;Password=changeme;Database=test_db";

        public static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                ShowUsage();
                return;
            }

            string directoryPath = args.Length > 1? args[1]: string.Empty;

            switch (args[0].ToLower())
            {
                // This case statement has redundancy, but the CLI is just for dev puposes, so didn't 
                // put any time into altering it. 
                case "load-company-data":
                    if (string.IsNullOrEmpty(directoryPath))
                    {
                        Console.WriteLine("You must specify a directory path for the 'load-company-data' command.");
                        ShowUsage();
                        return;
                    }
                    Console.WriteLine("Loading and storing company data");
                    await using (var connection = new NpgsqlConnection(ConnectionString))
                    {
                        await connection.OpenAsync();
                        var dataHandler = new DataHandlers.CompanyDataHandler(connection);
                        await dataHandler.LoadAndStoreDataAsync(directoryPath);
                    }
                    break;

                case "load-account-data":
                    if (string.IsNullOrEmpty(directoryPath))
                    {
                        Console.WriteLine("You must specify a directory path for the 'load-company-data' command.");
                        ShowUsage();
                        return;
                    }
                    Console.WriteLine("Loading and storing company data");
                    await using (var connection = new NpgsqlConnection(ConnectionString))
                    {
                        await connection.OpenAsync();
                        var dataHandler = new DataHandlers.AccountDataHandler(connection);
                        await dataHandler.LoadAndStoreDataAsync(directoryPath);
                    }
                    break;
                    
                case "load-psc-data":
                    if (string.IsNullOrEmpty(directoryPath))
                    {
                        Console.WriteLine("You must specify a directory path for the 'load-company-data' command.");
                        ShowUsage();
                        return;
                    }
                    Console.WriteLine("Loading and storing company data");
                    await using (var connection = new NpgsqlConnection(ConnectionString))
                    {
                        await connection.OpenAsync();
                        var dataHandler = new DataHandlers.PscDataHandler(connection);
                        await dataHandler.LoadAndStoreDataAsync(directoryPath);
                    }
                    break;

                // Additional cases for 'load-account-data' and 'load-psc-data' go here
                default:
                    Console.WriteLine("Invalid command.");
                    ShowUsage();
                    break;
            }
        }

        private static void ShowUsage()
        {
            // This is just for developer purposes
            Console.WriteLine("Usage: dotnet run [command]");
            Console.WriteLine("Commands:");
            Console.WriteLine("\tload-company-data - Takes a local company data CSV and loads into DB.");
            Console.WriteLine("\tload-account-data - Takes a local account data CSV and loads into DB.");
            Console.WriteLine("\tload-psc-data - Takes a local PSC CSV and loads into DB.");
            // will add other commands here
        }
    }
}

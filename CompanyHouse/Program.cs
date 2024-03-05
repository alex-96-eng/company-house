using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

using CsvHelper;
using CsvHelper.Configuration;
using Npgsql;


class Program
{
    /// <summary>
    /// This database is span up and exposed in the docker-compose.yaml file.
    /// </summary>
    private const string ConnectionString = "Host=localhost;Username=test_user;Password=changeme;Database=test_db";
    private const int BatchSize = 2;

    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            ShowUsage();
            return;
        }

        switch (args[0].ToLower())
        {
            case "download":
                if (args.Length < 2)
                {
                    Console.WriteLine("You must specify a directory path for the 'download' command.");
                    ShowUsage();
                    return;
                }
                string directoryPath = args[1];
                await DownloadAndStoreCompanyDataAsync(directoryPath);
                break;
            default:
                Console.WriteLine("Invalid command.");
                ShowUsage();
                break;
        }
    }

    private static void ShowUsage()
    {
        Console.WriteLine("Usage: dotnet run [command]");
        Console.WriteLine("Commands:");
        Console.WriteLine("\tdownload - Download and store company data");
    }

    private static async Task DownloadAndStoreCompanyDataAsync(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine($"The specified directory does not exist: {directoryPath}");
            return;
        }

        Console.WriteLine("Downloading and storing company data");

        var filePaths = Directory.GetFiles(directoryPath, "*.csv");
        foreach (var filePath in filePaths)
        {
            using var reader = new StreamReader(filePath);
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<CompanyRow>();
                var batch = new List<CompanyRow>();
                foreach (var record in records)
                {
                    batch.Add(record);
                    if (batch.Count >= BatchSize)
                    {
                        await InsertRecordsAsync(batch);
                        batch.Clear();
                    }
                }

                if (batch.Any())
                {
                    await InsertRecordsAsync(batch);
                }
            }
        }
    }

    private static void DownloadAndStoreAccountsDataAsync()
    {
        // todo
    }
    
    private static void DownloadAndStorePSCDataAsync()
    {
        // todo
    }

    private static async Task InsertRecordsAsync(List<CompanyRow> records)
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        
        // Make this clevererer
        var commandText = @"INSERT INTO company_data (
            companyname, companynumber, addressline1, addressline2, posttown, county, country, postcode,
            companycategory, companystatus, countryoforigin, dissolutiondate, incorporationdate, accountrefday,
            accountrefmonth, accountnextduedate, accountlastmadeupdate, accountcategory, returnsnextduedate,
            returnslastmadeupdate, mortgagesnummortcharges, mortgagesnummortoutstanding, mortgagesnummortpartsatisfied,
            nummortsatisfied, numgenpartners, numlimpartners, uri, confstmtnextduedate, confstmtlastmadeupdate
            ) VALUES (
            @companyname, @companynumber, @addressline1, @addressline2, @posttown, @county, @country, @postcode,
            @companycategory, @companystatus, @countryoforigin, @dissolutiondate::DATE, @incorporationdate::DATE, @accountrefday,
            @accountrefmonth, @accountnextduedate::DATE, @accountlastmadeupdate::DATE, @accountcategory, @returnsnextduedate::DATE,
            @returnslastmadeupdate::DATE, @mortgagesnummortcharges, @mortgagesnummortoutstanding, @mortgagesnummortpartsatisfied,
            @nummortsatisfied, @numgenpartners, @numlimpartners, @uri, @confstmtnextduedate::DATE, @confstmtlastmadeupdate::DATE)";

        foreach (var record in records)
        {
            using var command = new NpgsqlCommand(commandText, connection, transaction);
            var properties = record.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var propName = property.Name.ToLower();
                var value = property.GetValue(record);
                
                if (value is string stringValue && string.IsNullOrEmpty(stringValue))
                {
                    value = DBNull.Value;
                }
                command.Parameters.AddWithValue($"@{propName}", value ?? DBNull.Value);
            }
            await command.ExecuteNonQueryAsync();
        }
        await transaction.CommitAsync();
    }
}
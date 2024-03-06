// Not sure what to call it.
namespace CompanyHouseNamespace
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Globalization;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using CsvHelper;
    using Npgsql;

    class Program
    {
        const string connectionString = "Host=localhost;Username=test_user;Password=changeme;Database=test_db";
        private const int BatchSize = 100;

        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                ShowUsage();
                return;
            }
            string directoryPath = args.Length > 1 ? args[1] : string.Empty;
            switch (args[0].ToLower())
            {
                // This case statement has redundency, but the cli is just for dev puposes, so not 
                // put any time into altering it. 
                case "load-company-data":
                    if (string.IsNullOrEmpty(directoryPath))
                    {
                        Console.WriteLine("You must specify a directory path for the 'load-company-data' command.");
                        ShowUsage();
                        return;
                    }
                    Console.WriteLine("Loading and storing company data");
                    await using (var connection = new NpgsqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        var dataHandler = new CompanyDataHandler(connection);
                        await dataHandler.LoadAndStoreDataAsync(directoryPath, BatchSize);
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
                    await using (var connection = new NpgsqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        var dataHandler = new AccountDataHandler(connection);
                        await dataHandler.LoadAndStoreDataAsync(directoryPath, BatchSize);
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
                    await using (var connection = new NpgsqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        var dataHandler = new PSCDataHandler(connection);
                        await dataHandler.LoadAndStoreDataAsync(directoryPath, BatchSize);
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
            // This is just for developmr purposes
            Console.WriteLine("Usage: dotnet run [command]");
            Console.WriteLine("Commands:");
            Console.WriteLine("\tload-company-data - Takes a local Company Data CSV and loads into DB.");
            // will add other commands here
        }
    }
    abstract class DataHandler
    {
        protected readonly NpgsqlConnection connection;
        protected readonly int BatchSize;
        
        protected DataHandler(NpgsqlConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public async Task LoadAndStoreDataAsync(string directoryPath, int batchSize)
        {
            {
                if (!Directory.Exists(directoryPath))
                {
                    Console.WriteLine($"The specified directory does not exist: {directoryPath}");
                    return;
                }
                
                Console.WriteLine("Downloading and storing company data");
                string[] filePaths = Directory.GetFiles(directoryPath, "*.csv");
                foreach (string filePath in filePaths)
                {
                    using var reader = new StreamReader(filePath);
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        IEnumerable<CompanyDataRow> records = csv.GetRecords<CompanyDataRow>();
                        var batch = new List<Row>();
                        foreach (CompanyDataRow record in records)
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
        }

        public abstract Task InsertRecordsAsync(List<Row> records);
    }
    class CompanyDataHandler : DataHandler
    {
        public CompanyDataHandler(NpgsqlConnection connection) : base(connection)
        {
            
        }
        public override async Task InsertRecordsAsync(List<Row> records)
        {
            await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();

            // Make this clevererer
            string commandText = @"INSERT INTO company_data (
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
                PropertyInfo[] properties = record.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in properties)
                {
                    string propName = property.Name.ToLower();
                    object value = property.GetValue(record);
                    
                    // Should probs live in the constructor
                    if (value is Uri uriValue)
                    {
                        value = uriValue.ToString();
                    }
                    else if (value is string stringValue && string.IsNullOrEmpty(stringValue))
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
    class AccountDataHandler : DataHandler
    {
        public AccountDataHandler(NpgsqlConnection connection) : base(connection)
        {
            
        }
        public override async Task InsertRecordsAsync(List<Row> records)
        {
            // todo 
        }
    }
    class PSCDataHandler : DataHandler
    {
        public PSCDataHandler(NpgsqlConnection connection) : base(connection)
        {
            
        }
        public override async Task InsertRecordsAsync(List<Row> records)
        {
            // todo 
        }
    }
}
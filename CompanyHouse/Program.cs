namespace CompanyHouse
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Linq;
    using System.Reflection;
    using System.Globalization;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Npgsql;
    
    using DataHandlers;
    
    class Program
    {
        const string connectionString = "Host=localhost;Username=test_user;Password=changeme;Database=test_db";
        private const int batchSize = 100;

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
                // This case statement has redundancy, but the cli is just for dev puposes, so not 
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
                        await dataHandler.LoadCSVAndStoreDataAsync(directoryPath);
                    }
                    break;

                case "load-account-data":
                    if (string.IsNullOrEmpty(directoryPath))
                    {
                        Console.WriteLine("You must specify a directory path for the 'load-account-data' command.");
                        ShowUsage();
                        return;
                    }
                    Console.WriteLine("Loading and storing account data");
                    await using (var connection = new NpgsqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        var dataHandler = new AccountDataHandler(connection);
                        await dataHandler.LoadXHTMLAndStoreDataAsync(directoryPath);
                    }
                    break;
                    
                case "load-psc-data":
                    if (string.IsNullOrEmpty(directoryPath))
                    {
                        Console.WriteLine("You must specify a directory path for the 'load-psc-data' command.");
                        ShowUsage();
                        return;
                    }
                    Console.WriteLine("Loading and storing PSC data");
                    await using (var connection = new NpgsqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        var dataHandler = new PSCDataHandler(connection);
                        await dataHandler.LoadJSONAndStoreData(directoryPath);
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
            Console.WriteLine("\tload-company-data - Takes a local Company Data CSV and loads into DB.");
            // will add other commands here
        }
    }
    // abstract class DataHandler
    // {
    //     protected readonly NpgsqlConnection connection;
    //     protected readonly int batchSize;
    //     protected DataHandler(NpgsqlConnection connection)
    //     {
    //         this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
    //         this.batchSize = batchSize;
    //     }
    //     public string[] GetAllFilePathsInDirectory(string directoryPath, string fileType)
    //     {
    //         // if (!Directory.Exists(directoryPath))
    //         // {
    //         //     Console.WriteLine($"The specified directory does not exist: {directoryPath}");
    //         //     return;
    //         // }
    //         Console.WriteLine($"Reading files in: {directoryPath}");
    //         string[] filePaths = Directory.GetFiles(directoryPath, $"*.{fileType}");
    //         // if (filePaths?.Length > 0)
    //         // {
    //         //     Console.WriteLine($"The specified directory contains no files.");
    //         //     return;
    //         // }
    //         return filePaths;
    //     }
    //     public async Task LoadCSVAndStoreDataAsync(string directoryPath)
    //     {
    //         string[] filePaths = GetAllFilePathsInDirectory(directoryPath, "csv");
    //         Console.WriteLine("Downloading and storing CVS data");
    //         foreach (string filePath in filePaths)
    //         {
    //             using var reader = new StreamReader(filePath);
    //             using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
    //             {
    //                 IEnumerable<BaseData> records = csv.GetRecords<BaseData>();
    //                 var batchedData = new List<BaseData>();
    //                 foreach (CompanyDataRow record in records)
    //                 {
    //                     batchedData.Add(record);
    //                     if (batchedData.Count >= batchSize)
    //                     {
    //                         await InsertRecordsAsync(batchedData);
    //                         batchedData.Clear();
    //                     }
    //                 }
    //                 if (batchedData.Any())
    //                 {
    //                     await InsertRecordsAsync(batch);
    //                 }
    //             }
    //         }
    //     }
    //     public abstract Task InsertRecordsAsync(List<Row> records);
    // }
    // class CompanyDataHandler : DataHandler
    // {
    //     public CompanyDataHandler(NpgsqlConnection connection) : base(connection)
    //     {
    //         
    //     }
    //     public override async Task InsertRecordsAsync(List<Row> records)
    //     {
    //         await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();
    //
    //         // Make this clevererer
    //         string commandText = @"INSERT INTO company_data (
    //         companyname, companynumber, addressline1, addressline2, posttown, county, country, postcode,
    //         companycategory, companystatus, countryoforigin, dissolutiondate, incorporationdate, accountrefday,
    //         accountrefmonth, accountnextduedate, accountlastmadeupdate, accountcategory, returnsnextduedate,
    //         returnslastmadeupdate, mortgagesnummortcharges, mortgagesnummortoutstanding, mortgagesnummortpartsatisfied,
    //         nummortsatisfied, numgenpartners, numlimpartners, uri, confstmtnextduedate, confstmtlastmadeupdate
    //         ) VALUES (
    //         @companyname, @companynumber, @addressline1, @addressline2, @posttown, @county, @country, @postcode,
    //         @companycategory, @companystatus, @countryoforigin, @dissolutiondate, @incorporationdate, @accountrefday,
    //         @accountrefmonth, @accountnextduedate, @accountlastmadeupdate, @accountcategory, @returnsnextduedate,
    //         @returnslastmadeupdate, @mortgagesnummortcharges, @mortgagesnummortoutstanding, @mortgagesnummortpartsatisfied,
    //         @nummortsatisfied, @numgenpartners, @numlimpartners, @uri, @confstmtnextduedate, @confstmtlastmadeupdate)";
    //
    //         foreach (Row record in records)
    //         {
    //             using var command = new NpgsqlCommand(commandText, connection, transaction);
    //             PropertyInfo[] properties = record.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
    //             foreach (var property in properties)
    //             {
    //                 string propName = property.Name.ToLower();
    //                 object value = property.GetValue(record);
    //                 
    //                 // Should probs live in the constructor
    //                 if (value is Uri uriValue)
    //                 {
    //                     value = uriValue.ToString();
    //                 }
    //                 else if (value is string stringValue && string.IsNullOrEmpty(stringValue))
    //                 {
    //                     value = DBNull.Value;
    //                 }
    //                 
    //                 command.Parameters.AddWithValue($"@{propName}", value ?? DBNull.Value);
    //             }
    //             await command.ExecuteNonQueryAsync();
    //         }
    //         await transaction.CommitAsync();
    //     }
    // }
    // class AccountDataHandler : DataHandler
    // {
    //     public AccountDataHandler(NpgsqlConnection connection) : base(connection)
    //     {
    //         
    //     }
    //     public async Task LoadXHTMLAndStoreDataAsync(string directoryPath)
    //     {
    //         string[] filePaths = Directory.GetFiles(directoryPath, "*.html");
    //         List<Row> batch = new List<Row>();
    //
    //         foreach (string filePath in filePaths)
    //         {
    //             Console.WriteLine($"Processing HTML data from {filePath}");
    //             var document = new AccountDataDocument();
    //             // Surely there is some built in reflection tool that can offer this as part of constructing the class?
    //             var documentMapper = new Dictionary<string, Action<string>>
    //             {
    //                 ["uk-bus:UKCompaniesHouseRegisteredNumber"] = value => document.CompanyNumber = value,
    //                 ["uk-bus:EntityCurrentLegalOrRegisteredName"] = value => document.CompanyName = value,
    //                 ["uk-bus:EntityDormantTruefalse"] = value => document.IsDormant = bool.Parse(value),
    //                 ["uk-bus:StartDateForPeriodCoveredByReport"] = value => document.StartDate = value,
    //                 ["uk-bus:EndDateForPeriodCoveredByReport"] = value => document.EndDate =value,
    //                 ["uk-bus:BalanceSheetDate"] = value => document.BalanceSheetDate = value,
    //                 ["uk-direp:CompanyHasActedAsAnAgentDuringPeriodTruefalse"] = value => document.AsAgent = bool.Parse(value),
    //                 ["uk-bus:DescriptionPrincipalActivities"] = value => document.DescriptionPrincipalActivities = value
    //             };
    //             using (XmlReader reader = XmlReader.Create(filePath)) 
    //             { 
    //                 while (reader.Read())
    //                 {
    //                     if (reader.NodeType == XmlNodeType.Element && reader.HasAttributes)
    //                     {
    //                         string nameAttribute = reader["name"];
    //                         if (nameAttribute != null && documentMapper.TryGetValue(nameAttribute, out var setProperty))
    //                         {
    //                             var content = reader.ReadInnerXml();
    //                             setProperty(content);
    //                         }
    //                     }
    //                 }
    //             }
    //
    //             batch.Add(document); // Add the fully populated document to the batch after finishing the file
    //         }
    //
    //         if (batch.Any())
    //         {
    //             Console.WriteLine($"Inserting {batch.Count} records...");
    //             await InsertRecordsAsync(batch);
    //         }
    //     }
    //     public override async Task InsertRecordsAsync(List<Row> records)
    //     {
    //         await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();
    //         string commandText = @"INSERT INTO account_data (
    //                 companynumber, companyname, isdormant, startdate, enddate, balancesheetdate, entitytradingstatus, asagent, descriptionprincipalactivities
    //             )
    //             VALUES (
    //                 @companynumber, @companyname, @isdormant, @startdate, @enddate, @balancesheetdate, @entitytradingstatus, @asagent, @descriptionprincipalactivities
    //             )";
    //         
    //         foreach (Row record in records)
    //         {
    //             using var command = new NpgsqlCommand(commandText, connection, transaction);
    //             PropertyInfo[] properties = record.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
    //             foreach (var property in properties)
    //             {
    //                 string propName = property.Name.ToLower();
    //                 object value = property.GetValue(record);
    //                 if (value is string stringValue && string.IsNullOrEmpty(stringValue))
    //                 {
    //                     value = DBNull.Value;
    //                 }
    //                 command.Parameters.AddWithValue($"@{propName}", value ?? DBNull.Value);
    //             }
    //             await command.ExecuteNonQueryAsync();
    //         }
    //         await transaction.CommitAsync();
    //     }
    // }
    // class PSCDataHandler : DataHandler
    // {
    //     public PSCDataHandler(NpgsqlConnection connection) : base(connection)
    //     {
    //         
    //     }
    //
    //     public async Task LoadJSONAndStoreData(string directoryPath)
    //     {
    //         List<Row> batch = new List<Row>();
    //         string[] filePaths = GetAllFilePathsInDirectory(directoryPath, "json");
    //         Console.WriteLine("Downloading and storing JSON data");
    //         foreach (string filePath in filePaths)
    //         {
    //             string jsonContent = await File.ReadAllTextAsync(filePath);
    //             using JsonDocument doc = JsonDocument.Parse(jsonContent);
    //             string root = doc.RootElement;
    //             string data = root.GetProperty("data");
    //             var record = new FlatCompanyData
    //             {
    //                 CompanyNumber = root.GetString("company_number"),
    //                 AddressLine1 = data.GetProperty("address").GetString("address_line_1"),
    //                 Country = data.GetProperty("address").GetString("country"),
    //                 Locality = data.GetProperty("address").GetString("locality"),
    //                 PostalCode = data.GetProperty("address").GetString("postal_code"),
    //                 Premises = data.GetProperty("address").GetString("premises"),
    //                 CeasedOn = data.GetString("ceased_on"),
    //                 CountryOfResidence = data.GetString("country_of_residence"),
    //                 DateOfBirthMonth = data.GetProperty("date_of_birth").GetInt32("month"),
    //                 DateOfBirthYear = data.GetProperty("date_of_birth").GetInt32("year"),
    //             };
    //             {
    //                 batch.Add(record);
    //                 Console.WriteLine(record);
    //                 if (batch.Count >= batchSize)
    //                 {
    //                     //await InsertRecordsAsync(batch);
    //                     batch.Clear();
    //                 }
    //             }
    //             if (batch.Any())
    //             {
    //                 //await InsertRecordsAsync(batch);
    //             }
    //         }
    //     }
    //     
    //     public override async Task InsertRecordsAsync(List<Row> records)
    //     {
    //         await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();
    //         string commandText = @"INSERT INTO psc_data (companyname, data) VALUES (@companyname, @data::JSONB);";
    //         foreach (Row record in records)
    //         {
    //             using var command = new NpgsqlCommand(commandText, connection, transaction);
    //             // Assuming the record class has properties named CompanyName and Data
    //             command.Parameters.AddWithValue("companyname", record.CompanyName); // Make sure property names match your actual class
    //
    //             // Serialize Data property to JSON string if it's not already a string
    //             var jsonData = record.Data is string ? (string)record.Data : JsonConvert.SerializeObject(record.Data);
    //             var dataParam = command.CreateParameter();
    //             dataParam.ParameterName = "data";
    //             dataParam.Value = jsonData ?? DBNull.Value;
    //             dataParam.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Jsonb; // Explicitly set the NpgsqlDbType for JSONB
    //             command.Parameters.Add(dataParam);
    //         }
    //         await transaction.CommitAsync();
    //     }
    // }
} 
namespace CompanyHouse.DataHandlers
{
    using System.Reflection;
    
    using Npgsql;
    using CsvHelper;
    using System.Xml;

    using Entities;
    using Utilities;
    
    abstract class DataHandler
    {
        protected readonly NpgsqlConnection connection;
        protected readonly int batchSize;
        protected DataHandler(NpgsqlConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.batchSize = batchSize;
        }
        public async Task LoadCSVAndStoreDataAsync(string directoryPath)
        {
            string[] filePaths = Utilitlies.GetAllFilePathsInDirectory(directoryPath, "csv");
            Console.WriteLine("Downloading and storing CVS data");
            foreach (string filePath in filePaths)
            {
                using var reader = new StreamReader(filePath);
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    IEnumerable<BaseData> records = csv.GetRecords<BaseData>();
                    var batchData = new List<BaseData>();
                    foreach (BaseData record in records)
                    {
                        batch.Add(record);
                        if (batchData.Count >= batchSize)
                        {
                            await InsertRecordsAsync(batchData);
                            batchData.Clear();
                        }
                    }
                    if (batchData.Any())
                    {
                        await InsertRecordsAsync(batchData);
                    }
                }
            }
        }
        public abstract Task InsertRecordsAsync(List<BaseData> records);
    }
    class CompanyDataHandler : DataHandler
    {
        public CompanyDataHandler(NpgsqlConnection connection) : base(connection)
        {
            
        }
        public override async Task InsertRecordsAsync(List<BaseData> records)
        {
            await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();
            string commandText = @"INSERT INTO company_data (
            company_name, company_number, address_line_1, address_line_2, post_town, county, country, postcode,
            company_category, company_status, country_of_origin, dissolution_date, incorporation_date, account_ref_day,
            account_ref_month, account_next_due_date, account_last_made_update, account_category, returns_next_due_date,
            returns_last_made_update, mortgages_num_mort_charges, mortgages_num_mort_outstanding, mortgages_num_mort_part_satisfied,
            num_mort_satisfied, numgenpartners, num_lim_partners, uri, conf_stmt_next_due_date, conf_stmt_last_made_update
            ) VALUES (
            # TODO:: @companyname, @companynumber, @addressline1, @addressline2, @posttown, @county, @country, @postcode,
            @companycategory, @companystatus, @countryoforigin, @dissolutiondate, @incorporationdate, @accountrefday,
            @accountrefmonth, @accountnextduedate, @accountlastmadeupdate, @accountcategory, @returnsnextduedate,
            @returnslastmadeupdate, @mortgagesnummortcharges, @mortgagesnummortoutstanding, @mortgagesnummortpartsatisfied,
            @nummortsatisfied, @numgenpartners, @numlimpartners, @uri, @confstmtnextduedate, @confstmtlastmadeupdate)";

            foreach (BaseData record in records)
            {
                using var command = new NpgsqlCommand(commandText, connection, transaction);
                PropertyInfo[] properties = record.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in properties)
                {
                    string propName = property.Name.ToLower();
                    object value = property.GetValue(record);
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
        public async Task LoadXHTMLAndStoreDataAsync(string directoryPath)
        {
            string[] filePaths = Directory.GetFiles(directoryPath, "*.html");
            List<BaseData> batchedData = new List<BaseData>();
            foreach (string filePath in filePaths)
            {
                Console.WriteLine($"Processing HTML data from {filePath}");
                var document = new FinancialData();
                // Surely there is some built in reflection tool that can offer this as part of constructing the class?
                var documentMapper = new Dictionary<string, Action<string>>
                {
                    ["uk-bus:UKCompaniesHouseRegisteredNumber"] = value => document.CompanyNumber = value,
                    ["uk-bus:EntityCurrentLegalOrRegisteredName"] = value => document.CompanyName = value,
                    ["uk-bus:EntityDormantTruefalse"] = value => document.IsDormant = bool.Parse(value),
                    ["uk-bus:StartDateForPeriodCoveredByReport"] = value => document.StartDate = value,
                    ["uk-bus:EndDateForPeriodCoveredByReport"] = value => document.EndDate =value,
                    ["uk-bus:BalanceSheetDate"] = value => document.BalanceSheetDate = value,
                    ["uk-direp:CompanyHasActedAsAnAgentDuringPeriodTruefalse"] = value => document.AsAgent = bool.Parse(value),
                    ["uk-bus:DescriptionPrincipalActivities"] = value => document.DescriptionPrincipalActivities = value
                };
                using (XmlReader reader = XmlReader.Create(filePath)) 
                { 
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.HasAttributes)
                        {
                            string nameAttribute = reader["name"];
                            if (nameAttribute != null && documentMapper.TryGetValue(nameAttribute, out var setProperty))
                            {
                                var content = reader.ReadInnerXml();
                                setProperty(content);
                            }
                        }
                    }
                }
                batchedData.Add(document);
            }
            if (batchedData.Any())
            {
                Console.WriteLine($"Inserting {batchedData.Count} records.");
                await InsertRecordsAsync(batchedData);
            }
        }
        public override async Task InsertRecordsAsync(List<BaseData> records)
        {
            await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();
            string commandText = @"INSERT INTO account_data (
                company_number, company_name, is_dormant, start_date, end_date, balance_sheet_date, entity_trading_status, as_agent, description_principal_activities
            ) VALUES (
                # TOOD @company_number, @company_name, @isdormant, @startdate, @enddate, @balancesheetdate, @entitytradingstatus, @asagent, @descriptionprincipalactivities
            )";
            
            foreach (BaseData record in records)
            {
                using var command = new NpgsqlCommand(commandText, connection, transaction);
                PropertyInfo[] properties = record.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in properties)
                {
                    string propName = property.Name.ToLower();
                    object value = property.GetValue(record);
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
    class PSCDataHandler : DataHandler
    {
        public PSCDataHandler(NpgsqlConnection connection) : base(connection)
        {
            
        }
        public async Task LoadJSONAndStoreData(string directoryPath)
        {
            List<BaseData> batchedData = new List<BaseData>();
            string[] filePaths = GetAllFilePathsInDirectory(directoryPath, "json");
            Console.WriteLine("Downloading and storing JSON data");
            foreach (string filePath in filePaths)
            {
                string jsonContent = await File.ReadAllTextAsync(filePath);
                using JsonDocument doc = JsonDocument.Parse(jsonContent);
                string root = doc.RootElement;
                string data = root.GetProperty("data");
                var record = new CompanyData
                {
                    CompanyNumber = root.GetString("company_number"),
                    AddressLine1 = data.GetProperty("address").GetString("address_line_1"),
                    Country = data.GetProperty("address").GetString("country"),
                    Locality = data.GetProperty("address").GetString("locality"),
                    PostalCode = data.GetProperty("address").GetString("postal_code"),
                    Premises = data.GetProperty("address").GetString("premises"),
                    CeasedOn = data.GetString("ceased_on"),
                    CountryOfResidence = data.GetString("country_of_residence"),
                    DateOfBirthMonth = data.GetProperty("date_of_birth").GetInt32("month"),
                    DateOfBirthYear = data.GetProperty("date_of_birth").GetInt32("year"),
                };
                {
                    batchedData.Add(record);
                    Console.WriteLine(record);
                    if (batchedData.Count >= batchSize)
                    {
                        await InsertRecordsAsync(batch);
                        batchedData.Clear();
                    }
                }
                if (batch.Any())
                {
                    await InsertRecordsAsync(batch);
                }
            }
        }

        public override async Task InsertRecordsAsync(List<BaseData> records)
        {
            await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();
            string commandText = @""; // TODO: 
            foreach (BaseData record in records)
            {
                using var command = new NpgsqlCommand(commandText, connection, transaction);
                // TODO: 
            }
            await transaction.CommitAsync();
        }
    }
}
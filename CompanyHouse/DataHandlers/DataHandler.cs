namespace CompanyHouse.DataHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;
    using CsvHelper;
    using Npgsql;

    public abstract class DataHandler(NpgsqlConnection connection, int batchSize = 100)
    {
        protected readonly NpgsqlConnection Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        protected readonly int BatchSize = batchSize;

        public async Task LoadAndStoreDataAsync(string directoryPath)
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
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                
                IEnumerable<Rows.CompanyDataRow> records = csv.GetRecords<Rows.CompanyDataRow>();
                var batch = new List<Rows.Row>();
                foreach (Rows.CompanyDataRow record in records)
                {
                    batch.Add(record);
                    if (batch.Count >= BatchSize)
                    {
                        await InsertRecordsAsync(batch);
                        batch.Clear();
                    }
                }

                if (batch.Count != 0)
                    await InsertRecordsAsync(batch);
            }
        }

        protected abstract Task InsertRecordsAsync(List<Rows.Row> records);
    }
}
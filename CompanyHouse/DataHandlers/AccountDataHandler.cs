namespace CompanyHouse.DataHandlers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Npgsql;

    sealed class AccountDataHandler(NpgsqlConnection connection, int batchSize = 100): DataHandler(connection, batchSize)
    {
        protected override async Task InsertRecordsAsync(List<Rows.Row> records)
        {
            // todo 
        }
    }
}

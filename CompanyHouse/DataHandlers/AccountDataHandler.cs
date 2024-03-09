namespace CompanyHouse.DataHandlers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Npgsql;

    class AccountDataHandler(NpgsqlConnection connection): DataHandler(connection)
    {
        protected override async Task InsertRecordsAsync(List<Rows.Row> records)
        {
            // todo 
        }
    }
}

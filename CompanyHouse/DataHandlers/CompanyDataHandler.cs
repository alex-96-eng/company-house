namespace CompanyHouse.DataHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using Npgsql;

    class CompanyDataHandler(NpgsqlConnection connection): DataHandler(connection)
    {
        protected override async Task InsertRecordsAsync(List<Rows.Row> records)
        {
            await using NpgsqlTransaction transaction = await Connection.BeginTransactionAsync();

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

            foreach (Rows.Row record in records)
            {
                await using var command = new NpgsqlCommand(commandText, Connection, transaction);
                PropertyInfo[] properties = record.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (PropertyInfo property in properties)
                {
                    string propName = property.Name.ToLower();
                    object value = property.GetValue(record);
                    
                    // Should probs live in the constructor
                    if (value is Uri uriValue)
                        value = uriValue.ToString();
                    else if (value is string stringValue && string.IsNullOrEmpty(stringValue))
                        value = DBNull.Value;
                    
                    command.Parameters.AddWithValue($"@{propName}", value ?? DBNull.Value);
                }
                await command.ExecuteNonQueryAsync();
            }
            await transaction.CommitAsync();
        }
    }
}
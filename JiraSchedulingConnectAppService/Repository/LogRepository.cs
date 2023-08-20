using JiraSchedulingConnectAppService.Repository.Interfaces;
using ModelLibrary.DTOs;
using System.Data;
using System.Data.SqlClient;

namespace JiraSchedulingConnectAppService.Repository
{
    public class LogRepository : ILogRepository
    {
        private const string TABLE = "LogS";
        private readonly string ConnectionString;
        private readonly HashSet<int> transientSqlErrors = new()
        {
            -2, 258, 4060
        };
        private const int MAX_RETRIES = 3;
        private const int RETRY_SECONDS = 5;
        public LogRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public async Task Insert(List<LogMessage> logMessages)
        {
            DataTable table = new();
            table.TableName = TABLE;

            table.Columns.Add(nameof(LogMessage.Timestamp), typeof(DateTimeOffset));
            table.Columns.Add(nameof(LogMessage.LogLevel), typeof(string));
            table.Columns.Add(nameof(LogMessage.Message), typeof(string));

            table.Columns.Add(nameof(LogMessage.ExceptionSource), typeof(string));
            table.Columns.Add(nameof(LogMessage.ExceptionType), typeof(string));
            table.Columns.Add(nameof(LogMessage.ThreadId), typeof(int));
            foreach (var logMessage in logMessages)
            {
                var row = table.NewRow();

                row[nameof(LogMessage.Timestamp)] = logMessage.Timestamp;

                row[nameof(LogMessage.LogLevel)] = logMessage.LogLevel;
                row[nameof(LogMessage.Message)] = logMessage.Message ?? (object)DBNull.Value;

                row[nameof(LogMessage.ExceptionSource)] = logMessage.ExceptionSource ?? (object)DBNull.Value;
                row[nameof(LogMessage.ExceptionType)] = logMessage.ExceptionType ?? (object)DBNull.Value;


                row[nameof(LogMessage.ThreadId)] = logMessage.ThreadId;

                table.Rows.Add(row);
            }

            await BulkInsertWithRetries(table);
        }

        private async Task BulkInsertWithRetries(DataTable table)
        {
            int attempts = 1;
            while (true)
            {
                try
                {
                    using (var sqlBulkCopy = new SqlBulkCopy(ConnectionString))
                    {
                        sqlBulkCopy.DestinationTableName = table.TableName;
                        sqlBulkCopy.ColumnMappings.Add(nameof(LogMessage.Timestamp), "Timestamp");
                        sqlBulkCopy.ColumnMappings.Add(nameof(LogMessage.LogLevel), "LogLevel");
                        sqlBulkCopy.ColumnMappings.Add(nameof(LogMessage.Message), "Message");
                        sqlBulkCopy.ColumnMappings.Add(nameof(LogMessage.ExceptionSource), "ExceptionSource");
                        sqlBulkCopy.ColumnMappings.Add(nameof(LogMessage.ExceptionType), "ExceptionType");
                        sqlBulkCopy.ColumnMappings.Add(nameof(LogMessage.ThreadId), "ThreadId");

                        await sqlBulkCopy.WriteToServerAsync(table);
                        return;
                    }
                }
                catch (SqlException sqlEx)
                when (transientSqlErrors.Contains(sqlEx.Number) && attempts <= MAX_RETRIES)
                {
                    Console.WriteLine($"Transient SQL error. Retrying in {RETRY_SECONDS} seconds");
                    await Task.Delay(TimeSpan.FromSeconds(RETRY_SECONDS));
                    attempts++;
                }
            }
        }


    }
}
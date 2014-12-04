using System;
using System.Data.SqlClient;

namespace DeployDB.SqlServer
{
    internal static class SqlServerExtensions
    {
        public static DateTime? GetNullableDatetime(this SqlDataReader reader, int columnIndex)
        {
            if (reader.IsDBNull(columnIndex))
                return null;
            return reader.GetDateTime(columnIndex);
        }
    }
}

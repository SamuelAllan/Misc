using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeployDB.SqlServer
{
    public class SqlServerDatabase : Database, SchemaHistory, IDisposable
    {
        private readonly SqlConnection connection;
        private bool disposed;

        public SqlServerDatabase(string connectionString)
        {
            connection = new SqlConnection(connectionString);
            connection.Open();
        }

        public void ApplyScript(string script)
        {
            List<string> scriptParts = ScriptSplitter.Split(script);

            using (SqlTransaction transaction = connection.BeginTransaction())
            {
                foreach(string part in scriptParts)
                {
                    SqlCommand create = new SqlCommand(part, connection, transaction);
                    create.ExecuteNonQuery();
                }
                transaction.Commit();
            }
        }

        public void EnsureHistoryDeployed()
        {
            using(SqlTransaction transaction = connection.BeginTransaction())
            {
                SqlCommand create = new SqlCommand(SchemaHistorySql.CreateSchemaHistoryIfNotExist, connection, transaction);
                create.ExecuteNonQuery();
                transaction.Commit();
            }
        }

        public IEnumerable<AppliedScript> GetAppliedScripts()
        {
            SqlCommand create = new SqlCommand(SchemaHistorySql.GetAppliedScripts.Sql, connection);
            var raw = create.ExecuteReader();

            while(raw.Read())
            {
                string name = raw.GetString(SchemaHistorySql.GetAppliedScripts.Cols.Name);
                DateTime deployed = raw.GetDateTime(SchemaHistorySql.GetAppliedScripts.Cols.Deployed);
                DateTime? rolledBack = raw.GetNullableDatetime(SchemaHistorySql.GetAppliedScripts.Cols.RolledBack);

                yield return new AppliedScript(name, deployed, rolledBack);
            }
        }

        public void SaveAppliedScript(AppliedScript appliedScript)
        {
            using (SqlTransaction transaction = connection.BeginTransaction())
            {
                SqlCommand save = new SqlCommand(SchemaHistorySql.SaveAppliedScript.Sql, connection, transaction);
                save.Parameters.AddWithValue(SchemaHistorySql.SaveAppliedScript.Args.Name, appliedScript.Name);
                save.Parameters.AddWithValue(SchemaHistorySql.SaveAppliedScript.Args.Deployed, appliedScript.DeployTime);
                save.Parameters.AddWithValue(SchemaHistorySql.SaveAppliedScript.Args.RolledBack, appliedScript.RollbackTime);

                save.ExecuteNonQuery();
                transaction.Commit();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            disposed = true;

            if (disposing)
            {
                connection.Dispose();
            }
        }
    }
}

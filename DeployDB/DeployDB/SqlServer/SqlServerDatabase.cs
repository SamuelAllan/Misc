using System;
using System.Collections.Generic;
using System.Data.SqlClient;

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
            SqlCommand get = new SqlCommand(SchemaHistorySql.GetAppliedScripts.Sql, connection);
            using (var raw = get.ExecuteReader())
            {
                while (raw.Read())
                {
                    string name = raw.GetString(SchemaHistorySql.GetAppliedScripts.Cols.Name);
                    DateTime deployed = raw.GetDateTime(SchemaHistorySql.GetAppliedScripts.Cols.Deployed);
                    DateTime? rolledBack = raw.GetNullableDatetime(SchemaHistorySql.GetAppliedScripts.Cols.RolledBack);

                    yield return new AppliedScript(name, deployed, rolledBack);
                }
            }
        }

        public AppliedScript GetDeployedScript(string name)
        {
            SqlCommand get = new SqlCommand(SchemaHistorySql.GetDeployedScript.Sql, connection);
            get.Parameters.AddWithValue(SchemaHistorySql.SaveAppliedScript.Args.Name, name);
            using (var raw = get.ExecuteReader())
            {
                if (!raw.Read())
                    return null;
                
                DateTime deployed = raw.GetDateTime(SchemaHistorySql.GetDeployedScript.Cols.Deployed);
                AppliedScript result = new AppliedScript(name, deployed, null);

                if (raw.Read())
                    throw new Exception("Multiple deployed scripts named: " + name + ".");

                return result;
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

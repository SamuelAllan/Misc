namespace DeployDB.SqlServer
{
    internal static class SchemaHistorySql
    {
        public const string CreateSchemaHistoryIfNotExist = @"
if not exists (select 1 from INFORMATION_SCHEMA.SCHEMATA where SCHEMA_NAME = 'SchemaHistory')
begin
    exec ('
    create schema SchemaHistory authorization dbo
        create table AppliedScript
        (
            Name nvarchar(255) not null,
            Deployed datetime not null,
            RolledBack datetime null,
            constraint PK_AppliedScript primary key (Deployed, Name)
        )')
end
";

        public static class GetAppliedScripts
        {
            public const string Sql = @"
select Name, Deployed, RolledBack
from SchemaHistory.AppliedScript
";

            public static class Cols
            {
                public const int Name = 0;
                public const int Deployed = 1;
                public const int RolledBack = 2;
            }
        }

        public static class GetDeployedScript
        {
            public const string Sql = @"
select Name, Deployed, RolledBack
from SchemaHistory.AppliedScript
where Name = @name and RolledBack is null
";

            public static class Cols
            {
                public const int Name = 0;
                public const int Deployed = 1;
                public const int RolledBack = 2;
            }

            public static class Args
            {
                public const string Name = "@name";
            }
        }

        public static class SaveAppliedScript
        {
            public const string Sql = @"
if exists (select 1 from SchemaHistory.AppliedScript where Name = @name and Deployed = @deployed)
begin
    update SchemaHistory.AppliedScript
    set RolledBack = @rolledBack
    where Name = @name and Deployed = @deployed
end
else
begin
    insert into SchemaHistory.AppliedScript
    (
        Name,
        Deployed,
        RolledBack
    )
    values
    (
        @name,
        @deployed,
        @rolledBack
    )
end
";
            public static class Args
            {
                public const string Name = "@name";
                public const string Deployed = "@deployed";
                public const string RolledBack = "@rolledBack";
            }
        }
    }
}

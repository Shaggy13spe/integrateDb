using System;

using integrateDb.Interfaces;
using integrateDb.Managers;
using integrateDb.SqlServer;

namespace integrateDbConsole {
    class Program {

        static void Main(string[] args) {
            Console.WriteLine("Hello World");
            var dbManager = SqlDatabaseManager.Initialize(new DatabaseManagerConfig { SerializeTests = false }, "my-test.json")
                .PassConnectionString("Server=localhost, 1433;Database=integrateDbTest;User Id=sa;Password=Gr3@tM!nds;")
                .ExecuteSqlScripts("truncate-my-table")
                .LoadTables("my-table-seed");

            dbManager.AssertDatasetVsScript("my-table-updated", "select-my-name-my-table", "Id");

            Console.WriteLine("Goodbye World");
        }
    }
}

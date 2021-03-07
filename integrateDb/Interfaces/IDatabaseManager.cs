using System;

using integrateDb.Managers;

namespace integrateDb.Interfaces {
    public interface IDatabaseManager {
        DatabaseManagerConfig Config { get; }

        IDatabaseManager ExecuteSqlScripts(params string[] scriptNames);
        IDatabaseManager LoadTables(params string[] datasetNames);

        IDatabaseManager RegisterFormatter(Type type, Func<object, string> func);
        IDatabaseManager RegisterFormatter(Type type, IColumnFormatter formatter);
        IDatabaseManager RegisterFormatter(string column, Func<object, string> func);
        IDatabaseManager RegisterFormatter(string column, IColumnFormatter formatter);

        void Completed();
        void AssertDatasetVsScript(string expectedDatasetName, string actualScriptName, bool sorted, string key, params string[] otherKeys);
        void AssertDatasetVsScript(string expectedDatasetName, string actualScriptName, string key, params string[] otherKeys);
    }
}

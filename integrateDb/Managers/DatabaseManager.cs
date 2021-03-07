using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;

using integrateDb.Interfaces;

namespace integrateDb.Managers {
    public abstract class DatabaseManager<TDatabaseClient> : IDatabaseManager
        where TDatabaseClient : IDatabaseClient {

        private readonly FormatterManager formatterManager = new();
        private readonly List<KeyValuePair<string, JsonDocument>> inputs = new();
        private static readonly object syncObject = new();

        protected TDatabaseClient DatabaseClient { get; set; }

        public DatabaseManagerConfig Config { get; } = DatabaseManagerConfig.GlobalConfig;

        public DatabaseManager(DatabaseManagerConfig config) {
            Config = config;
        }


        public IDatabaseManager ExecuteSqlScripts(params string[] scriptNames) {
            ValidateDependencies();

            foreach(var scriptName in scriptNames) {
                var script = FindScript(scriptName);
                DatabaseClient.ExecuteCommand(script.GetProperty("value").GetString());
            }

            return this;
        }

        public void Completed() {
            EndTest();
        }

        public IDatabaseManager LoadTables(params string[] datasetNames) {
            ValidateDependencies();

            foreach(var datesetName in datasetNames) {
                var dataset = FindDataset(datesetName);
                DatabaseClient.InsertTableData(dataset);
            }

            return this;
        }

        public void AssertDatasetVsScript(string expectedDatasetName, string actualScriptName, string key, params string[] otherKeys) {
            AssertDatasetVsScript(expectedDatasetName, actualScriptName, false, key, otherKeys);
        }

        public void AssertDatasetVsScript(string expectedDatasetName, string actualScriptName, bool sorted, string key, params string[] otherKeys) {
            ValidateDependencies();

            var expectedData = FindDataset(expectedDatasetName);
            var actualDataScript = FindScript(actualScriptName).GetProperty("value").ToString();
            var actualData = DatabaseClient.ReadTableData(actualDataScript, formatterManager);

            string[] keys = new string[] { key };
            if(otherKeys != null)
                keys = keys.Union(otherKeys).ToArray();

            DatabaseManagerHelper.CompareDatasets(expectedData, actualData, keys, sorted, false);
        }

        public IDatabaseManager RegisterFormatter(Type type, Func<object, string> func) {
            throw new NotImplementedException();
        }

        public IDatabaseManager RegisterFormatter(Type type, IColumnFormatter formatter) {
            throw new NotImplementedException();
        }

        public IDatabaseManager RegisterFormatter(string column, Func<object, string> func) {
            throw new NotImplementedException();
        }

        public IDatabaseManager RegisterFormatter(string column, IColumnFormatter formatter) {
            throw new NotImplementedException();
        }

        protected void BeginTest() {
            if(Config.SerializeTests)
                Monitor.Enter(syncObject);
        }

        protected void EndTest() {
            if(Config.SerializeTests)
                Monitor.Exit(syncObject);
        }

        protected DatabaseManager<TDatabaseClient> Load(string fileName) {
            var jsonFile = File.ReadAllText(fileName);
            var json = JsonDocument.Parse(jsonFile);
            var input = new KeyValuePair<string, JsonDocument>(fileName, json);
            inputs.Add(input);
            return this;
        }

        protected virtual void ValidateDependencies() {
            if(inputs.Count == 0)
                throw new InvalidOperationException("Input file not specified");
        }

        private JsonElement FindScript(string scriptName) {
            foreach(var input in inputs) {
                var jsonDoc = input.Value;
                var root = jsonDoc.RootElement;
                var scripts = root.GetProperty("scripts");

                foreach(var script in scripts.EnumerateArray()) {
                    if(script.GetProperty("name").GetString() == scriptName)
                        return script;
                }
            }

            throw new InvalidOperationException($"Script '{scriptName}' not found");
        }

        private JsonElement FindDataset(string datasetName) {
            foreach(var input in inputs) {
                var jsonDoc = input.Value;
                var root = jsonDoc.RootElement;
                var datasets = root.GetProperty("datasets");

                foreach(var dataset in datasets.EnumerateArray()) {
                    if(dataset.GetProperty("name").GetString() == datasetName)
                        return dataset;
                }
            }

            throw new InvalidOperationException($"Dataset '{datasetName} not found");
        }
    }
}

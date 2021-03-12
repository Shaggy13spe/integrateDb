using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;

using integrateDb.Interfaces;
using integrateDb.Models;

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
            foreach(var script in from scriptName in scriptNames
                                  let script = FindScript(scriptName)
                                  select script) {
                DatabaseClient.ExecuteCommand(script.Value);
            }

            return this;
        }

        public void Completed() {
            EndTest();
        }

        public IDatabaseManager LoadTables(params string[] datasetNames) {
            ValidateDependencies();
            foreach(var dataset in from datesetName in datasetNames
                                   let dataset = FindDataset(datesetName)
                                   select dataset) {
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
            var actualDataScript = FindScript(actualScriptName).Value;
            var actualData = DatabaseClient.ReadTableData(actualDataScript, formatterManager);

            var keys = new string[] { key };
            if(otherKeys != null)
                keys = keys.Union(otherKeys).ToArray();

            DatabaseManagerHelper.CompareDatasets(expectedData, actualData, keys, sorted, false);
        }

        public IDatabaseManager RegisterFormatter(Type type, Func<object, string> func) {
            formatterManager.Register(type, func);
            return this;
        }

        public IDatabaseManager RegisterFormatter(Type type, IColumnFormatter formatter) {
            formatterManager.Register(type, formatter);
            return this;
        }

        public IDatabaseManager RegisterFormatter(string columnName, Func<object, string> func) {
            formatterManager.Register(columnName, func);
            return this;
        }

        public IDatabaseManager RegisterFormatter(string columnName, IColumnFormatter formatter) {
            formatterManager.Register(columnName, formatter);
            return this;
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

        private Script FindScript(string scriptName) {
            foreach(var script in from input in inputs
                                  let jsonDoc = input.Value
                                  let root = jsonDoc.RootElement
                                  let scripts = root.GetProperty("scripts")
                                  from script in scripts.EnumerateArray()
                                  where script.GetProperty("name").GetString() == scriptName
                                  select script) {
                return Script.Parse(script);
            }

            throw new InvalidOperationException($"Script '{scriptName}' not found");
        }

        private Dataset FindDataset(string datasetName) {
            foreach(var dataset in from input in inputs
                                   let jsonDoc = input.Value
                                   let root = jsonDoc.RootElement
                                   let datasets = root.GetProperty("datasets")
                                   from dataset in datasets.EnumerateArray()
                                   where dataset.GetProperty("name").GetString() == datasetName
                                   select dataset) {
                return Dataset.Parse(dataset);
            }

            throw new InvalidOperationException($"Dataset '{datasetName} not found");
        }
    }
}

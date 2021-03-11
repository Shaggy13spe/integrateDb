using System.Text.Json;

using integrateDb.Managers;
using integrateDb.Models;

namespace integrateDb.Interfaces {

    /// <summary>
    /// Provides an interface for initialzing classes that execute SQL commands
    /// </summary>
    public interface IDatabaseClient {

        /// <summary>
        /// Executes a SQL command
        /// </summary>
        /// <param name="command">A SQL command to execute</param>
        void ExecuteCommand(string command);

        /// <summary>
        /// Inserts data into a table
        /// </summary>
        /// <param name="dataset">the data that is to be inserted</param>
        void InsertTableData(Dataset dataset);

        /// <summary>
        /// Executes a SQL command that returns an object from the data
        /// </summary>
        /// <param name="command"></param>
        /// <param name="formatter"></param>
        Dataset ReadTableData(string command, FormatterManager formatter);
    }
}

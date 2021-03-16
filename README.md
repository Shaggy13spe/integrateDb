# integrateDb - Database Integration testing for .Net
[![Build status](https://dev.azure.com/GreatMinds4U/integrateDb/_apis/build/status/Shaggy13spe.integrateDb)](https://dev.azure.com/GreatMinds4U/integrateDb/_build/latest?definitionId=2)

Based on the [DbSafe](https://github.com/dbsafe/dbsafe) project, this library provides for the ability to make integration testing with your database a little easier, allowing for populating the database, executing commands, and comparing expected vs actual datasets.

## Getting Started
------------------
### Supported databases
Currently the following databases are supported, with more planned on the way:

MS SQL Server is supported by this [Nuget package](https://www.nuget.org/packages/IntegrateSqlDb/)

PostgreSQL is supported by this [Nuget package](https://www.nuget.org/packages/IntegratePgDb/)

### Initialization
Each specialized Nuget package contains an instance of a DatabaseManager class (i.e. SqlDatabaseManager, PgDatabaseManager, etc). This class contains a static `Initialize` method that returns an instance of the DatabaseManager class to be used during testing. 


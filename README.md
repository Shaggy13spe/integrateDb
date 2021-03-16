# integrateDb - Database Integration testing for .Net
[![Build status](https://dev.azure.com/GreatMinds4U/integrateDb/_apis/build/status/Shaggy13spe.integrateDb)](https://dev.azure.com/GreatMinds4U/integrateDb/_build/latest?definitionId=2)

Based on the [DbSafe](https://github.com/dbsafe/dbsafe) project, this library provides for the ability to make integration testing with your database a little easier, allowing for populating the database, executing commands, and comparing expected vs actual datasets.  This fork adds functionality not added to the original branch and thus the reason for changing the name of the library.

## Getting Started

### Supported databases
Currently the following databases are supported, with more planned on the way:

MS SQL Server is supported by this [Nuget package](https://www.nuget.org/packages/IntegrateSqlDb/)

PostgreSQL is supported by this [Nuget package](https://www.nuget.org/packages/IntegratePgDb/)

### Initialization
Each specialized Nuget package contains an instance of a DatabaseManager class (e.g. SqlDatabaseManager, PgDatabaseManager, etc). This class contains a static `Initialize` method that returns an instance of the DatabaseManager class to be used during testing. This method also takes one or more [input files](#input-files) as a parameter.

Once initialized, you can set the connection string for the manager to utilize, execute any sql scripts, and load any tables needed for the current test case.

For instance, assuming you were testing with a SQL Server database, your code may look like this:

```csharp
public class ProductDbTest {
  private IDatabaseManager databaseManager;

  // ...

  public ProductDbTest() {
    databaseManager = SqlDatabaseManager.Initialize("product-db-test.json");
    databaseManager.SetConnectionString("ProductEntities");
    databaseManager.ExecuteScripts("delete-products", "delete-categories", "delete-suppliers", "reseed-product-table");
    databaseManager.LoadTables("categories", "suppliers", "products");

    // ...
  }
}
```

`SetConnectionString` will use the parameter passed in to load the connection string from the projects configuration file(s). See the section [Connection String](#connection-string) for more options.

`ExecuteScripts` can be used to clean the tables before and/or after the test data is loaded.

`LoadTables` loads the database with the data given in the input file(s) passed in to the `Initialize` method.

The initialization methods can also be chained.

```csharp
public ProductDbTest() {
  databaseManager = SqlDatabaseManager.Initialize("product-db-test.json")
    .SetConnectionString("ProductEntities")
    .ExecuteScripts("delete-products", "delete-categories", "delete-suppliers", "reseed-product-table")
    .LoadTables("categories", "suppliers", "products");
}
  ```
  
  ### Configuration
  
  `Initialize` has an overload that takes an instance of `DatabaseManagerConfig`.

  ```csharp
  public void ProductDbTest() {
    var databaseConfig = new DatabaseManagerConfig { SerializeTests = true, ReuseConnection = true };

    // ...
  }
  ```

When calling `Initialize` without passing a config object, a default configuration is used. `DatabaseManager.Config.GlobalConfig` is the instance of the default configuration.

**Properties of DatabaseManagerConfig**
| Property Name   | Type    | Defaults | Description                                                     |
|-----------------|---------|:--------:|-----------------------------------------------------------------|
| SerializeTests  | boolean | **true**     | Indicates whether the tests should be serialized |
| ReuseConnection | boolean | **false**     | Indicates whether the database connection should be reused |

  ### Input Files

  integrateDb uses one or more json files with SQL scripts and datasets. The input file has a `scripts` array and a `datasets` array. 
  Script commands can be executed at any time during the test (e.g. cleaning tables, selecting actual data, etc). Datasets are used to populate a table or can be used as expected data, for instance after running an UPDATE script command.

  The `scripts` array holds objects with the following properties:
    
  > `name`: unique identifier of script object
  >
  > `value`: the command text that will be executed when running the script

  The `datasets` array holds objects with following properties:

  > `name`: unique identifier of dataset object
  >
  > `table`: the table name that this dataset will load in to
  > 
  > `schema`: **OPTIONAL** - the schema used for this table (defaults to ***dbo***)
  > 
  > `setIdentityInsert`: **OPTIONAL** - sets whether or not IDENTITY_INSERT should be set to on before loading (defaults to ***false***) 
  >
  > `rows`: an array that holds objects that define rows of the dataset. Each row object contains an array of `columns`
  
  The `columns` array holds objects with the following properties:

  > `name`: the name of the column
  >
  > `value`: the value for the column
  >
  > `converterType`: **OPTIONAL** - the data type to use for an explicit conversion/cast when loading data

  An example of an input file would be:

  ```json
  {
    "scripts": [
        {
            "name": "delete-products",
            "value": "DELETE [dbo].[Product];"
        },
        {
            "name": "delete-categories",
            "value": "DELETE [dbo].[Category];"
        }
    ],
    "datasets": [
        {
            "name": "categories",
            "table": "Category",
            "setIdentityInsert": "true",
            "rows": [
                {
                    "columns": [
                        {
                            "name": "Id",
                            "value": "1"
                        },
                        {
                            "name": "Name",
                            "value": "category-1"
                        }
                    ]
                },
                {
                    "columns": [
                        {
                            "name": "Id",
                            "value": "2"
                        },
                        {
                            "name": "Name",
                            "value": "category-2"
                        }
                    ]
                },
                {
                    "columns": [
                        {
                            "name": "Id",
                            "value": "3"
                        },
                        {
                            "name": "Name",
                            "value": "category-3"
                        }
                    ]
                }
            ]
        },
        {
            "name": "suppliers",
            "table": "Supplier",
            "rows": [
                {
                    "columns": [
                        {
                            "name": "Id",
                            "value": "1"
                        },
                        {
                            "name": "Name",
                            "value": "supplier-1"
                        },
                        {
                            "name": "ContactName",
                            "value": "contact-name-1"
                        },
                        {
                            "name": "ContactPhone",
                            "value": "100-200-0001"
                        },
                        {
                            "name": "ContactEmail",
                            "value": "email-1@test.com"
                        }
                    ]
                },{
                    "columns": [
                        {
                            "name": "Id",
                            "value": "2"
                        },
                        {
                            "name": "Name",
                            "value": "supplier-2"
                        },
                        {
                            "name": "ContactName",
                            "value": "contact-name-2"
                        },
                        {
                            "name": "ContactPhone",
                            "value": "100-200-0002"
                        },
                        {
                            "name": "ContactEmail",
                            "value": "email-2@test.com"
                        }
                    ]
                },{
                    "columns": [
                        {
                            "name": "Id",
                            "value": "3"
                        },
                        {
                            "name": "Name",
                            "value": "supplier-3"
                        },
                        {
                            "name": "ContactName",
                            "value": "contact-name-3"
                        },
                        {
                            "name": "ContactPhone",
                            "value": "100-200-0003"
                        },
                        {
                            "name": "ContactEmail",
                            "value": "email-3@test.com"
                        }
                    ]
                }
            ]
        }
    ]
}
```

### Test Completion
By default, test executions are serialized. The `Initialize` method gets a lock and other tests will have to wait for the test that has the lock to complete. At the end of each test, the method `Completed` must be called to release the lock.

The serialization of the tests is necessary to avoid tests competing for the same data when running at the same time, resulting in deadlocks. If the tests are designed to use different data, they can be run in parallel by setting the configuration property `SerializeTests` to ***false***.

```csharp
public void Dispose() {
  databaseManager?.Completed();
}
```

### Connection String
`SetConnectionString` loads a connection string from either an app.config or appsettings.json file. The connection string name must be defined in that file.

`PassConnectionString` allows you to pass in a connection string. This method can be used if the project does not have either an app.config or appsettings.json file.

The connection string used by `SqlDatabaseManager` is an ordinal ADO.NET connection string and cannot include any specific Entity Framework (or other ORM) metadata.

## Testing

integrateDb supports writing unit tests using the AAA (Arrange, Act, Assert) pattern.

**Arrange** initializes objects and sets the value of the data that is passed to the method under test.

**Act** invokes the method under test with the arranged parameters

**Assert** verifies that the action of the method under test behaves as expected

The method `AssertDatabaseVsScript` on the DatabaseManager instance can be used to compare expected data vs actual data in the database

### Simple test
This test verifies that the method `UpdateSupplier` updates a record in the database. `UpdateSupplier` accepts a `Supplier` object that will be used to update the database based on the `Id`. Here, the object `supplier2` represents the supplier with the `Id` 2.

***<small>The tests written here use Xunit (and the [FluentAssertions](https://fluentassertions.com/) library), but you can use any .Net testing framework</small>***

```csharp
[Fact]
public void UpdateSupplier_GivenSupplier_ThenUpdateSupplierAndReturnTrue() {
  var supplier2 = new Supplier {
    Id = 2,
    Name = "supplier-2-updated",
    ContactName = "contact-name-2-updated",
    ContactPhone = "100-200-9999",
    ContactEmail = "email-2-updated@test.com"
  };

  var actual = target.UpdateSupplier(supplier2);

  actual.Should().BeTrue();
  databaseManager.AssertDatasetVsScript("suppliers-updated", "select-all-suppliers", "Id");
}
```

During initialization of the DatabaseManager, the table `Suppliers` was populated with the following dataset:

```json
  {
    "name": "suppliers",
    "table": "Supplier",
    "setIdentityInsert": true,
    "rows": [
      {
        "columns": [
          {
            "name": "Id",
            "value": "1"
          },
          {
            "name": "Name",
            "value": "supplier-1"
          },
          {
            "name": "ContactName",
            "value": "contact-name-1"
          },
          {
            "name": "ContactPhone",
            "value": "100-200-0001"
          },
          {
            "name": "ContactEmail",
            "value": "email-1@test.com"
          }
        ]
      },
      {
        "columns": [
          {
            "name": "Id",
            "value": "2"
          },
          {
            "name": "Name",
            "value": "supplier-2"
          },
          {
            "name": "ContactName",
            "value": "contact-name-2"
          },
          {
            "name": "ContactPhone",
            "value": "100-200-0002"
          },
          {
            "name": "ContactEmail",
            "value": "email-2@test.com"
          }
        ]
      },
      {
        "columns": [
          {
            "name": "Id",
            "value": "3"
          },
          {
            "name": "Name",
            "value": "supplier-3"
          },
          {
            "name": "ContactName",
            "value": "contact-name-3"
          },
          {
            "name": "ContactPhone",
            "value": "100-200-0003"
          },
          {
            "name": "ContactEmail",
            "value": "email-3@test.com"
          }
        ]
      }
    ]
  }
```

After `UpdateSupplier(supplier2)` is executed the method `AssertDatasetVsScript` asserts that the data in the dataset `suppliers-updated` matches the data returned by the script `select-all-suppliers`. The column `Id` is used as the key value.

```json
{
  "scripts": [
    ...
    {
      "name": "select-all-suppliers",
      "value": "SELECT * FROM [dbo].[Supplier];"
    }
    ...
  ],
  "datasets": [
    ...
    {
      "name": "suppliers-updated",
      "table": "Supplier",
      "rows": [
        {
          "columns": [
            {
              "name": "Id",
              "value": "1"
            },
            {
              "name": "Name",
              "value": "supplier-1"
            },
            {
              "name": "ContactName",
              "value": "contact-name-1"
            },
            {
              "name": "ContactPhone",
              "value": "100-200-0001"
            },
            {
              "name": "ContactEmail",
              "value": "email-1@test.com"
            }
          ]
        },
        {
          "columns": [
            {
              "name": "Id",
              "value": "2"
            },
            {
              "name": "Name",
              "value": "supplier-2-updated"
            },
            {
              "name": "ContactName",
              "value": "contact-name-2-updated"
            },
            {
              "name": "ContactPhone",
              "value": "100-200-9999"
            },
            {
              "name": "ContactEmail",
              "value": "email-2-updated@test.com"
            }
          ]
        },
        {
          "columns": [
            {
              "name": "Id",
              "value": "3"
            },
            {
              "name": "Name",
              "value": "supplier-3"
            },
            {
              "name": "ContactName",
              "value": "contact-name-3"
            },
            {
              "name": "ContactPhone",
              "value": "100-200-0003"
            },
            {
              "name": "ContactEmail",
              "value": "email-3@test.com"
            }
          ]
        }
      ]
    }
    ...
  ]
}
```

## Column Formatters

Values read from a table are converted to a `string` to create an actual local dataset. The conversion depends on the local settings.

**money and decimal data types**

The SQL Server data type `money` converts to a `string` with four decimal places. The `decimal` data type converts using the number of decimal places of the type, e.g. `101.10 is converted to `101.1000`.

**datetime(s) data type**

The SQL Server `datetime2` data type converts to `1/1/2000 12:00:00 AM` by default.

Using custom formatters avoids having to write datasets with meaningless decimal places or dates with `00:00:00` in the time part.

Use the `RegisterFormatter` method on the `DatabaseManager` object to register a formatter.

There are 6 overloads to the `RegisterFormatter` method:

```csharp
IDatabaseManager RegisterFormatter(string tableName, string columnName, Func<object, string> func);
IDatabaseManager RegisterFormatter(string tableName, string columnName, IColumnFormatter formatter);
IDatabaseManager RegisterFormatter(string columnName, Func<object, string> func);
IDatabaseManager RegisterFormatter(string columnName, IColumnFormatter formatter);
IDatabaseManager RegisterFormatter(Type type, Func<object, string> func);
IDatabaseManager RegisterFormatter(Type type, IColumnFormatter formatter);
```

#### Formatters
A formatter can be a class that implements the interface `IColumnFormatter` or a function that takes an `object` and returns a `string`

A formatter can be registered for:

**Table & column name** - The formatter will be used for a specific column in a specific table.

**Column name** - The formatter will be used for a column in any table that matches the name

**Type** - the formatter will be used for any columns that are of a specific type in any table

The order of precedence is: **table & column name** --> **column name** -> **type**

There are two pre-defined formatters in integrateDb: `DecimalFormatter` & `DateTimeFormatter`

In this example, `DateTimeFormatter` is used to format all the columns that are of type `DateTime` using the format `"yyyy-MM-dd HH:mm:ss"` and to format all the columns called `ReleaseDate` to truncate the time part. `DecimalFormatter` is used to format the columns of type `decimal` using two decimal places.

```csharp
databaseManager.RegisterFormatter(typeof(DateTime), new DateTimeFormatter("yyyy-MM-dd HH:mm:ss"))
  .RegisterFormatter("ReleaseDate", new DateTimeFormatter("yyyy-MM-dd"))
  .RegisterFormatter(typeof(decimal), new DecimalFormatter("0.00"));
  ```

  To create a custom formatter, you can create a class in your test project that implements the `IColumnFormatter` interface, or you can pass in a function like the following example:

  ```csharp
  // convert byte array (varbinary in SQL Server) results to base64 encoded string
  databaseManager.RegisterFormatter(typeof(byte[]), (value) => {
      if(value.GetType() == typeof(byte[]))
          return Convert.ToBas64String(value);
      else
          return value;
  });
```

## Please help out
This project is under development. Feedback and suggestions are very welcome and I encourage you to use the [Issues list](https://github.com/shaggy13spe/integrateDb/issues) on Github to provide that feedback.

Feel free to fork this repository and to commit your additions. For a list of all contributors, please see the [AUTHORS](https://github.com/shaggy13spe/integrateDb/blob/main/AUTHORS) file.

## Contributing
If you are interested in fixing issues and contributing directly to the code base:

Fork the repository. Then, run:

`git clone git@github.com:<username>/integrateDb.git`

Then, do work and commit your changes, and push to your origin.

`git push`

When done, open a pull request to your fork.

## License terms
integrateDb is published under the MIT license, see the [LICENSE](https://github.com/shaggy13spe/integrateDb/blob/main/LICENSE) file.

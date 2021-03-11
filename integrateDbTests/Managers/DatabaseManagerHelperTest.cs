using System;
using System.Collections.Generic;
using System.Text.Json;

using FluentAssertions;

using integrateDb.Managers;
using integrateDb.Models;

using Xunit;
using Xunit.Sdk;

namespace integrateDbTests.Managers {
    public class DatabaseManagerHelperTest {
        [Fact]
        public void CompareDataSets_WhenDatasetsHaveDifferentTableNames_ThenFail() {
            var expectedDataset = new Dataset {
                Name = "all-my-table-records-a",
                Table = "MyTable"
            };

            var actualDataset = new Dataset {
                Name = "all-my-table-records-b",
                Table = "AnotherTable"
            };

            Action action = () => DatabaseManagerHelper.CompareDatasets(expectedDataset, actualDataset, new string[] { "key" }, false, true);
            action.Should().Throw<XunitException>().WithMessage("*because table names should be the same*");
        }

        [Fact]
        public void CompareDatasets_WhenBothDatasetsAreEmpty_ThenSucceed() {
            var expectedDataset = new Dataset {
                Name = "all-my-table-records-a",
                Table = "MyTable",
                Rows = new()
            };

            var actualDataset = new Dataset {
                Name = "all-my-table-records-b",
                Table = "MyTable",
                Rows = new()
            };

            Action action = () => DatabaseManagerHelper.CompareDatasets(expectedDataset, actualDataset, new string[] { "key" }, false, true);
            action.Should().NotThrow();
        }

        [Fact]
        public void CompareDatasets_WhenExpectedIsEmptyAndActualIsNot_ThenFail() {
            var expectedDataset = new Dataset {
                Name = "all-my-table-records-a",
                Table = "MyTable",
                Rows = new()
            };

            var actualDataset = new Dataset {
                Name = "all-my-table-records-b",
                Table = "MyTable",
                Rows = new() {
                    new Row {
                        Columns = new() {
                            new("Id", "1")
                        }
                    }
                }
            };

            Action action = () => DatabaseManagerHelper.CompareDatasets(expectedDataset, actualDataset, new string[] { "key" }, false, true);
            action.Should().Throw<XunitException>().WithMessage("*because expected data is empty*");
        }

        [Fact]
        public void CompareDatasets_WhenExpectedIsNotEmptyAndActualIs_ThenFail() {
            var expectedDataset = new Dataset {
                Name = "all-my-table-records-a",
                Table = "MyTable",
                Rows = new() {
                    new Row {
                        Columns = new() {
                            new("Id", "1")
                        }
                    }
                }
            };

            var actualDataset = new Dataset {
                Name = "all-my-table-records-b",
                Table = "MyTable",
                Rows = new()
            };

            Action action = () => DatabaseManagerHelper.CompareDatasets(expectedDataset, actualDataset, new string[] { "key" }, false, true);
            action.Should().Throw<XunitException>().WithMessage("*because expected data is not empty*");
        }

        [Fact]
        public void CompareDatasets_WhenNumberOfRowsDiffer_ThenFail() {
            var expectedDataset = new Dataset {
                Name = "all-my-table-records-a",
                Table = "MyTable",
                Rows = new() {
                    new Row {
                        Columns = new() {
                            new("Id", "1")
                        }
                    },
                    new Row {
                        Columns = new() {
                            new("Id", "2")
                        }
                    }
                }
            };

            var actualDataset = new Dataset {
                Name = "all-my-table-records-b",
                Table = "MyTable",
                Rows = new() {
                    new Row {
                        Columns = new() {
                            new("Id", "1")
                        }
                    }
                }
            };

            Action action = () => DatabaseManagerHelper.CompareDatasets(expectedDataset, actualDataset, new string[] { "key" }, false, false);
            action.Should().Throw<XunitException>();
        }

        [Fact]
        public void CompareDatasets_WhenActualDatasetIsMissingKeys_ThenFail() {
            var expectedDataset = new Dataset {
                Name = "all-my-table-records-a",
                Table = "MyTable",
                Rows = new() {
                    new Row {
                        Columns = new() {
                            new("Id", "1"),
                            new("OtherKey", "1")
                        }
                    }
                }
            };

            var actualDataset = new Dataset {
                Name = "all-my-table-records-b",
                Table = "MyTable",
                Rows = new() {
                    new Row {
                        Columns = new() {
                            new("Id", "1")
                        }
                    }
                }
            };

            Action action = () => DatabaseManagerHelper.CompareDatasets(expectedDataset, actualDataset, new string[] { "Id", "OtherKey" }, false, false);
            action.Should().Throw<XunitException>().WithMessage("*because actual data is missing key:*");
        }

        [Fact]
        public void CompareDatasets_WhenUnsortedDatasetsMatch_ThenSucceed() {
            var expectedDataset = new Dataset {
                Name = "all-my-table-records-a",
                Table = "MyTable",
                Rows = new() {
                    new Row {
                        Columns = new() {
                            new("Id", "1"),
                            new("Name", "John Doe")
                        }
                    },
                    new Row {
                        Columns = new() {
                            new("Id", "2"),
                            new("Name", "John Q. Public")
                        }
                    }
                }
            };

            var actualDataset = new Dataset {
                Name = "all-my-table-records-b",
                Table = "MyTable",
                Rows = new() {
                    new Row {
                        Columns = new() {
                            new("Id", "2"),
                            new("Name", "John Q. Public")
                        }
                    },
                    new Row {
                        Columns = new() {
                            new("Id", "1"),
                            new("Name", "John Doe")
                        }
                    }
                }
            };

            Action action = () => DatabaseManagerHelper.CompareDatasets(expectedDataset, actualDataset, new string[] { "Id" }, false, false);
            action.Should().NotThrow();
        }

        [Fact]
        public void CompareDatasets_WhenDatasetsMatchButNotSorted_ThenFail() {
            var expectedDataset = new Dataset {
                Name = "all-my-table-records-a",
                Table = "MyTable",
                Rows = new() {
                    new Row {
                        Columns = new() {
                            new("Id", "1"),
                            new("Name", "John Doe")
                        }
                    },
                    new Row {
                        Columns = new() {
                            new("Id", "2"),
                            new("Name", "John Q. Public")
                        }
                    }
                }
            };

            var actualDataset = new Dataset {
                Name = "all-my-table-records-b",
                Table = "MyTable",
                Rows = new() {
                    new Row {
                        Columns = new() {
                            new("Id", "2"),
                            new("Name", "John Q. Public")
                        }
                    },
                    new Row {
                        Columns = new() {
                            new("Id", "1"),
                            new("Name", "John Doe")
                        }
                    }
                }
            };

            Action action = () => DatabaseManagerHelper.CompareDatasets(expectedDataset, actualDataset, new string[] { "Id" }, true, false);
            action.Should().Throw<XunitException>();
        }
    }
}

using System;
using System.Text.RegularExpressions;
using SpreadsheetUtilities;
using System.Text.Json;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        Dictionary<string, Cell> cells;
        DependencyGraph dg;
        private bool change;


        /// <summary>
        /// Constructor that creates a new spreadsheet
        /// </summary>
        public Spreadsheet() : base("default")
        {
            //initialize variables
            cells = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
            change = false;
        }

        //Added for PS5
        public bool Changed
        {
            get
            {
                return change;
            }
            protected set
            {
                change = value;
            }
        }

        /// <summary>
        /// Added for PS5
        /// This will write the contents of the spreadsheet to a file using Json format.
        /// </summary>
        public override void Save(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new SpreadsheetReadWriteException("Filename cannot be null or empty.");
            }

            try
            {
                //Creates a list to hold cell data
                List<Dictionary<string, object>> cellDataList = new List<Dictionary<string, object>>();

                foreach (string cell in cells.Keys)
                {
                    //Creates a dictionary to represent a cell
                    Dictionary<string, object> cellData = new Dictionary<string, object>
                    {
                        { "name", cell }
                    };

                    //Checks the type of contents in the cell
                    if (cells[cell].contents is double)
                    {
                        cellData["contents"] = cells[cell].contents;
                    }
                    else if (cells[cell].contents is Formula)
                    {
                        cellData["contents"] = "=" + cells[cell].contents.ToString();
                    }
                    else
                    {
                        cellData["contents"] = (string)cells[cell].contents;
                    }

                    //Adds the cell data to the list
                    cellDataList.Add(cellData);
                }

                //Creates an anonymous type to represent the JSON structure
                var jsonData = new
                {
                    version = Version,
                    cells = cellDataList
                };

                //Serialize the data to JSON
                string json = JsonSerializer.Serialize(jsonData);

                //Write the JSON string to the file
                File.WriteAllText(filename, json);
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException(e.ToString());
            }

            Changed = false;
        }

        public override object GetCellValue(string name)
        {
            //If the name is 'null' or invalid, then it will throw an InvalidNameException
            if (ReferenceEquals(name, null) || !IsValidName(name))
            {
                throw new InvalidNameException();
            }

            //Value of Name
            Cell cell;

            //Returns the value of cell
            if (cells.TryGetValue(name, out cell))
            {
                return cell.value;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Creates a cell object
        /// </summary>
        private class Cell
        {
            public Object contents { get; private set; }
            public Object? value { get; private set; }
            string typeOfContent;
            string? typeOfValue;

            // Constructor for Strings
            public Cell(string str)
            {
                contents = str;
                value = str;
                typeOfContent = "string";
                typeOfValue = typeOfContent;
            }

            //Constructor for doubles
            public Cell(double dbl)
            {
                contents = dbl;
                value = dbl;
                typeOfContent = "double";
                typeOfValue = typeOfContent;
            }

            //Constructor for Formulas
            public Cell(Formula formula)
            {
                contents = formula;
                typeOfContent = "formula";
            }
        }

        /// <summary>
        /// Enumerates the names of all non-empty cells in spreadsheet
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return cells.Keys;
        }

        /// <summary>
        /// If name is 'null' then it will throw an null argument exception.
        /// Next, it will check if name is a valid cell name, if not then it will also
        /// throw an arugument exception for the name.
        /// If it passes the two tests, then it will return an enumeration of the name of all cells
        /// that contain 'name' in a formula.
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (ReferenceEquals(name, null))
            {
                throw new NullReferenceException();
            }

            if (!IsValidName(name))
            {
                throw new InvalidNameException();
            }
            return dg.GetDependents(name);
        }

        private bool IsValidName(string name)
        {
            if (Regex.IsMatch(name, @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*$", RegexOptions.Singleline))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// If name is null, then it will throw an null argument exception.
        /// If name is not a valid cell format, then it will throw a name exception.
        /// If it passes the tests, then it will return the contents of the cell.
        /// </summary>
        public override object GetCellContents(string name)
        {
            if (ReferenceEquals(name, null))
            {
                throw new NullReferenceException();
            }
            if (!IsValidName(name))
            {
                throw new InvalidNameException();
            }
            Cell? value;
            if (cells.TryGetValue(name, out value))
            {
                return value.contents;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// If formula or name is null, throw an argument exception, also check to see if name is
        /// valid.
        /// Also checks to see if contents of the named cell to be 'formula' would cause a
        /// circular dependency.
        /// If it passes the tests, then the contents become formula.
        /// </summary>
        public override IList<string> SetCellContents(string name, Formula formula)
        {
            if (ReferenceEquals(formula, null) || ReferenceEquals(name, null))
            {
                throw new NullReferenceException();
            }
            if (!IsValidName(name))
            {
                throw new InvalidNameException();
            }

            //temporary variable
            IEnumerable<string> oldDependees = dg.GetDependees(name);

            //replace dependents of 'name' with variables in formula
            dg.ReplaceDependees(name, formula.GetVariables());

            try
            {
                List<string> allDependee = new List<string>(GetCellsToRecalculate(name));
                Cell cell = new Cell(formula);
                //if cells already contains 'name' then it will replace the key with the new value
                if (cells.ContainsKey(name))
                {
                    cells[name] = cell;
                }
                else
                {
                    cells.Add(name, cell);
                }
                return (IList<string>)allDependee;
            }
            catch
            {
                dg.ReplaceDependees(name, oldDependees);
                throw new CircularException();
            }

        }

        /// <summary>
        /// First checks for a valid name, and null. If it passes these tests, then the contents
        /// of cell becomes a number.
        /// </summary>
        public override IList<string> SetCellContents(string name, double number)
        {
            if (!IsValidName(name))
            {
                throw new InvalidNameException();
            }
            if (ReferenceEquals(name, null))
            {
                throw new NullReferenceException();
            }
            Cell cell = new Cell(number);
            if (cells.ContainsKey(name))
            {
                cells[name] = cell;
            }
            else
            {
                cells.Add(name, cell);
            }

            dg.ReplaceDependees(name, new List<string>());

            List<string> allDependee = new List<string>(GetCellsToRecalculate(name));
            return (IList<string>)allDependee;
        }

        /// <summary>
        /// If 'text' or 'name' is null, or 'name' is not a valid name, then throw an appropriate
        /// error. If it passes, then the conetents of the cell becomes 'text'.
        /// </summary>
        public override IList<string> SetCellContents(string name, string text)
        {
            if (ReferenceEquals(text, null) || ReferenceEquals(name, null))
            {
                throw new NullReferenceException();
            }
            if (!IsValidName(name))
            {
                throw new InvalidNameException();
            }

            Cell cell = new Cell(text);
            if (cells.ContainsKey(name))
            {
                cells[name] = cell;
            }
            else
            {
                cells.Add(name, cell);
            }

            dg.ReplaceDependees(name, new List<string>());

            List<string> allDependee = new List<string>(GetCellsToRecalculate(name));
            return (IList<string>)allDependee;
        }
    }
}
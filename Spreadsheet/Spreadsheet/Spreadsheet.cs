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
        private bool changed


        /// <summary>
        /// Constructor that creates a new spreadsheet
        /// </summary>
        public Spreadsheet() : base("default")
        {
            //initialize variables
            cells = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
            changed = false;
        }

        public string GetSavedVersion(string filename)
        {
            return ReadFile(filename, true);
        }

        //Added for PS5
        public new bool Changed
        {
            get
            {
                return changed;
            }
            protected set
            {
                changed = value;
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

        /// <summary>
        /// This code will hopefully read from a Json document
        /// </summary>
        private string ReadFile(string filename, bool only_get_version)
        {
            if (ReferenceEquals(filename, null))
                throw new SpreadsheetReadWriteException("The filename cannot be null");

            if (filename.Equals(""))
                throw new SpreadsheetReadWriteException("The filename cannot be empty");

            try
            {
                string name = "";       // the name of a given cell
                string contents = "";   // the contents of the corresponding cell

                string parsedVersion = null;  // Variable to store the parsed version

                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    JsonDocument doc = JsonDocument.Parse(fs);

                    foreach (JsonProperty element in doc.RootElement.EnumerateObject())
                    {
                        switch (element.Name)
                        {
                            case "version":
                                parsedVersion = element.Value.GetString();
                                if (only_get_version)
                                    return parsedVersion;
                                // Note: Do not assign to Version directly, since it's read-only
                                break;
                            case "cells":
                                foreach (JsonElement cellElement in element.Value.EnumerateArray())
                                {
                                    foreach (JsonProperty cellProperty in cellElement.EnumerateObject())
                                    {
                                        switch (cellProperty.Name)
                                        {
                                            case "name":
                                                name = cellProperty.Value.GetString();
                                                break;
                                            case "contents":
                                                contents = cellProperty.Value.GetString();
                                                SetContentsOfCell(name, contents);
                                                break;
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            catch (JsonException e)
            {
                throw new SpreadsheetReadWriteException(e.ToString());
            }
            catch (IOException e)
            {
                throw new SpreadsheetReadWriteException(e.ToString());
            }

            return Version;
        }

        /// <summary>
        /// If 'name' is 'null' or invalid, then it will throw an InvalidNameException, if it does
        /// exist, then it will return the value of the corresponding cell. Otherwise, it will
        /// return an empty string.
        /// </summary>
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
                value = contents;
                typeOfContent = str.GetType().ToString();
                typeOfValue = typeOfContent;
            }

            //Constructor for doubles
            public Cell(double dbl)
            {
                contents = dbl;
                value = contents;
                typeOfContent = dbl.GetType().ToString();
                typeOfValue = typeOfContent;
            }

            //Constructor for Formulas
            public Cell(Formula formula, Func<string, double> lookup)
            {
                contents = formula;
                value = formula.Evaluate(lookup);
                typeOfContent = formula.GetType().ToString();
                typeOfValue = value.GetType().ToString();
            }

            //Added for PS5
            /// <summary>
            /// Helper method to re-evaluate formulas.
            /// </summary>
            /// <param name="lookup"></param>
            public void ReEvaluate(Func<string, double> lookup)
            {
                if (typeOfContent.Equals("SpreadsheetUtilities.Formula"))
                {
                    Formula same = (Formula)contents;
                    value = same.Evaluate(lookup);
                }
            }
        }

        /// <summary>
        /// Helper method for evaluating functions. It will return the value that is
        /// related to 'str' or the cell name
        /// </summary>
        private double LookupValue(string str)
        {
            Cell? cell;

            if (cells.TryGetValue(str, out cell)){
                if (cell.value is double)
                {
                    return (double)cell.value;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            else
                throw new ArgumentException();
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
        /// First checks for null and empty, if it is then it will throw appropriate exceptions.
        /// Then it will process the content based on type,
        /// If it is empty, then it will set accordingly
        /// If it is a double, it converts and sets the cell contents as a numeric value
        /// If it is a formula, it creates a Formula object and sets the cell contents accordingly
        /// Otherwise it will just treat it as a string
        /// </summary>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            //Both 'content' and 'name' cannot be null, if so, then throw an NullReferenceException
            if (ReferenceEquals(content, null) || ReferenceEquals(name, null))
            {
                throw new NullReferenceException();
            }

            //If name is empty, then throw an invalid name exception
            if (name.Equals(""))
            {
                throw new InvalidNameException();
            }

            //holds the list of dependees
            List<string> allDependents;

            //If content is a double then it will be used
            double result;

            //If it is empty, then just add it to cell
            if (content.Equals(""))
            {
                allDependents = new List<string>(SetCellContents(name, content));
            }
            //Checks to see if it is a double, if so then it will set the cell to the contents
            else if (double.TryParse(content, out result))
            {
                allDependents = new List<string>(SetCellContents(name, result));
            }
            //Checks to see if 'content' is a formula
            else if (content.Substring(0, 1).Equals("="))
            {
                string formulaString = content.Substring(1, content.Length - 1);
                Formula formula = new Formula(formulaString);
                allDependents = new List<string>(SetCellContents(name, formula));
            }
            //At the end, it should just be a string, so just save it to cell
            else
            {
                allDependents = new List<string>(SetCellContents(name, content));
            }

            Changed = true;

            foreach (string s in allDependents)
            {
                Cell cellValue;
                if(cells.TryGetValue(s, out cellValue))
                {
                    cellValue.ReEvaluate(LookupValue);
                }
            }

            return (IList<string>)allDependents;
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
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            //temporary variable
            IEnumerable<string> oldDependees = dg.GetDependees(name);

            //replace dependents of 'name' with variables in formula
            dg.ReplaceDependees(name, formula.GetVariables());

            try
            {
                List<string> allDependee = new List<string>(GetCellsToRecalculate(name));
                Cell cell = new Cell(formula, LookupValue);
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
        protected override IList<string> SetCellContents(string name, double number)
        {
            //Creates a cell
            Cell cell = new Cell(number);

            //If it contains 'name,' then it will replace the key with the new value
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
        protected override IList<string> SetCellContents(string name, string text)
        {
            //Creates a cell
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
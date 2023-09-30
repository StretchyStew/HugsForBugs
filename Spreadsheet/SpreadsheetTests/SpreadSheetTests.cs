using System;
using SS;
using SpreadsheetUtilities;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        //Tests if cell name is 'null' for GetCellValue
        public void TestNull1()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.GetCellValue(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        //Tests if the cell name is invalid
        public void TestNull2()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            object value = spreadsheet.GetCellValue("&^)&3");
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        //Tests for null content
        public void TestNull3()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("B3", null);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        //Tests for a null name
        public void TestNull4()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell(null, "Ryan");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        //Tests a null filename
        public void TestNull5()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("D1", "1");
            spreadsheet.Save(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        //Test invalid cell name
        public void TestInvalidName()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("123", "Ryan");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        //Test for an invalid formula
        public void TestInvalidFormula()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("= C3", "^_^ + <3");
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestCircularException()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "=A1");
        }

        bool valid = true;

        public bool checkVar(string s)
        {
            if (valid)
            {
                valid = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        [TestMethod]
        //Tests GetCellValue with a string
        public void TestGetCellValue1()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "Ryan");
            object val = spreadsheet.GetCellValue("A1");
            Assert.AreEqual("Ryan", val);
        }

        [TestMethod]
        //Tests GetCellValue with a double
        public void TestGetCellContents1()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "20.1");
            object val = spreadsheet.GetCellContents("A1");
            Assert.AreEqual(20.1, val);
        }

        [TestMethod]
        //Tests GetCellValue with a formula
        public void TestGetCellValue2()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("B1", "1");
            spreadsheet.SetContentsOfCell("C1", "3");
            spreadsheet.SetContentsOfCell("A1", "=B1 + C1");
            object val = spreadsheet.GetCellValue("A1");
            Assert.AreEqual(4.0, val);
        }

        [TestMethod]
        //Test GetCellValue with multiple formulas
        public void TestGetCellValue3()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("D1", "1");
            spreadsheet.SetContentsOfCell("E1", "1");
            spreadsheet.SetContentsOfCell("F1", "3");
            spreadsheet.SetContentsOfCell("B1", "=D1 + E1");
            spreadsheet.SetContentsOfCell("C1", "=F1");
            spreadsheet.SetContentsOfCell("A1", "=B1 + C1");
            object value = spreadsheet.GetCellValue("A1");
            Assert.AreEqual(5.0, value);
        }

        [TestMethod]
        public void TestGetCellValue4()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            Assert.AreEqual(spreadsheet.Changed, false);
            object value = spreadsheet.GetCellValue("A1");
            Assert.AreEqual("", value);
        }

        [TestMethod]
        public void TestGetCellValue5()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("D1", "1");
            sheet.SetContentsOfCell("E1", "1");
            sheet.SetContentsOfCell("D1", "2");
            object D1value = sheet.GetCellValue("D1");
            Assert.AreEqual(2.0, D1value);
            sheet.SetContentsOfCell("F1", "3");
            sheet.SetContentsOfCell("B1", "=D1 + E1");
            sheet.SetContentsOfCell("B1", "=F1 + D1");
            object B1value = sheet.GetCellValue("B1");
            Assert.AreEqual(5.0, B1value);
            sheet.SetContentsOfCell("C1", "=F1");
            sheet.SetContentsOfCell("A1", "=B1 + C1");
            object A1value = sheet.GetCellValue("A1");
            Assert.AreEqual(8.0, A1value);
        }

        [TestMethod]
        public void TestMethod55()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("D1", "4");
            spreadsheet.SetContentsOfCell("E1", "=D1");
            spreadsheet.SetContentsOfCell("F1", "=D1");
            spreadsheet.SetContentsOfCell("C1", "7");
            spreadsheet.SetContentsOfCell("F1", "=C1");
            spreadsheet.SetContentsOfCell("C1", "10");
            spreadsheet.SetContentsOfCell("G1", "=D1");
            HashSet<string> valDependD1 = new HashSet<string>(spreadsheet.SetContentsOfCell("D1", "1"));
            object E1value = spreadsheet.GetCellValue("E1");
            object F1value = spreadsheet.GetCellValue("F1");
            object G1value = spreadsheet.GetCellValue("G1");
            Assert.AreEqual(E1value, 1.0);
            Assert.AreEqual(F1value, 10.0);
            Assert.AreEqual(G1value, 1.0);
            List<string> set = new List<string>();
            set.Add("D1");
            set.Add("E1");
            set.Add("F1");
            set.Add("G1");
            foreach (string s in valDependD1)
            {
                Assert.IsTrue(set.Contains(s));
            }
        }

        // Tests null
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void PS4TestNull1()
        {
            string empty = null;
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A3", empty);
        }

        //Tests null
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void PS4TestNull2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell(null, "string");
        }

        //Tests null
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void PS4TestNull3()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents(null);
        }

        //Tests an invalid name
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TPS4estInvalidName4()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("Ryan", "string");
        }

        //Tests an invalid name
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PS4TestInvalidName1()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A55A", "string");
        }

        //Tests an invalid name
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PS4TestInvalidName2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("A55A");
        }

        //Test for CircularException
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void PS4TestForCircularException()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            //Formula formula = new Formula("B12 - A3");
            sheet.SetContentsOfCell("B12", "=B12-A3");
        }

        //Tests SetCellContents for Formula
        [TestMethod]
        public void PS4TestSetCellContentsFormula()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string formula = ("B12 + A31");
            sheet.SetContentsOfCell("Z2", formula);
            object value = sheet.GetCellContents("Z2");
            Assert.AreEqual(formula, value);
        }

        //Tests SetCellContents for String
        [TestMethod]
        public void PS4TestSetCellContentsString()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "Ryan Reynolds");
            object value = sheet.GetCellContents("A1");
            Assert.AreEqual("Ryan Reynolds", value);
        }

        //Tests SetCellContents for Double
        [TestMethod]
        public void PS4TestSetCellContentsDouble()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A3", "2");
            object value = sheet.GetCellContents("A3");
            Assert.AreEqual(2.0, value);
        }

        //Tests SetCellContents for Formula
        [TestMethod]
        public void PS4TestSetCellContentsFormula2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string formula = "A1 - o2";
            sheet.SetContentsOfCell("B2", formula);
            object value = sheet.GetCellContents("B2");
            Assert.AreEqual(formula, value);
        }

        //Tests For Replacing Contents of Cell
        [TestMethod]
        public void PS4TestReplaceContents1()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string formula = "A1 + o2";
            sheet.SetContentsOfCell("A3", formula);
            sheet.SetContentsOfCell("A3", "Ryan");
            sheet.SetContentsOfCell("A3", "34");
            HashSet<string> t1 = new HashSet<string>(sheet.GetNamesOfAllNonemptyCells());
            HashSet<string> t2 = new HashSet<string>();
            t2.Add("A1");
            Assert.AreEqual(t1.Count, t2.Count);
        }

        //Tests For Replacing Contents of Cell
        [TestMethod]
        public void PS4TestReplaceContents2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string formula = "A1 + o2";
            sheet.SetContentsOfCell("A3", formula);
            sheet.SetContentsOfCell("A1", "Ryan");
            sheet.SetContentsOfCell("A2","34");
            HashSet<string> t1 = new HashSet<string>(sheet.GetNamesOfAllNonemptyCells());
            HashSet<string> t2 = new HashSet<string>();
            t2.Add("A1");
            t2.Add("A2");
            t2.Add("A3");
            Assert.AreEqual(t1.Count, t2.Count);
        }
    }
}


using System;
using SS;
using SpreadsheetUtilities;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        // Tests null
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestNull1()
        {
            string empty = null;
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A3", empty);
        }

        //Tests null
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestNull2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents(null, "string");
        }

        //Tests null
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestNull3()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents(null);
        }

        //Test null
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestNull4()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Formula empty = null;
            sheet.SetCellContents("B7", empty);
        }

        //Test null
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestNull5()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Formula formula = new Formula("5 + 5");
            sheet.SetCellContents(null, formula);
        }

        //Test null
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNull6()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents(null, 76);
        }

        //Tests an invalid name
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestInvalidName4()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("Ryan", "string");
        }

        //Tests an invalid name
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestInvalidName1()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A55A", "string");
        }

        //Tests an invalid name
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestInvalidName2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("A55A");
        }

        //Tests an invalid name
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestInvalidName3()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Formula formula = new Formula("17 + 3");
            sheet.SetCellContents("5ABBA0", formula);
        }

        //Test for CircularException
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestForCircularException()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Formula formula = new Formula("B12 - A3");
            sheet.SetCellContents("B12", formula);
        }

        //Tests SetCellContents for Formula
        [TestMethod]
        public void TestSetCellContentsFormula()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Formula formula = new Formula("B12 + A31");
            sheet.SetCellContents("Z2", formula);
            object value = sheet.GetCellContents("Z2");
            Assert.AreEqual(new Formula("B12 + A31"), value);
        }

        //Tests SetCellContents for String
        [TestMethod]
        public void TestSetCellContentsString()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", "Ryan Reynolds");
            object value = sheet.GetCellContents("A1");
            Assert.AreEqual("Ryan Reynolds", value);
        }

        //Tests SetCellContents for Double
        [TestMethod]
        public void TestSetCellContentsDouble()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A3", 2);
            object value = sheet.GetCellContents("A3");
            Assert.AreEqual(2.0, value);
        }

        //Tests SetCellContents for Formula
        [TestMethod]
        public void TestSetCellContentsFormula2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Formula formula = new Formula("A1 - o2", x => x.ToUpper(), x => true);
            sheet.SetCellContents("B2", formula);
            object value = sheet.GetCellContents("B2");
            Assert.AreEqual(new Formula("A1 - O2"), value);
        }

        //Tests For Replacing Contents of Cell
        [TestMethod]
        public void TestReplaceContents1()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Formula formula = new Formula("A1 + o2", x => x.ToUpper(), x => true);
            sheet.SetCellContents("A3", formula);
            sheet.SetCellContents("A3", "Ryan");
            sheet.SetCellContents("A3", 34);
            HashSet<string> t1 = new HashSet<string>(sheet.GetNamesOfAllNonemptyCells());
            HashSet<string> t2 = new HashSet<string>();
            t2.Add("A1");
            Assert.AreEqual(t1.Count, t2.Count);
        }

        //Tests For Replacing Contents of Cell
        [TestMethod]
        public void TestReplaceContents2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Formula formula = new Formula("A1 + o2", x => x.ToUpper(), x => true);
            sheet.SetCellContents("A3", formula);
            sheet.SetCellContents("A1", "Ryan");
            sheet.SetCellContents("A2", 34);
            HashSet<string> t1 = new HashSet<string>(sheet.GetNamesOfAllNonemptyCells());
            HashSet<string> t2 = new HashSet<string>();
            t2.Add("A1");
            t2.Add("A2");
            t2.Add("A3");
            Assert.AreEqual(t1.Count, t2.Count);
        }
    }
}
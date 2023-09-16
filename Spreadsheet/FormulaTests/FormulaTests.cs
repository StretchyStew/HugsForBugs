using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System;

namespace FormulaTester
{
    [TestClass()]
    public class FormulaTests
    {
        //Testing int with and without whitespace
        [TestMethod()]
        public void FormulaEqualityTest1()
        {
            Formula f1 = new Formula("2+3");
            Formula f2 = new Formula("     2 +    3");
            object result = f1.Evaluate(null);
            Assert.AreEqual(f1, f2);
            Assert.AreEqual(5.0, result);
        }

        //testing int and double equality
        [TestMethod()]
        public void FormulaEqualityTest2()
        {
            Formula f1 = new Formula("2+3");
            Formula f2 = new Formula("2.0 + 3.0");
            object result = f1.Evaluate(null);
            Assert.AreEqual(f1, f2);
            Assert.AreEqual(5.0, result);
        }

        //Testing Multiplication
        [TestMethod()]
        public void FormulaEqualityTest3()
        {
            Formula f1 = new Formula("2*3");
            object result = f1.Evaluate(null);
            Assert.AreEqual(6.0, result);
        }

        //Tests Divide by 0
        [TestMethod()]
        public void FormulaEqualityTest4()
        {
            Formula f1 = new Formula("5 / 0");
            Assert.ThrowsException<DivideByZeroException>(() =>
            {
                double result = (double)f1.Evaluate(variable => 0);
            });
        }

        //Testing with parentheses
        [TestMethod()]
        public void ParenthesesTest()
        {
            Formula f1 = new Formula("(10 + 20) / 3");
            object result = f1.Evaluate(null);
            Assert.AreEqual(10.0, result);
        }

        //Tests with variables
        [TestMethod()]
        public void VariableTest1()
        {
            Formula f1 = new Formula("b11 + B11");
            object result = f1.Evaluate(x => 10);
            Assert.AreEqual(20.0, result);
        }

        //Tests with Invalid Variable
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidVariable()
        {
            Formula f1 = new Formula("1HELLO + 7", x => x.ToUpper(), x => true);
        }

        //Testing ToString and Normalizer
        [TestMethod()]
        public void ToStringAndNormalizerTest()
        {
            Formula f1 = new Formula("A7 + b7", x => x.ToUpper(), x => true);
            string formula = f1.ToString();
            Assert.AreEqual("A7+B7", formula);
        }

        //Testing the Equal
        [TestMethod()]
        public void TestEqual()
        {
            Formula f1 = new Formula("(A2 + b7) * 7 - 8", x => x.ToUpper(), x => true);
            Formula f2 = new Formula("(a2 + B7) * 7.0 - 8.0", x => x.ToUpper(), x => true);
            Assert.IsTrue(f1.Equals(f2));
        }

        //Testing Equal On Two Non-Equal Formulas
        [TestMethod()]
        public void TestEqualFalse()
        {
            Formula f1 = new Formula("O1 + b1 - 2 * 1.0", x => x.ToUpper(), x => true);
            Formula f2 = new Formula("o1 + b1 * 1.00", x => x.ToUpper(), x => true);
            Assert.IsFalse(f1.Equals(f2));
        }

        //Testing Equal
        [TestMethod()]
        public void TestEqualWithAnswer()
        {
            Formula f1 = new Formula("2 + 2");
            object result = f1.Evaluate(x => 1);
            Assert.AreEqual(4.0, result);
        }

        //Test !=
        [TestMethod()]
        public void TestNotEqual()
        {
            Formula f1 = new Formula("A1 + B2 + 5.0", x => x.ToUpper(), x => true);
            Formula f2 = new Formula("A1 + B2 + 5.0", x => x.ToUpper(), x => true);
            bool result = f1 != f2;
            Assert.IsFalse(result);
        }

        //Testing Equal with String
        [TestMethod()]
        public void TestEqualWithString()
        {
            Formula f1 = new Formula("2 + 3.0 + b2", x => x.ToUpper(), x => true);
            string test = "Ryan Reynolds Is the Best";
            bool res = f1.Equals(test);
            Assert.IsFalse(res);
        }

        //Test GetHashCode
        [TestMethod()]
        public void TestGetHashCode()
        {
            Formula f1 = new Formula("B7 + a3");
            string hash = "B7+a3";
            int code = hash.GetHashCode();
            int formulaCode = f1.GetHashCode();
            Assert.AreEqual(code, formulaCode);
        }

        //Test GetHashCode with Normalize
        [TestMethod()]
        public void TestGetHashCodeWithNormalize()
        {
            Formula f1 = new Formula("A1 + b1", x => x.ToUpper(), x => true);
            string hash = "A1+B1";
            int code = hash.GetHashCode();
            int formulaCode = f1.GetHashCode();
            Assert.AreEqual(code, formulaCode);
        }

        //Test for Valid Variable
        [TestMethod()]
        public void TestValidVariableFormat()
        {
            Formula f1 = new Formula("A1 + _____6 + HI + H3H", x => x.ToUpper(), x => true);
            object result = f1.Evaluate(x => 1);
            Assert.AreEqual(4.0, result);
        }

        //Test for Unequal Parentheses
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestUnequalParentheses()
        {
            Formula f1 = new Formula("(a1 + 8", x => x.ToUpper(), x => true);
        }

        //Test for Unequal Parentheses
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestUnequalParentheses2()
        {
            Formula f1 = new Formula("B7 + 19 (7 / 6))", x => x.ToUpper(), x => true);
        }

        //Test Double Operator
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestDoubleOperator()
        {
            Formula f1 = new Formula("x ++ 1", x => x.ToUpper(), x => true);
        }

        //Test Long Maths
        [TestMethod()]
        public void TestLongFormula()
        {
            Formula f1 = new Formula("(1 + ab1) * A1 / C3 - 1 / ((1 + 1) - 1) * (d3 + (_d - 1))", x => x.ToUpper(), x => true);
            object result = f1.Evaluate(x => 1);
            Assert.AreEqual(1.0, result);
        }

        //Test GetVariable
        [TestMethod()]
        public void TestGetVariable()
        {
            Formula f1 = new Formula("x+Y*z", s => s.ToUpper(), s => true);
            Formula f2 = new Formula("x+z", s => s.ToUpper(), s => true);
            List<String> a = new List<String>(f1.GetVariables());
            List<String> b = new List<String>(f2.GetVariables());
            Assert.AreEqual(a.ToString(), b.ToString());
        }

        //Test False IsValid
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestIsValid()
        {
            Formula f1 = new Formula("1 + 2.0 + A3", x => x.ToUpper(), x => false);
        }

        //Test Unacceptable Normalize
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestBadNormalize()
        {
            Formula f1 = new Formula("1 + 3.0 * A7", x => "3", x => false);
        }
    }
}
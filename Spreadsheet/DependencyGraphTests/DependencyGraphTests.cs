namespace DependencyGraphTests;

using System;
using SpreadsheetUtilities;

[TestClass]
public class DependencyGraphTests
{
    [TestMethod]
    public void TestAddDependency()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("A1", "A3");
        Assert.AreEqual("A1", dg.GetDependents("A1"));
    }
}
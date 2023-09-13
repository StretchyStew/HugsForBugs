using System.Text.RegularExpressions;
using System;

namespace FormulaEvaluator;

public static class Evaluator
{
    public delegate int Lookup(String v);

    public static int Evaluate(String exp, Lookup variableEvaluator)
    {
        string[] substrings = Regex.Split(exp, "(\\() | (\\))| (-) | (\\+)| (\\*)| (/)");

        //removes whitespace
        for (int i = 0; i < substrings.Length; i++)
        {
            substrings[i] = substrings[i].Trim();
        }

        
        return 0;
    }

}

namespace FormulaEvaluator;

public delegate int Lookup(String v);

public static int Evaluate(String exp, Lookup variableEvaluator)
{
    string[] substrings = Regex.Split(s, "(\\() | (\\))| (-) | (\\+)| (\\*)| (/)");
}


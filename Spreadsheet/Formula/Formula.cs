// Skeleton written by Profs Zachary, Kopta and Martin for CS 3500
// Read the entire skeleton carefully and completely before you
// do anything else!
// Last updated: August 2023 (small tweak to API)

using System;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities;

/// <summary>
/// Represents formulas written in standard infix notation using standard precedence
/// rules.  The allowed symbols are non-negative numbers written using double-precision
/// floating-point syntax (without unary preceeding '-' or '+');
/// variables that consist of a letter or underscore followed by
/// zero or more letters, underscores, or digits; parentheses; and the four operator
/// symbols +, -, *, and /.
///
/// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
/// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable;
/// and "x 23" consists of a variable "x" and a number "23".
///
/// Associated with every formula are two delegates: a normalizer and a validator.  The
/// normalizer is used to convert variables into a canonical form. The validator is used to
/// add extra restrictions on the validity of a variable, beyond the base condition that
/// variables must always be legal: they must consist of a letter or underscore followed
/// by zero or more letters, underscores, or digits.
/// Their use is described in detail in the constructor and method comments.
/// </summary>
public class Formula
{
    /// <summary>
    /// Creates a Formula from a string that consists of an infix expression written as
    /// described in the class comment.  If the expression is syntactically invalid,
    /// throws a FormulaFormatException with an explanatory Message.
    ///
    /// The associated normalizer is the identity function, and the associated validator
    /// maps every string to true.
    /// </summary>
    public Formula(string formula) :
        this(formula, s => s, s => true)
    {
    }

    public List<string> token;

    public HashSet<string> normalizedVariables;

    /// <summary>
    /// Creates a Formula from a string that consists of an infix expression written as
    /// described in the class comment.  If the expression is syntactically incorrect,
    /// throws a FormulaFormatException with an explanatory Message.
    ///
    /// The associated normalizer and validator are the second and third parameters,
    /// respectively.
    ///
    /// If the formula contains a variable v such that normalize(v) is not a legal variable,
    /// throws a FormulaFormatException with an explanatory message.
    ///
    /// If the formula contains a variable v such that isValid(normalize(v)) is false,
    /// throws a FormulaFormatException with an explanatory message.
    ///
    /// Suppose that N is a method that converts all the letters in a string to upper case, and
    /// that V is a method that returns true only if a string consists of one letter followed
    /// by one digit.  Then:
    ///
    /// new Formula("x2+y3", N, V) should succeed
    /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
    /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
    /// </summary>
    public Formula(string formula, Func<string, string> normalize, Func<string, bool> isValid)
    {
        //checks for an empty formula
        if (formula == string.Empty || ReferenceEquals(formula, null))
        {
            throw new FormulaFormatException("Your Formula is empty, try adding a formula or characters into the cell.");
        }

        token = new List<string>(GetTokens(formula));

        normalizedVariables = new HashSet<string>();

        double number;

        int openParenthesesCount = 0;
        int closeParenthesesCount = 0;

        //first checking if the first and last token are valid (parentheses, number, and variable)
        string firstToken = token.First<string>();
        string lastToken = token.Last<string>();

        if (!firstToken.Equals("("))
        {
            if (!double.TryParse(firstToken, out number))
            {
                if (!ValidVariable(firstToken))
                {
                    throw new FormulaFormatException("The first character does not contain a parentheses, number, or variable.");
                }
            }
        }

        if (!lastToken.Equals(")"))
        {
            if (!double.TryParse(lastToken, out number))
            {
                if (!ValidVariable(lastToken))
                {
                    throw new FormulaFormatException("The last character does not contain a parentheses, number, or variable.");
                }
            }
        }

        string previousToken = "";

        List<string> operations = new List<string> {"+", "-", "*", "/"};

        //Check if the token is a valid character, double, or operation.
        for (int i = 0; i < token.Count(); i++)
        {
            string t = token[i];
            if (t.Equals("("))
            {
                openParenthesesCount++;
            }
            else if (t.Equals(")"))
            {
                closeParenthesesCount++;
                if (openParenthesesCount < closeParenthesesCount)
                {
                    throw new FormulaFormatException("You cannot have more closing parenthese than open ones.");
                }
            }
            else if (operations.Contains(t)) { }
            else if (double.TryParse(t, out number))
            {
                token[i] = number.ToString();
            }
            else if (ValidVariable(t)) { }
            else
            {
                throw new FormulaFormatException("One of your constants is invalid.");
            }

            //Makes sure the formula is in proper writing, meaning "(A1 + 1)/2" and not something like ( + 1)/
            if (operations.Contains(t) || t.Equals("("))
            {
                if (!(double.TryParse(t, out number) || ValidVariable(t) || t.Equals("(")))
                {
                    throw new FormulaFormatException("An operation or '(' needs to be followed by a number, variable, or '('.");
                }
            }
            else if (previousToken.Equals(")") || double.TryParse(previousToken, out number) || ValidVariable(previousToken))
            {
                if (!operations.Contains(t))
                {
                    throw new FormulaFormatException("Numbers, variables, and ')' need to be followed by an operation.");
                }
            }

            previousToken = t;
        }

        if(openParenthesesCount != closeParenthesesCount)
        {
            throw new FormulaFormatException("Your formula does not contain an equal number of '(' to ')'.");
        }

        //Note: should be a valid formula

        for (int i = 0; i != token.Count(); i++)
        {
            string v = token[i];
            if (ValidVariable(v))
            {
                if (!ValidVariable(normalize(v)))
                {
                    throw new FormulaFormatException("The normalized variable is not a valid variable.");
                }
                if (!isValid(normalize(v)))
                {
                    throw new FormulaFormatException("The normalized variable is not valid.");
                }
                else
                {
                    token[i] = normalize(v);
                    normalizedVariables.Add(token[i]);
                }
            }
        }

    }


    /// <summary>
    /// Checks to see if the token is a valid variable
    /// </summary>
    private static bool ValidVariable(string token)
    {
        if(Regex.IsMatch(token, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*", RegexOptions.Singleline))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// Evaluates this Formula, using the lookup delegate to determine the values of
    /// variables.  When a variable symbol v needs to be determined, it should be looked up
    /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to
    /// the constructor.)
    ///
    /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters
    /// in a string to upper case:
    ///
    /// new Formula("x+7", N, s => true).Evaluate(L) is 11
    /// new Formula("x+7").Evaluate(L) is 9
    ///
    /// Given a variable symbol as its parameter, lookup returns the variable's value
    /// (if it has one) or throws an ArgumentException (otherwise).
    ///
    /// If no undefined variables or divisions by zero are encountered when evaluating
    /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.
    /// The Reason property of the FormulaError should have a meaningful explanation.
    ///
    /// This method should never throw an exception.
    /// </summary>
    public object Evaluate(Func<string, double> lookup)
    {
        string[] substrings = token.Cast<string>().ToArray<string>();

        //removes whitespace
        for (int i = 0; i < substrings.Length; i++)
        {
            string trim = substrings[i].Trim();
            substrings[i] = trim;
        }

        //creates 2 stacks to hold the values and operators
        Stack<int> value = new Stack<int>();
        Stack<string> operators = new Stack<string>();

        for (int i = 0; i < substrings.Length; i++)
        {
            string token = substrings[i];
            double number;

            if (token.Equals(""))
            {
                continue;
            }

            if (token.Equals("*") || token.Equals("/") || token.Equals("("))
            {
                operators.Push(token);
            }

            else if (token.Equals("+") || token.Equals("-") || token.Equals(")"))
            {
                string top = "";

                if (operators.Count > 0)
                {
                    top = operators.Peek();
                }

                if (top.Equals("+") || top.Equals("-"))
                {

                    double valOne = value.Pop();
                    double valTwo = value.Pop();
                    string operation = operators.Pop();

                    int answer = 0;

                    if (operation.Equals("+"))
                    {
                        answer = valTwo + valOne;
                    }
                    else
                    {
                        answer = valTwo - valOne;
                    }
                    value.Push(answer);
                }

                if (token.Equals(")"))
                {
                    string newTop = "";

                    if (operators.Count > 0)
                    {
                        newTop = operators.Peek();
                    }

                    //removes the '(' from the top
                    operators.Pop();

                    if (operators.Count > 0)
                    {
                        newTop = operators.Peek();
                    }

                    if (newTop.Equals("*") || newTop.Equals("/"))
                    {

                        int valOne = value.Pop();
                        int valTwo = value.Pop();
                        string operation = operators.Pop();

                        int answer;

                        if (operation.Equals("*"))
                        {
                            answer = valOne * valTwo;
                        }
                        else
                        {
                            //divide by zero error
                            if (valOne == 0)
                            {
                                throw new ArgumentException();
                            }
                            answer = valTwo / valOne;
                        }
                        value.Push(answer);
                    }
                }
                else
                {
                    operators.Push(token);
                }
            }

            //checks to see if our token is a number, if so then do multiplication/division if applicable
            else if (Double.TryParse(token, out number))
            {
                int num = (int)number;

                string top = "";

                if (operators.Count > 0)
                {
                    top = operators.Peek();
                }

                if (top.Equals("*") || top.Equals("/"))
                {
                    //Because we haven't popped the current token, we don't need a new variable here
                    int valOne = value.Pop();
                    string operation = operators.Pop();
                    int answer;

                    if (operation.Equals("*"))
                    {
                        answer = valOne * num;
                    }
                    else
                    {
                        //divide by zero error
                        if (num == 0)
                        {
                            throw new ArgumentException();
                        }
                        answer = valOne / num;
                    }
                    value.Push(answer);
                }
                else
                {
                    value.Push(num);
                }
            }

            //if we get this far, then it should be a variable with format char, int (A1)
            else
            {
                for (int a = 0; a != token.Length; a++)
                {
                    char cur = token[a];

                    //if the first character is not a letter, then throw an exception i.e. 12
                    if (a == 0 && !(char.IsLetter(cur)))
                    {
                        throw new ArgumentException();
                    }

                    //if it encounters a digit, then it will make sure the rest of them are digits i.e. A1%
                    if (char.IsDigit(cur))
                    {
                        for (int b = a; b < token.Length; b++)
                        {
                            char curr = token[b];
                            if (!char.IsDigit(curr))
                            {
                                throw new ArgumentException();
                            }
                        }
                    }

                    //makes sure there are digits on the end i.e. ABC
                    if ((a == token.Length - 1) && !char.IsDigit(cur))
                    {
                        throw new ArgumentException();
                    }
                }

                int t = variableEvaluator(token);

                String top = "";
                if (operators.Count > 0)
                {
                    top = operators.Peek();
                }

                if (top.Equals("*") || top.Equals("/"))
                {
                    int valOne = value.Pop();
                    string operation = operators.Pop();

                    int answer;

                    if (operation.Equals("*"))
                    {
                        answer = valOne * t;
                    }
                    else
                    {
                        if (t == 0)
                        {
                            throw new ArgumentException();
                        }
                        answer = valOne / t;
                    }
                    value.Push(answer);
                }
                else
                {
                    value.Push(t);
                }
            }
        }

        if (operators.Count == 0)
        {
            if (value.Count != 1)
            {
                throw new ArgumentException();
            }
            return value.Pop();
        }

        else
        {
            if ((operators.Count != 1) || (value.Count != 2))
            {
                throw new ArgumentException();
            }

            int valOne = value.Pop();
            int valTwo = value.Pop();
            string operation = operators.Pop();

            int answer;

            if (operation.Equals("+"))
            {
                answer = valTwo + valOne;
            }
            else
            {
                answer = valTwo - valOne;
            }
            return answer;
        }
    }

    /// <summary>
    /// Enumerates the normalized versions of all of the variables that occur in this
    /// formula.  No normalization may appear more than once in the enumeration, even
    /// if it appears more than once in this Formula.
    ///
    /// For example, if N is a method that converts all the letters in a string to upper case:
    ///
    /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
    /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
    /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
    /// </summary>
    public IEnumerable<string> GetVariables()
    {
        HashSet<string> temp = new HashSet<string>(normalizedVariables);
        return temp;
    }

    /// <summary>
    /// Returns a string containing no spaces which, if passed to the Formula
    /// constructor, will produce a Formula f such that this.Equals(f).  All of the
    /// variables in the string should be normalized.
    ///
    /// For example, if N is a method that converts all the letters in a string to upper case:
    ///
    /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
    /// new Formula("x + Y").ToString() should return "x+Y"
    /// </summary>
    public override string ToString()
    {
        string formula = "";
        for (int i = 0; i < token.Count; i++)
        {
            formula += token[i];
        }
        return formula;
    }

    /// <summary>
    /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
    /// whether or not this Formula and obj are equal.
    ///
    /// Two Formulae are considered equal if they consist of the same tokens in the
    /// same order.  To determine token equality, all tokens are compared as strings
    /// except for numeric tokens and variable tokens.
    /// Numeric tokens are considered equal if they are equal after being "normalized" by
    /// using C#'s standard conversion from string to double (and optionally back to a string).
    /// Variable tokens are considered equal if their normalized forms are equal, as
    /// defined by the provided normalizer.
    ///
    /// For example, if N is a method that converts all the letters in a string to upper case:
    ///
    /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
    /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
    /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
    /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
    /// </summary>
    public override bool Equals(object? obj)
    {
        //if null or not a formula, then we have to return false because we can not check if they are equal
        if (ReferenceEquals(obj, null) || obj.GetType() != this.GetType())
        {
            return false;
        }

        Formula formula = (Formula) obj;

        //checks each token, return false if not equal
        for (int i = 0; i < this.token.Count(); i++)
        {
            string current = this.token[i];
            string actualCurrent = formula.token[i];

            double currentNumber;
            double actualCurrentNumber;

            if (double.TryParse(current, out currentNumber) && double.TryParse(actualCurrent, out actualCurrentNumber))
            {
                if (current != actualCurrent)
                {
                    return false;
                }
                else
                {
                    if (!current.Equals(actualCurrent))
                    {
                        return false;
                    }
                }
            }
        }
        //if it comes this far, then it should be equal.
        return true;
    }

    /// <summary>
    /// Reports whether f1 == f2, using the notion of equality from the Equals method.
    /// Note that f1 and f2 cannot be null, because their types are non-nullable
    /// </summary>
    public static bool operator ==(Formula f1, Formula f2)
    {
        return f1.Equals(f2);
    }

    /// <summary>
    /// Reports whether f1 != f2, using the notion of equality from the Equals method.
    /// Note that f1 and f2 cannot be null, because their types are non-nullable
    /// </summary>
    public static bool operator !=(Formula f1, Formula f2)
    {
        if (!(f1 == f2))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
    /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two
    /// randomly-generated unequal Formulae have the same hash code should be extremely small.
    /// </summary>
    public override int GetHashCode()
    {
        int hash_code = this.ToString().GetHashCode();
        return hash_code;
    }

    /// <summary>
    /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
    /// right paren; one of the four operator symbols; a legal variable token;
    /// a double literal; and anything that doesn't match one of those patterns.
    /// There are no empty tokens, and no token contains white space.
    /// </summary>
    private static IEnumerable<string> GetTokens(string formula)
    {
        // Patterns for individual tokens
        string lpPattern = @"\(";
        string rpPattern = @"\)";
        string opPattern = @"[\+\-*/]";
        string varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
        string doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
        string spacePattern = @"\s+";

        // Overall pattern
        string pattern = string.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                        lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

        // Enumerate matching tokens that don't consist solely of white space.
        foreach (string s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
        {
            if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
            {
                yield return s;
            }
        }

    }
}

/// <summary>
/// Used to report syntactic errors in the argument to the Formula constructor.
/// </summary>
public class FormulaFormatException : Exception
{
    /// <summary>
    /// Constructs a FormulaFormatException containing the explanatory message.
    /// </summary>
    public FormulaFormatException(string message) : base(message)
    {
    }
}

/// <summary>
/// Used as a possible return value of the Formula.Evaluate method.
/// </summary>
public struct FormulaError
{
    /// <summary>
    /// Constructs a FormulaError containing the explanatory reason.
    /// </summary>
    /// <param name="reason"></param>
    public FormulaError(string reason) : this()
    {
        Reason = reason;
    }

    /// <summary>
    ///  The reason why this FormulaError was created.
    /// </summary>
    public string Reason { get; private set; }
}


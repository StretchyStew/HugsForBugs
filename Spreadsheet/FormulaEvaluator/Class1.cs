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

        //creates 2 stacks to hold the values and operators
        Stack<int> value = new Stack<int>();
        Stack<string> operators = new Stack<string>();

        for (int i = 0; i < substrings.Length; i++)
        {
            string token = substrings[i];
            double number;

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
                    int valOne = value.Pop();
                    int valTwo = value.Pop();
                    string operation = operators.Pop();

                    if (operation == "+")
                    {
                        value.Push(valOne + valTwo);
                    }
                    else
                    {
                        value.Push(valOne - valTwo);
                    }
                }

                if (top.Equals(")"))
                {
                    string newTop = "";

                    if (operators.Count > 0)
                    {
                        newTop = operators.Peek();
                    }

                    //makes sure it is complete i.e. (2+2) and not 2+2)
                    if (newTop != "(")
                    {
                        throw new ArgumentException();
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

                        if (operation.Equals("*"))
                        {
                            value.Push(valOne * valTwo);
                        }
                        else
                        {
                            //divide by zero error
                            if (valOne == 0)
                            {
                                throw new ArgumentException();
                            }
                            value.Push(valTwo / valOne);
                        }
                    }
                }
                else
                {
                    operators.Push(token);
                }
            }

            //checks to see if our token is a number, if so then do multiplication/division if applicable
            else if(Double.TryParse(token, out number)){
                int num = (int) number;
                string top = "";
                if (operators.Count > 0)
                {
                    top = operators.Peek();
                }

                if (top.Equals("*") || top.Equals("/"))
                {
                    //Because we haven't pushed the current token, we don't need a new variable here
                    int valOne = value.Pop();
                    string operation = operators.Pop();

                    if (operation.Equals("*"))
                    {
                        value.Push(valOne * num);
                    }
                    else
                    {
                        //divide by zero error
                        if (valOne == 0)
                        {
                            throw new ArgumentException();
                        }
                        value.Push(num / valOne);
                    }
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

                    if(a == 0 && char.IsLetter(cur))
                    {

                    }
                }
            }
        }

    }

}

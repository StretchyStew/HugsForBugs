using System.Text.RegularExpressions;
using System;

namespace FormulaEvaluator
{

    public static class Evaluator
    {

        public delegate int Lookup(String v);


        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            string[] substrings = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

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

                        if (value.Count < 2)
                        {
                            throw new ArgumentException();
                        }

                        int valOne = value.Pop();
                        int valTwo = value.Pop();
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

                            if (value.Count < 2)
                            {
                                throw new ArgumentException();
                            }

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
                    int num = (int) number;

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
                            if (valOne == 0)
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

    }

}
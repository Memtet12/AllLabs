using System.Diagnostics.CodeAnalysis;

namespace RPNcal
{
    public class RPN
    {
        public static List<Token> TokenListMake(string input, string x)
        {
            List<Token> tokens = new List<Token>();
            string? numberString = null;
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsDigit(input[i]))
                {
                    numberString += input[i];
                }
                if (char.IsLetter(input[i]))
                {
                    if (input[i]=='x'|| input[i] == 'X')
                    {
                        numberString += double.Parse(x);
                    }
                    
                }
                else if (input[i] != ' ')
                {
                    if (Number.TryParse(numberString, out Number outputNumber))
                    {
                        tokens.Add(outputNumber);
                    }
                    
                    if (Operation.TryParse(input[i].ToString(), out Operation outputOperation))
                    {
                        tokens.Add(outputOperation);
                    }
                    else if (Parenthesis.TryParse(input[i].ToString(), out Parenthesis outputBracket))
                    {
                        tokens.Add(outputBracket);
                    }

                    numberString = null;
                }
            }
            if (!string.IsNullOrEmpty(numberString))
            {
                if (Number.TryParse(numberString, out Number outputNumber))
                {
                    tokens.Add(outputNumber);
                }
            }
            return tokens;
        }

        public static List<Token> RewriteToRPN(List<Token> tokens)
        {
            List<Token> output = new List<Token>();
            Stack<Token> stack = new Stack<Token>();
            foreach (Token token in tokens)
            {
                if (stack.Count == 0 && !(token is Number))
                {
                    stack.Push(token);
                    continue;
                }
                if (token is Operation)
                {
                    if (stack.Peek() is Parenthesis)
                    {
                        stack.Push(token);
                        continue;
                    }
                    if (((Operation)token).priority > ((Operation)stack.Peek()).priority)
                    {
                        stack.Push(token);
                    }
                    else if (((Operation)token).priority < ((Operation)stack.Peek()).priority)
                    {
                        while (stack.Count > 0 && !(token is Parenthesis))
                        {
                            output.Add(stack.Pop());
                        }
                        stack.Push(token);
                    }
                    else if (((Operation)token).priority == ((Operation)stack.Peek()).priority)
                    {
                        output.Add(stack.Pop());
                        stack.Push(token);
                    }
                }
                else if (token is Parenthesis)
                {
                    if (!((Parenthesis)token).isOpen)
                    {
                        while (!(stack.Peek() is Parenthesis))
                        {
                            output.Add(stack.Pop());
                        }
                        stack.Pop();
                    }
                    else
                    {
                        stack.Push(token);
                    }
                }
                else if (token is Number)
                {
                    output.Add(token);
                }
            }
            while (stack.Count > 0)
            {
                output.Add(stack.Pop());
            }
            return output;
        }
        public static Number CalculateRPN(List<Token> inputTokens)
        {
            Stack<Number> stack = new Stack<Number>();
            foreach (Token token in inputTokens)
            {
                if (token is Number)
                {
                    stack.Push((Number)token);
                }
                else if (token is Operation)
                {
                    Number b = stack.Pop();
                    Number a = stack.Pop();
                    Number result = Perform((Operation)token, a, b);
                    stack.Push(result);
                }
            }
            return stack.Pop();
        }
        public static Number Perform(Operation inputOperation, Number inputA, Number inputB)
        {
            return inputOperation.operation switch
            {
                '+' => inputA + inputB,
                '-' => inputA - inputB,
                '*' => inputA * inputB,
                '/' => inputA / inputB,
                _ => throw new ArgumentException("Invalid operation")
            };
        }
    }
    
    public class Token
    {
        //Number, operation and parenthesis
    }
    public class Variable
    {
        public double value { get; set; }
        public char variable { get; set; }
        public Variable(double _value, char _variable)
            {
            value = _value;
            variable = _variable;
            }

    }

    public class Number : Token
    {
        //само число
        public double value { get; set; }
        public Number(double _value)
        {
            value = _value;
        }

        //операции для чисел
        public static Number operator +(Number x, Number y)
        {
            return new Number(x.value + y.value);
        }
        public static Number operator -(Number x, Number y)
        {
            return new Number(x.value - y.value);
        }
        public static Number operator *(Number x, Number y)
        {
            return new Number(x.value * y.value);
        }
        public static Number operator /(Number x, Number y)
        {
            if (y.value != 0)
            {
                return new Number(x.value / y.value);
            }
            else
            {
                throw new ArgumentException("Делить на ноль нельзя!");
            }
        }

        public static implicit operator double(Number _value)
        {
            return _value.value;
        }

        public static Number Parse(string input)
        {
            return new Number(double.Parse(input));
        }
        public static bool TryParse([NotNullWhen(true)] string? input, out Number output)
        {
            if (input == null)
            {
                output = new Number(0);
                return false;
            }
            for (int i = 0; i < input.Length; i++)
            {
                if (!char.IsDigit(input[i]))
                {
                    output = new Number(0);
                    return false;
                }
            }
            output = new Number(double.Parse(input));
            return true;
        }
    }

    public class Operation : Token
    {
        public char operation { get; set; }
        public int priority;
        public Operation(char _operation)
        {
            operation = _operation;
            if (IsOperator(_operation))
            {
                priority = GetPriority(_operation);
            }
        }

        private static int GetPriority(char operation)
        {
            if ("+-*/".Contains(operation))
            {
                return operation switch
                {
                    '*' => 2,
                    '/' => 2,
                    '-' => 1,
                    '+' => 1,
                    _ => throw new ArgumentException("Не правильная операция!")
                };
            }
            else
            {
                throw new ArgumentException("Не правильная операция!");
            }
        }
        public static Operation Parse(string input)
        {
            if (TryParse(input, out Operation output))
            {
                return output;
            }
            else
            {
                throw new ArgumentException("Обработка не удалась!");
            }
        }
        public static bool TryParse([NotNullWhen(true)] string? input, out Operation output)
        {
            output = new Operation('\0');
            if (input == null)
            {
                return false;
            }
            if (input.Length != 1)
            {
                return false;
            }
            if (!"+-*/".Contains(input[0]))
            {
                return false;
            }
            output = new Operation(input[0]);
            return true;
        }
        public static bool IsOperator(char symbol)
        {
            if ("+-*/".Contains(symbol))
            {
                return true;
            }
            return false;
        }
    }

    public class Parenthesis : Token
    {
        public char bracket { get; set; }
        public bool isOpen;
        public Parenthesis(char _bracket)
        {
            bracket = _bracket;
            if (IsBracket(_bracket))
            {
                isOpen = Transparency(_bracket);
            }
        }
        private bool Transparency(char _bracket)
        {
            return isOpen = _bracket switch
            {
                '(' => true,
                ')' => false,
                _ => throw new ArgumentException("Не скобочка!")
            };
        }
        public static Parenthesis Parse(string input)
        {
            if (char.TryParse(input, out char output))
            {
                if ("()".Contains(output))
                {
                    return new Parenthesis(output);
                }
                else
                {
                    throw new ArgumentException("Обработка не удалась!");
                }
            }
            else
            {
                throw new ArgumentException("Обработка не удалась!");
            }
        }
        public static bool TryParse([NotNullWhen(true)] string? input, out Parenthesis output)
        {
            output = new Parenthesis('\0');
            if (input == null)
            {
                return false;
            }
            if (input.Length != 1)
            {
                return false;
            }
            if (!"()".Contains(input[0]))
            {
                return false;
            }
            output = new Parenthesis(input[0]);
            return true;
        }
        public static bool IsBracket(char _bracket)
        {
            if ("()".Contains(_bracket))
            {
                return true;
            }
            return false;
        }
    }
}

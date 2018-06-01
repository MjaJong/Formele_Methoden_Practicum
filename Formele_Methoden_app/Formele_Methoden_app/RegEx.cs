using System;
using System.Collections.Generic;

namespace Formele_Methoden_app
{
    public enum Operator
    {
        PLUS,
        STAR,
        OR,
        DOT,
        ONE
    };

    public class RegExp
    {
        private string terminals;
        private static CompareByLength compareByLength = new CompareByLength();
        private Operator op;
        private RegExp left;
        private RegExp right;

        public RegExp LeftRegExp { get { return left; } }
        public RegExp RightRegExp { get { return right; } }
        public Operator Op { get { return op; } }

        public RegExp()
        {
            op = Operator.ONE;
            terminals = string.Empty;
            left = null;
            right = null;
        }

        public string GetLanguageString(int maxSteps)
        {
            SortedSet<String> set = GetLanguageSet(maxSteps);
            string result = "";

            foreach (string symbol in set)
            {
                result += symbol;
                result += "; \n";
            }


            return result;
        }

        public SortedSet<string> GetLanguageSet(int maxSteps)
        {
            SortedSet<String> emptyLanguage = new SortedSet<String>(compareByLength);
            SortedSet<String> languageResult = new SortedSet<String>(compareByLength);
            SortedSet<String> languageLeft, languageRight;

            if (maxSteps < 1) return emptyLanguage;

            switch (op)
            {
                case Operator.ONE:
                    languageResult.Add(terminals);
                    break;

                case Operator.OR:
                    languageLeft = left == null ? emptyLanguage : left.GetLanguageSet(maxSteps - 1);
                    languageRight = right == null ? emptyLanguage : right.GetLanguageSet(maxSteps - 1);
                    AddSetToSet(languageResult, languageLeft);
                    AddSetToSet(languageResult, languageRight);
                    break;

                case Operator.DOT:
                    languageLeft = left == null ? emptyLanguage : left.GetLanguageSet(maxSteps - 1);
                    languageRight = right == null ? emptyLanguage : right.GetLanguageSet(maxSteps - 1);
                    foreach (String s1 in languageLeft)
                        foreach (String s2 in languageRight)
                            languageResult.Add(s1 + s2);
                    break;

                case Operator.STAR:
                case Operator.PLUS:
                    languageLeft = left == null ? emptyLanguage : left.GetLanguageSet(maxSteps - 1);
                    AddSetToSet(languageResult, languageLeft);
                    for (int i = 1; i < maxSteps; i++)
                    {
                        HashSet<String> languageTemp = new HashSet<String>(languageResult);
                        foreach (String s1 in languageLeft)
                        {
                            foreach (String s2 in languageTemp)
                            {
                                languageResult.Add(s1 + s2);
                            }
                        }
                    }
                    if (op  == Operator.STAR)
                        languageResult.Add("");
                    break;


                default:
                    Console.WriteLine("getLanguage is nog niet gedefinieerd voor de operator: " + op);
                    break;
            }
            return languageResult;
        }

        private void AddSetToSet(SortedSet<String> keep, SortedSet<String> merge)
        {
            foreach (string symbol in merge)
                keep.Add(symbol);
        }

        private class CompareByLength : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                if (x.Length == y.Length)
                 return x.CompareTo(y);

                return x.Length - y.Length;
            }
        }

        public RegExp(String p)
        {
            op = Operator.ONE;
            terminals = p;
            left = null;
            right = null;
        }

        public RegExp Plus()
        {
            RegExp result = new RegExp();
            result.op = Operator.PLUS;
            result.left = this;
            return result;
        }

        public RegExp Star()
        {
            RegExp result = new RegExp();
            result.op = Operator.STAR;
            result.left = this;
            return result;
        }

        public RegExp Or(RegExp e2)
        {
            RegExp result = new RegExp();
            result.op = Operator.OR;
            result.left = this;
            result.right = e2;
            return result;
        }

        public RegExp Dot(RegExp e2)
        {
            RegExp result = new RegExp();
            result.op = Operator.DOT;
            result.left = this;
            result.right = e2;
            return result;
        }

        /// <summary>
        /// to string method that recursively walks through all left and right parts, adding them together and returning (hopefully) the complete language as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string leftPart = TraverseLeft(left);
            string rightPart = TraverseRight(right);

            char operatorAsChar = EnumToChar(op);

            //So i guess its left + terminals + operator + right
            string completeString = terminals;

            if (operatorAsChar != '$') { completeString = completeString + operatorAsChar; }
            if (leftPart != string.Empty) { completeString = leftPart + completeString; }
            if (rightPart != string.Empty) { completeString = completeString + rightPart; }

            return completeString;
        }

        /// <summary>
        /// Traverses all left fields of the <see cref="RegExp"/>.
        /// </summary>
        /// <param name="left">The initial left part of the <see cref="RegExp"/>.</param>
        /// <returns>Returns the final string of everyting left of the original.</returns>
        private string TraverseLeft(RegExp left)
        {
            string leftString = terminals;
            if(left.op != Operator.ONE) { leftString = leftString + EnumToChar(left.op); }
            if(left.LeftRegExp != null)
            {
                string evenMoreLeft = TraverseLeft(left.LeftRegExp);
                leftString = evenMoreLeft + leftString;
            }
            return leftString;
        }

        /// <summary>
        /// Traverses all left fields of the <see cref="RegExp"/>.
        /// </summary>
        /// <param name="right">The initial right part of the <see cref="RegExp"/>.</param>
        /// <returns>Returns the final string of everyting right of the original.</returns>
        private string TraverseRight(RegExp right)
        {
            string rightString = terminals;
            if (right.op != Operator.ONE) { rightString = rightString + EnumToChar(left.op); }
            if (right.RightRegExp != null)
            {
                string evenMoreLeft = TraverseLeft(right.RightRegExp);
                rightString = evenMoreLeft + rightString;
            }
            return rightString;
        }

        /// <summary>
        /// Translates a <see cref="Operator"/> to a <see cref="char"/>.
        /// </summary>
        /// <param name="op">The operator to translate.</param>
        /// <returns>The <see cref="char"/> that represents the given <see cref="Operator"/>.</returns>
        private char EnumToChar(Operator op)
        {
            char operatorAsChar = '$';

            //Also, the worlds worst switch case
            switch (op)
            {
                case Operator.DOT: operatorAsChar = '.'; break;
                case Operator.OR: operatorAsChar = '|'; break;
                case Operator.PLUS: operatorAsChar = '+'; break;
                case Operator.STAR: operatorAsChar = '*'; break;
            }

            return operatorAsChar;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public RegExp()
        {
            op = Operator.ONE;
            terminals = "";
            left = null;
            right = null;
        }

        public string GetLanguageString(int maxSteps)
        {
            SortedSet<String> set = GetLanguageSet(maxSteps);
            string result = "";

            foreach (string symbol in set)
                result += symbol;

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
    }
}

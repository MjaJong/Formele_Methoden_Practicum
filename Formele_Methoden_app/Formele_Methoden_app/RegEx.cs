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

        public HashSet<string> GetLanguage(int maxSteps)
        {
            HashSet<String> emptyLanguage = new HashSet<String>(compareByLength);
            HashSet<String> languageResult = new HashSet<String>(compareByLength);
            HashSet<String> languageLeft, languageRight;

            if (maxSteps < 1) return emptyLanguage;

            return languageResult;
        }

        private class CompareByLength : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                if (x.Length == y.Length)
                { return x.CompareTo(y); }
                else
                { return x.Length - y.Length; }
            }



            public int GetHashCode(string obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}

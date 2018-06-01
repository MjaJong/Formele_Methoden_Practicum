using System;

namespace Formele_Methoden_app
{
    public class RegExpTest
    {
        private RegExp expr1, expr2, expr3, expr4, expr5, a, b, all;

        public RegExpTest()
        {
            a = new RegExp("a");
            b = new RegExp("b");

            // expr1: "baa"
            expr1 = new RegExp("baa");
            // expr2: "bb"
            expr2 = new RegExp("bb");
            // expr3: "baa | baa"
            expr3 = expr1.Or(expr2);

            // all: "(a|b)*"
            all = (a.Or(b)).Star();

            // expr4: "(baa | baa)+"
            expr4 = expr3.Plus();
            // expr5: "(baa | baa)+ (a|b)*"
            expr5 = expr4.Dot(all);
        }

        public void testLanguage()
        {
            Console.Write("expr4 = " + expr4.ToString());

            Console.WriteLine("taal van (baa):\n" + expr1.GetLanguageString(5));
            Console.WriteLine("taal van (bb):\n" + expr2.GetLanguageString(5));
            Console.WriteLine("taal van (baa | bb):\n" + expr3.GetLanguageString(5));

            Console.WriteLine("taal van (a|b)*:\n" + all.GetLanguageString(5));
            Console.WriteLine("taal van (baa | bb)+:\n" + expr4.GetLanguageString(5));
            Console.WriteLine("taal van (baa | bb)+ (a|b)*:\n" + expr5.GetLanguageString(6));
        }
    }
}

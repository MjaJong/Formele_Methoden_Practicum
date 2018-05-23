using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formele_Methoden_app
{
    class Program
    {
        static void Main(string[] args)
        {
            Automata<String> auto1 = TestAutomata.getExampleSlide8Lesson2();
            Automata<String> auto2 = TestAutomata.getExampleSlide14Lesson2();

            auto1.PrintTransitions();
            Console.WriteLine("-----------------------------------------\n");
            auto2.PrintTransitions();
            Console.WriteLine("-----------------------------------------\n");

            RegExpTest ret = new RegExpTest();
            ret.testLanguage();
            Console.WriteLine("-----------------------------------------\n");
            Console.Read();
        }
    }
}

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
            Console.WriteLine("Auto1 is dfa? " + auto1.IsDfa());
            Console.WriteLine("-----------------------------------------");
            auto2.PrintTransitions();
            Console.WriteLine("Auto2 is dfa? " + auto2.IsDfa());
            Console.WriteLine("-----------------------------------------");
        }
    }
}

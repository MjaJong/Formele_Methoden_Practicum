using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formele_Methoden_app
{
    class Program
    {
        private const string VALID_STRING_1 = "aba";
        private const string VALID_STRING_2 = "aaaba"; //Actually invalid, damned test
        private const string VALID_STRING_3 = "abababa";
        private const string INVALID_STRING_1 = "bba";
        private const string INVALID_STRING_2 = "aaa";
        private const string INVALID_STRING_3 = "bbbbaaa";

        static void Main(string[] args)
        {
            Automata<String> auto1 = TestAutomata.getExampleSlide8Lesson2();
            Automata<String> auto2 = TestAutomata.getExampleSlide14Lesson2();

            auto1.PrintTransitions();
            Console.WriteLine("Auto1 is dfa? " + auto1.IsDfa());
            Console.WriteLine("String {0} is a {1} string for Auto1.", VALID_STRING_1, auto1.IsStringAcceptable(VALID_STRING_1) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for Auto1.", VALID_STRING_2, auto1.IsStringAcceptable(VALID_STRING_2) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for Auto1.", VALID_STRING_3, auto1.IsStringAcceptable(VALID_STRING_3) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for Auto1.", INVALID_STRING_1, auto1.IsStringAcceptable(INVALID_STRING_1) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for Auto1.", INVALID_STRING_2, auto1.IsStringAcceptable(INVALID_STRING_2) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for Auto1.", INVALID_STRING_3, auto1.IsStringAcceptable(INVALID_STRING_3) ? "valid" : "invalid");
            Console.WriteLine("-----------------------------------------");
            auto2.PrintTransitions();
            Console.WriteLine("Auto2 is dfa? " + auto2.IsDfa());
            Console.WriteLine("-----------------------------------------");
        }
    }
}

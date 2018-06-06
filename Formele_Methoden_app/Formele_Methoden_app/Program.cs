using System;

namespace Formele_Methoden_app
{
    class Program
    {
        private const string VALID_STRING_1 = "aba";
        private const string INVALID_STRING_0 = "aaaba"; //Actually invalid, damned test
        private const string VALID_STRING_2 = "abababa";
        private const string INVALID_STRING_1 = "bba";
        private const string INVALID_STRING_2 = "aaa";
        private const string INVALID_STRING_3 = "bbbbaaa";

        private const string THOMPSON_TEST_1 = "baabaabb";
        private const string THOMPSON_TEST_2 = "ababaabb";
        private const string THOMPSON_TEST_3 = "abaababb";

        static void Main(string[] args)
        {
            Automata<String> auto1 = TestAutomata.getExampleSlide8Lesson2();
            Automata<String> auto2 = TestAutomata.getExampleSlide14Lesson2();

            auto1.PrintTransitions();
            Console.WriteLine("Auto1 is dfa? " + auto1.IsDfa());
            Console.WriteLine("String {0} is a {1} string for Auto1.", VALID_STRING_1, auto1.IsStringAcceptable(VALID_STRING_1) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for Auto1.", INVALID_STRING_0, auto1.IsStringAcceptable(INVALID_STRING_0) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for Auto1.", VALID_STRING_2, auto1.IsStringAcceptable(VALID_STRING_2) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for Auto1.", INVALID_STRING_1, auto1.IsStringAcceptable(INVALID_STRING_1) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for Auto1.", INVALID_STRING_2, auto1.IsStringAcceptable(INVALID_STRING_2) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for Auto1.", INVALID_STRING_3, auto1.IsStringAcceptable(INVALID_STRING_3) ? "valid" : "invalid");
            Console.WriteLine("Generate a string of length 3. Resulting string = " + auto1.GenerateLanguageOfGivenLength(3));
            Console.WriteLine("-----------------------------------------");
            auto2.PrintTransitions();
            Console.WriteLine("Auto2 is dfa? " + auto2.IsDfa());
            Console.WriteLine("-----------------------------------------");

            RegExpTest ret = new RegExpTest();
            //ret.testLanguage();
            ret.testToString();
            Console.WriteLine("-----------------------------------------");

            RegExp regExp1 = new RegExp("baa");
            RegExp regExp2 = new RegExp("aba");
            RegExp regExp3 = new RegExp("bb");

            RegExp oneOrTwo = regExp1.Or(regExp2);
            RegExp orStar = oneOrTwo.Star();
            RegExp orStarDotThree = orStar.Dot(regExp3);
            RegExp orStarDotThreePlus = orStarDotThree.Plus();

            Console.WriteLine(orStarDotThreePlus.ToString());

            ThompsonConstruction thompson = new ThompsonConstruction();
            Automata<string> auto3 = thompson.GenerateNDFA(orStarDotThreePlus);

            Console.WriteLine(auto3.IsDfa());
            Console.WriteLine("-----------------------------------------");

            auto3.PrintTransitions();
            Console.WriteLine("-----------------------------------------");

            //Should all be valid
            Console.WriteLine("String {0} is a {1} string for Auto3.", THOMPSON_TEST_1, auto3.IsStringAcceptable(THOMPSON_TEST_1) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for Auto3.", THOMPSON_TEST_2, auto3.IsStringAcceptable(THOMPSON_TEST_2) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for Auto3.", THOMPSON_TEST_3, auto3.IsStringAcceptable(THOMPSON_TEST_3) ? "valid" : "invalid");
            Console.WriteLine("-----------------------------------------");

            ret.testLanguage();
            Console.WriteLine("-----------------------------------------\n");

            DiagramWriter writer = new DiagramWriter();
            writer.WriteToGVFile(auto1, "test", "", true);
            Console.Read();
        }
    }
}

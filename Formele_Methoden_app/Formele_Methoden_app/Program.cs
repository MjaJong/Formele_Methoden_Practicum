using System;

namespace Formele_Methoden_app
{
    class Program
    {
        #region constant test strings for automata
        private const string VALID_STRING_1 = "aba";
        private const string INVALID_STRING_0 = "aaaba";
        private const string VALID_STRING_2 = "abababa";
        private const string INVALID_STRING_1 = "bba";
        private const string INVALID_STRING_2 = "aaa";
        private const string INVALID_STRING_3 = "bbbbaaa";

        private const string THOMPSON_TEST_1 = "baabaabb";
        private const string THOMPSON_TEST_2 = "ababaabb";
        private const string THOMPSON_TEST_3 = "abaababb";

        private const string NDFA_TO_DFA_VALID_1 = "aaaa";
        private const string NDFA_TO_DFA_VALID_2 = "aabaaaa";
        private const string NDFA_TO_DFA_VALID_3 = "bbaaa";
        private const string NDFA_TO_DFA_VALID_4 = "bbabbbaaa";
        private const string NDFA_TO_DFA_INVALID_1 = "bbb";
        private const string NDFA_TO_DFA_INVALID_2 = "bbaa";
        private const string NDFA_TO_DFA_INVALID_3 = "aabbbb";
        #endregion

        static void Main(string[] args)
        {
            bool exitLoop = false;
            while (true)
            {
                Console.WriteLine("Commands are as follows:");
                Console.WriteLine("TestClass for the test of the (N)DFA class.");
                Console.WriteLine("Regex for the test of the RegExp and thompson construction.");
                Console.WriteLine("NdfaToDfa for the test of the NDFA to DFA class.");
                Console.WriteLine("Operations for the test of the dfa operations and minimalization.");
                Console.WriteLine("And exit to stop the application.");
                Console.WriteLine("Please input a command to show a test case:");

                string line = Console.ReadLine().ToLower();

                switch (line)
                {
                    case "testclass":
                        Console.Clear();
                        TestNDFAClass();
                        break;
                    case "regex":
                        Console.Clear();
                        TestRegExAndThompson();
                        break;
                    case "ndfatodfa":
                        Console.Clear();
                        TestNdfaToDfa();
                        break;
                    case "operations":
                        Console.Clear();
                        TestMutationAndMinimalization();
                        break;
                    case "exit":
                        exitLoop = true; ;
                        break;
                    default:
                        Console.WriteLine("Invalid command given. \n");
                        break;
                }
                if (exitLoop) { break; }
            }
        }

        private static void TestNDFAClass()
        {
            Automata<String> NDFA1 = TestAutomata.getExampleSlide8Lesson2();
            Automata<String> NDFA2 = TestAutomata.getExampleSlide14Lesson2();

            NDFA1.PrintTransitions();
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("NDFA1 is dfa? " + NDFA1.IsDfa());
            Console.WriteLine("-----------------------------------------");
            foreach (string state in NDFA1.StartStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");
            foreach (string state in NDFA1.FinalStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("String {0} is a {1} string for Auto1.", VALID_STRING_1, NDFA1.IsStringAcceptable(VALID_STRING_1) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for Auto1.", INVALID_STRING_0, NDFA1.IsStringAcceptable(INVALID_STRING_0) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for Auto1.", VALID_STRING_2, NDFA1.IsStringAcceptable(VALID_STRING_2) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for Auto1.", INVALID_STRING_1, NDFA1.IsStringAcceptable(INVALID_STRING_1) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for Auto1.", INVALID_STRING_2, NDFA1.IsStringAcceptable(INVALID_STRING_2) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for Auto1.", INVALID_STRING_3, NDFA1.IsStringAcceptable(INVALID_STRING_3) ? "valid" : "invalid");
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("Generate a string of length 9. Resulting string = " + NDFA1.GenerateLanguageOfGivenLength(9));
            Console.WriteLine("-----------------------------------------");

            NDFA2.PrintTransitions();
            Console.WriteLine("NDFA2 is dfa? " + NDFA2.IsDfa());
            Console.WriteLine("-----------------------------------------");
        }

        private static void TestRegExAndThompson()
        {
            RegExpTest ret = new RegExpTest();
            ret.testLanguage();
            ret.testToString();

            RegExp regExp1 = new RegExp("baa");
            RegExp regExp2 = new RegExp("aba");
            RegExp regExp3 = new RegExp("bb");

            RegExp oneOrTwo = regExp1.Or(regExp2);
            RegExp orStar = oneOrTwo.Star();
            RegExp orStarDotThree = orStar.Dot(regExp3);
            RegExp orStarDotThreePlus = orStarDotThree.Plus();
            Console.WriteLine("-----------------------------------------");

            Console.WriteLine("And conversion by use of the thompson construction:");
            Console.WriteLine("-----------------------------------------");

            ThompsonConstruction thompson = new ThompsonConstruction();
            Automata<string> thompsonNDFA = thompson.GenerateNDFA(orStarDotThreePlus);

            Console.WriteLine("thompsonNDFA is dfa? " + thompsonNDFA.IsDfa());
            Console.WriteLine("-----------------------------------------");

            thompsonNDFA.PrintTransitions();
            Console.WriteLine("-----------------------------------------");

            foreach (string state in thompsonNDFA.StartStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            foreach (string state in thompsonNDFA.FinalStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            Console.WriteLine("String {0} is a {1} string for Auto3.", THOMPSON_TEST_1, thompsonNDFA.IsStringAcceptable(THOMPSON_TEST_1) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for Auto3.", THOMPSON_TEST_2, thompsonNDFA.IsStringAcceptable(THOMPSON_TEST_2) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for Auto3.", THOMPSON_TEST_3, thompsonNDFA.IsStringAcceptable(THOMPSON_TEST_3) ? "valid" : "invalid");
            Console.WriteLine("-----------------------------------------");
        }

        private static void TestNdfaToDfa()
        {
            Automata<string> testNdfa = TestAutomata.ndfaToDfaTest();
            Console.WriteLine("testNdfa is dfa? " + testNdfa.IsDfa());
            Console.WriteLine("-----------------------------------------");

            testNdfa.PrintTransitions();
            Console.WriteLine("-----------------------------------------");

            Console.WriteLine("StartStatesIn NDFA: ");
            foreach(string startState in testNdfa.StartStates) { Console.WriteLine(startState); }
            Console.WriteLine("-----------------------------------------");

            Console.WriteLine("EndStates NDFA: ");
            foreach (string endState in testNdfa.FinalStates) { Console.WriteLine(endState); }
            Console.WriteLine("-----------------------------------------");

            Console.WriteLine("String {0} is a {1} string for testNdfa.", NDFA_TO_DFA_VALID_1, testNdfa.IsStringAcceptable(NDFA_TO_DFA_VALID_1) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for testNdfa.", NDFA_TO_DFA_VALID_2, testNdfa.IsStringAcceptable(NDFA_TO_DFA_VALID_2) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for testNdfa.", NDFA_TO_DFA_VALID_3, testNdfa.IsStringAcceptable(NDFA_TO_DFA_VALID_3) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for testNdfa.", NDFA_TO_DFA_VALID_4, testNdfa.IsStringAcceptable(NDFA_TO_DFA_VALID_4) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for testNdfa.", NDFA_TO_DFA_INVALID_1, testNdfa.IsStringAcceptable(NDFA_TO_DFA_INVALID_1) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for testNdfa.", NDFA_TO_DFA_INVALID_2, testNdfa.IsStringAcceptable(NDFA_TO_DFA_INVALID_2) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for testNdfa.", NDFA_TO_DFA_INVALID_3, testNdfa.IsStringAcceptable(NDFA_TO_DFA_INVALID_3) ? "valid" : "invalid");
            Console.WriteLine("-----------------------------------------");

            NdfaToDfa<string> converter = new NdfaToDfa<string>();
            Automata<string> testNdfaDFA = converter.TransformNdfaIntoDfa(testNdfa);
            Console.WriteLine("testNdfaDFA is dfa? " + testNdfaDFA.IsDfa());
            Console.WriteLine("-----------------------------------------");

            testNdfaDFA.PrintTransitions();
            Console.WriteLine("-----------------------------------------");

            foreach (string state in testNdfaDFA.StartStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            foreach (string state in testNdfaDFA.FinalStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            Console.WriteLine("And the same strings as befor in the DFA:");
            Console.WriteLine();
            Console.WriteLine("String {0} is a {1} string for testNdfaDFA.", NDFA_TO_DFA_VALID_1, testNdfaDFA.IsStringAcceptable(NDFA_TO_DFA_VALID_1) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for testNdfaDFA.", NDFA_TO_DFA_VALID_2, testNdfaDFA.IsStringAcceptable(NDFA_TO_DFA_VALID_2) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for testNdfaDFA.", NDFA_TO_DFA_VALID_3, testNdfaDFA.IsStringAcceptable(NDFA_TO_DFA_VALID_3) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for testNdfaDFA.", NDFA_TO_DFA_VALID_4, testNdfaDFA.IsStringAcceptable(NDFA_TO_DFA_VALID_4) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for testNdfaDFA.", NDFA_TO_DFA_INVALID_1, testNdfaDFA.IsStringAcceptable(NDFA_TO_DFA_INVALID_1) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for testNdfaDFA.", NDFA_TO_DFA_INVALID_2, testNdfaDFA.IsStringAcceptable(NDFA_TO_DFA_INVALID_2) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for testNdfaDFA.", NDFA_TO_DFA_INVALID_3, testNdfaDFA.IsStringAcceptable(NDFA_TO_DFA_INVALID_3) ? "valid" : "invalid");
            Console.WriteLine("-----------------------------------------");
        }

        private static void TestMutationAndMinimalization()
        {
            Automata<string> L1 = TestAutomata.dfaMutationTestL1();
            Automata<string> L4 = TestAutomata.dfaMutationTestL4();

            Console.WriteLine("L1 is dfa? " + L1.IsDfa());
            Console.WriteLine("-----------------------------------------");

            L1.PrintTransitions();
            Console.WriteLine("-----------------------------------------");

            foreach (string state in L1.StartStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            foreach (string state in L1.FinalStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            Console.WriteLine("L4 is dfa? " + L4.IsDfa());
            Console.WriteLine("-----------------------------------------");

            L4.PrintTransitions();
            Console.WriteLine("-----------------------------------------");

            foreach (string state in L4.StartStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            foreach (string state in L4.FinalStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            //Not L1
            DfaMutation<string> dfaMutator = new DfaMutation<string>();
            Automata<string> notL1 = dfaMutator.NotDfa(L1);

            foreach (string state in notL1.StartStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            foreach (string state in notL1.FinalStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            //Reverted L4
            Automata<string> revertedL4 = dfaMutator.ReverseDfa(L4);

            Console.WriteLine("revertedL4 is dfa? " + revertedL4.IsDfa());
            Console.WriteLine("-----------------------------------------");

            revertedL4.PrintTransitions();
            Console.WriteLine("-----------------------------------------");

            foreach (string state in revertedL4.StartStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            foreach (string state in revertedL4.FinalStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            Automata<string> L1AndL4 = dfaMutator.CombineAutomataAnd(L1, L4);

            Console.WriteLine("L1AndL4 is dfa? " + L1AndL4.IsDfa());
            Console.WriteLine("-----------------------------------------");

            L1AndL4.PrintTransitions();
            Console.WriteLine("-----------------------------------------");

            foreach (string state in L1AndL4.StartStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            foreach (string state in L1AndL4.FinalStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            Automata<string> L1OrL4 = dfaMutator.CombinaAutomataOr(L1, L4);

            Console.WriteLine("L1OrL4 is dfa? " + L1OrL4.IsDfa());
            Console.WriteLine("-----------------------------------------");

            L1OrL4.PrintTransitions();
            Console.WriteLine("-----------------------------------------");

            foreach (string state in L1OrL4.StartStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            foreach (string state in L1OrL4.FinalStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            Minimizer<string> minimizer = new Minimizer<string>();
            Automata<string> L1AndL4Minimal = minimizer.MinimizeUsingBrzozowski(L1AndL4);

            Console.WriteLine("L1AndL4Minimal is dfa? " + L1AndL4Minimal.IsDfa());
            Console.WriteLine("-----------------------------------------");

            L1AndL4Minimal.PrintTransitions();
            Console.WriteLine("-----------------------------------------");

            foreach (string state in L1AndL4Minimal.StartStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            foreach (string state in L1AndL4Minimal.FinalStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");
        }
    }
}

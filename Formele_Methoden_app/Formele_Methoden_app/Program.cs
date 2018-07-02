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

        private const string NDFA_TO_DFA_VALID_1 = "aaaa";
        private const string NDFA_TO_DFA_VALID_2 = "aabaaaa";
        private const string NDFA_TO_DFA_VALID_3 = "bbaaa";
        private const string NDFA_TO_DFA_VALID_4 = "bbabbbaaa";
        private const string NDFA_TO_DFA_INVALID_1 = "bbb";
        private const string NDFA_TO_DFA_INVALID_2 = "bbaa";
        private const string NDFA_TO_DFA_INVALID_3 = "aabbbb";

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

            //At this point, somehow this regexp screws up. For now NDFA -> DFA is being implemented
            ////Builds: a*(aa+|ba*b)*(abba|baab|bbbb)+
            //RegExp secondTest1 = new RegExp("a");
            //RegExp secondTest2 = new RegExp("b");
            //RegExp secondTest3 = new RegExp("abba");
            //RegExp secondTest4 = new RegExp("baab");
            //RegExp secondTest5 = new RegExp("bbbb");

            //RegExp aStar = secondTest1.Star();
            //RegExp aPlus = secondTest1.Plus();
            //RegExp orFirstHalf = secondTest1.Dot(aPlus); //a.a+
            //RegExp orSecondHalf = secondTest2.Dot(aStar.Dot(secondTest2));//b.a*.b
            //RegExp firstOr = orFirstHalf.Or(orSecondHalf);//a.a+|b.a*.b
            //RegExp firstOrStarred = firstOr.Star();//(a.a+|b.a*.b)*
            //RegExp secondTestFirstHalf = aStar.Dot(firstOrStarred);//a*.(a.a+|b.a*.b)*
            //RegExp secondOr = secondTest3.Or(secondTest4.Or(secondTest5));//abba|baab|bbbb
            //RegExp secondTestSecondHalf = secondOr.Plus();//(abba|baab|bbbb)+
            //RegExp secondTest = secondTestFirstHalf.Dot(secondTestSecondHalf);//(a*.(a.a+|b.a*.b)*).((abba|baab|bbbb)+)


            //Console.WriteLine(secondTest);
            //Console.WriteLine("-----------------------------------------");

            //Automata<string> auto4 = thompson.GenerateNDFA(secondTest);//This fails due to the final dot operator, which fucking sucks. Probably anyway. I have no idea of how to fix this.
            //Console.WriteLine(auto4.IsDfa());
            //Console.WriteLine("-----------------------------------------");

            //auto4.PrintTransitions();
            //Console.WriteLine("-----------------------------------------");

            Automata<string> auto4 = TestAutomata.ndfaToDfaTest();
            Console.WriteLine("auto4 is dfa? " + auto4.IsDfa());
            Console.WriteLine("String {0} is a {1} string for Auto4.", NDFA_TO_DFA_VALID_1, auto4.IsStringAcceptable(NDFA_TO_DFA_VALID_1) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for Auto4.", NDFA_TO_DFA_VALID_2, auto4.IsStringAcceptable(NDFA_TO_DFA_VALID_2) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for Auto4.", NDFA_TO_DFA_VALID_3, auto4.IsStringAcceptable(NDFA_TO_DFA_VALID_3) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for Auto4.", NDFA_TO_DFA_VALID_4, auto4.IsStringAcceptable(NDFA_TO_DFA_VALID_4) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for Auto4.", NDFA_TO_DFA_INVALID_1, auto4.IsStringAcceptable(NDFA_TO_DFA_INVALID_1) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for Auto4.", NDFA_TO_DFA_INVALID_2, auto4.IsStringAcceptable(NDFA_TO_DFA_INVALID_2) ? "valid" : "invalid");
            Console.WriteLine("String {0} is a {1} string for Auto4.", NDFA_TO_DFA_INVALID_3, auto4.IsStringAcceptable(NDFA_TO_DFA_INVALID_3) ? "valid" : "invalid");
            Console.WriteLine("-----------------------------------------");

            NdfaToDfa<string> converter = new NdfaToDfa<string>();
            Automata<string> auto4DFA = converter.TransformNdfaIntoDfa(auto4);
            Console.WriteLine("auto4DFA is dfa? " + auto4DFA.IsDfa());
            Console.WriteLine("-----------------------------------------");

            auto4DFA.PrintTransitions();
            Console.WriteLine("-----------------------------------------");

            foreach(string state in auto4DFA.StartStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            foreach (string state in auto4DFA.FinalStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            //This needs a better test, due to these not being dfa's
            Automata<string> auto5 = TestAutomata.dfaMutationTestL1();
            Automata<string> auto6 = TestAutomata.dfaMutationTestL4();

            Console.WriteLine("auto5 is dfa? " + auto5.IsDfa());
            Console.WriteLine("-----------------------------------------");

            auto5.PrintTransitions();
            Console.WriteLine("-----------------------------------------");

            foreach (string state in auto5.StartStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            foreach (string state in auto5.FinalStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            //Different dfa here
            Console.WriteLine("auto6 is dfa? " + auto6.IsDfa());
            Console.WriteLine("-----------------------------------------");

            auto6.PrintTransitions();
            Console.WriteLine("-----------------------------------------");

            foreach (string state in auto6.StartStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            foreach (string state in auto6.FinalStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            //Not L1
            DfaMutation<string> dfaMutator = new DfaMutation<string>();
            Automata<string> notL1 = dfaMutator.NotDfa(auto5);

            foreach (string state in notL1.StartStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            foreach (string state in notL1.FinalStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            //Reverted L4
            Automata<string> revertedL4 = dfaMutator.ReverseDfa(auto6);

            Console.WriteLine("revertedL4 is dfa? " + revertedL4.IsDfa());
            Console.WriteLine("-----------------------------------------");

            revertedL4.PrintTransitions();
            Console.WriteLine("-----------------------------------------");

            foreach (string state in revertedL4.StartStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            foreach (string state in revertedL4.FinalStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            //DiagramWriter writer = new DiagramWriter();
            //writer.WriteToGVFile(auto1, "test", "C:/Users/menno/Documents/Formele methoden/Formele_Methoden_Practicum/Formele_Methoden_app", true);

            //Well, rewrite the process to better reflect what you do by hand. 
            Automata<string> L1AndL4 = dfaMutator.CombineAutomataAnd(auto5, auto6);

            Console.WriteLine("L1AndL4 is dfa? " + L1AndL4.IsDfa());
            Console.WriteLine("-----------------------------------------");

            L1AndL4.PrintTransitions();
            Console.WriteLine("-----------------------------------------");

            foreach (string state in L1AndL4.StartStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            foreach (string state in L1AndL4.FinalStates) { Console.WriteLine(state); }
            Console.WriteLine("-----------------------------------------");

            Automata<string> L1OrL4 = dfaMutator.CombinaAutomataOr(auto5, auto6);

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

            Console.Read();
        }
    }
}

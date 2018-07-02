using System;

namespace Formele_Methoden_app
{
    public class TestAutomata
    {
        static public Automata<String> getExampleSlide8Lesson2()
        {
            char[] alphabet = { 'a', 'b' };
            Automata<String> automata = new Automata<string>(alphabet);

            automata.AddTransition(new Transition<String>("q0", "q1", 'a'));
            automata.AddTransition(new Transition<String>("q0", "q4", 'b'));

            automata.AddTransition(new Transition<String>("q1", "q4", 'a'));
            automata.AddTransition(new Transition<String>("q1", "q2", 'b'));

            automata.AddTransition(new Transition<String>("q2", "q3", 'a'));
            automata.AddTransition(new Transition<String>("q2", "q4", 'b'));
            automata.AddTransition(new Transition<String>("q3", "q1", 'a'));
            automata.AddTransition(new Transition<String>("q3", "q2", 'b'));

            // the error state, loops for a and b:
            automata.AddTransition(new Transition<String>("q4", 'a'));
            automata.AddTransition(new Transition<String>("q4", 'b'));

            // only on start state in a dfa:
            automata.DefineAsStartState("q0");

            // two final states:
            automata.DefineAsFinalState("q2");
            automata.DefineAsFinalState("q3");

            return automata;
        }

        static public Automata<String> getExampleSlide14Lesson2()
        {
            char[] alphabet = { 'a', 'b' };
            Automata<String> automata = new Automata<String>(alphabet);

            automata.AddTransition(new Transition<String>("A", "C", 'a'));
            automata.AddTransition(new Transition<String>("A", "B", 'b'));
            automata.AddTransition(new Transition<String>("A", "C", 'b'));

            automata.AddTransition(new Transition<String>("B", "C", 'b'));
            automata.AddTransition(new Transition<String>("B", "C"));

            automata.AddTransition(new Transition<String>("C", "D", 'a'));
            automata.AddTransition(new Transition<String>("C", "E", 'a'));
            automata.AddTransition(new Transition<String>("C", "D", 'b'));

            automata.AddTransition(new Transition<String>("D", "B", 'a'));
            automata.AddTransition(new Transition<String>("D", "C", 'a'));

            automata.AddTransition(new Transition<String>("E", 'a'));
            automata.AddTransition(new Transition<String>("E", "D"));

            // only on start state in a dfa:
            automata.DefineAsStartState("A");

            // two final states:
            automata.DefineAsFinalState("C");
            automata.DefineAsFinalState("E");

            return automata;
        }

        static public Automata<String> ndfaToDfaTest()
        {
            char[] alphabet = { 'a', 'b' };
            Automata<String> automata = new Automata<String>(alphabet);

            automata.AddTransition(new Transition<string>("A", "B", 'a'));
            automata.AddTransition(new Transition<string>("A", "C", 'b'));
            automata.AddTransition(new Transition<string>("B", "D", 'a'));
            automata.AddTransition(new Transition<string>("C", "B", 'b'));
            automata.AddTransition(new Transition<string>("C", "D"));
            automata.AddTransition(new Transition<string>("D", "A", 'b'));
            automata.AddTransition(new Transition<string>("D", "E", 'a'));
            automata.AddTransition(new Transition<string>("E", "C"));
            automata.AddTransition(new Transition<string>("E", "F", 'a'));

            automata.DefineAsStartState("A");
            automata.DefineAsFinalState("F");

            return automata;
        }

        static public Automata<String> dfaMutationTestL1()
        {
            char[] alphabet = { 'a', 'b' };
            Automata<String> automata = new Automata<String>(alphabet);

            automata.AddTransition(new Transition<string>("A", "B", 'b'));
            automata.AddTransition(new Transition<string>("B", "C", 'a'));
            automata.AddTransition(new Transition<string>("C", "D", 'b'));
            automata.AddTransition(new Transition<string>("D", "E", 'a'));
            automata.AddTransition(new Transition<string>("E", "F", 'a'));

            automata.AddTransition(new Transition<string>("A", "G", 'a'));
            automata.AddTransition(new Transition<string>("B", "G", 'b'));
            automata.AddTransition(new Transition<string>("C", "G", 'a'));
            automata.AddTransition(new Transition<string>("D", "G", 'b'));
            automata.AddTransition(new Transition<string>("E", "G", 'b'));

            //End point shenenigans so testing goes a bit smoother
            automata.AddTransition(new Transition<string>("F", 'a'));
            automata.AddTransition(new Transition<string>("F", 'b'));
            automata.AddTransition(new Transition<string>("G", 'a'));
            automata.AddTransition(new Transition<string>("G", 'b'));

            automata.DefineAsStartState("A");
            automata.DefineAsFinalState("F");

            return automata;
        }

        static public Automata<String> dfaMutationTestL4()
        {
            char[] alphabet = { 'a', 'b' };
            Automata<String> automata = new Automata<String>(alphabet);

            automata.AddTransition(new Transition<string>("A", "B", 'a'));
            automata.AddTransition(new Transition<string>("B", "D", 'b'));
            automata.AddTransition(new Transition<string>("D", "E", 'b'));
            automata.AddTransition(new Transition<string>("B", 'a'));
            automata.AddTransition(new Transition<string>("D", "B", 'a'));

            automata.AddTransition(new Transition<string>("A", "C", 'b'));
            automata.AddTransition(new Transition<string>("C", "F", 'b'));
            automata.AddTransition(new Transition<string>("F", "G", 'a'));
            automata.AddTransition(new Transition<string>("C", "B", 'a'));
            automata.AddTransition(new Transition<string>("F", "C", 'b'));

            automata.AddTransition(new Transition<string>("E", 'a'));
            automata.AddTransition(new Transition<string>("E", 'b'));
            automata.AddTransition(new Transition<string>("G", 'a'));
            automata.AddTransition(new Transition<string>("G", 'b'));

            automata.DefineAsStartState("A");
            automata.DefineAsFinalState("E");
            automata.DefineAsFinalState("G");

            return automata;
        }
    }
}

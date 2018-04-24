using System;
using System.Collections.Generic;

namespace Formele_Methoden_app
{
    public class Automata<T> where T : IComparable
    {
        private ISet<Transition<T>> transitions;
        private ISet<T> states;
        private ISet<T> startStates;
        private ISet<T> finalStates;
        private ISet<char> symbols;

        public Automata(char[] chars) : this(new HashSet<char>(chars))
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="symbols"></param>
        public Automata(HashSet<char> symbols = null)
        {
            if (symbols == null) this.symbols = new HashSet<char>();
            else this.symbols = symbols;

            transitions = new HashSet<Transition<T>>();
            states = new HashSet<T>();
            startStates = new HashSet<T>();
            finalStates = new HashSet<T>();
        }

        /// <summary>
        /// Add transition to the set
        /// </summary>
        /// <param name="transition"></param>
        public void AddTransition(Transition<T> transition)
        {
            transitions.Add(transition);
            states.Add(transition.FromState);
            states.Add(transition.ToState);
        }

        /// <summary>
        /// Add state to startstates
        /// </summary>
        /// <param name="state">The state</param>
        public void DefineAsStartState(T state)
        {
            states.Add(state);
            startStates.Add(state);
        }

        /// <summary>
        /// Add state to finalstates
        /// </summary>
        /// <param name="state">The state</param>
        public void DefineAsFinalState(T state)
        {
            states.Add(state);
            finalStates.Add(state);
        }

        /// <summary>
        /// Print all transitions to console
        /// </summary>
        public void PrintTransitions()
        {
            foreach (Transition<T> transition in transitions)
            {
                Console.WriteLine(transition);
            }
        }

        public bool IsDfa()
        {
            bool dfa = true;

            foreach (T from in states)
            {
                foreach (char symbol in symbols)
                {
                    dfa = dfa && GetToStates(from, symbol).Count == 1;
                }
            }
            return dfa;
        }

        /// <summary>
        /// Dud method so that VS compiles.
        /// TODO: Replace this as soon as humanly possible.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private List<char> GetToStates(T from, char symbol)
        {
            return new List<char>();
        }
    }
}

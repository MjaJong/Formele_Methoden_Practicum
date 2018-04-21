using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formele_Methoden_app
{
    public class Automata<T> where T : IComparable
    {
        private ISet<Transition<T>> transitions;
        private ISet<T> states;
        private ISet<T> startStates;
        private ISet<T> finalStates;
        private ISet<char> symbols;

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
        public void addTransition(Transition<T> transition)
        {
            transitions.Add(transition);
            states.Add(transition.GetFromState());
            states.Add(transition.GetToState());
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
        public void defineAsFinalState(T state)
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
                    dfa = dfa && getToStates(from, symbol).size() == 1;
                }
            }
            return dfa;
        }
    }
}

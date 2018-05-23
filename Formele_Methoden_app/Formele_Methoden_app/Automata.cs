using System;
using System.Collections.Generic;
using System.Linq;

namespace Formele_Methoden_app
{
    public class Automata<T> where T : IComparable
    {
        private HashSet<Transition<T>> transitions;
        private HashSet<T> states;
        private HashSet<T> startStates;
        private HashSet<T> finalStates;
        private HashSet<char> symbols;

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
        /// Gets all to states that have the given from state and symbol.
        /// </summary>
        /// <param name="from">The from state to check for.</param>
        /// <param name="symbol">The symbol to check for.</param>
        /// <returns>A list containing all to states.</returns>
        private List<T> GetToStates(T from, char symbol)
        {
           List<T> toStates =  new List<T>();

            foreach(Transition<T> transition in transitions)
            {
                if(transition.FromState.Equals(from) && transition.Identifier.Equals(symbol)) { toStates.Add(transition.ToState); }
            }

            return toStates;
        }

        /// <summary>
        /// Checks whether the given string fits within this automata
        /// </summary>
        /// <param name="stringToVerify">The string to check.</param>
        /// <returns>A boolean indicator that is true if the string can be formed by this automata by traversing it's nodes.</returns>
        public bool IsStringAcceptable(string stringToVerify)
        {
            bool stringIsAccepted = false;

            //Pak start states + eerste element in char array
            char charToCheckFor = stringToVerify[0];
            HashSet<T> startingStates = startStates;

            //Verwijder eerste character
            if (stringToVerify.Count() > 1) { stringToVerify = stringToVerify.Substring(1); }
            else stringToVerify = "";

            //Voor iedere uitgaande transitie check of het kan met het gegeven character
            foreach (T state in startingStates)
            {
                if(stringToVerify.Count() == 0)
                {
                    stringIsAccepted = finalStates.Contains(state);
                    break;
                }

                IEnumerable<Transition<T>> validTransitions = transitions.Where(x => x.Identifier == charToCheckFor && x.FromState.Equals(state));
                
                foreach(Transition<T> transition in validTransitions)
                {
                    stringIsAccepted = CheckNextNode(transition.ToState, stringToVerify);
                    if (stringIsAccepted) { break; }
                }
            }

            //Als er een terug komt met true, dan bestaat de string
            return stringIsAccepted;
        }

        /// <summary>
        /// Recursively checks to see if we can form the string
        /// </summary>
        /// <param name="state">the state to check</param>
        /// <param name="remainingString">the remainder of the string to check</param>
        /// <returns>A boolean that is true if the string has been fully formed</returns>
        private bool CheckNextNode(T state, string stringToVerify)
        {
            bool stringIsAccepted = false;

            if (stringToVerify.Count() == 0) //Early escape if we're finished
            {
                stringIsAccepted = finalStates.Contains(state);
                return stringIsAccepted;
            } 

            char currentCharacter = stringToVerify[0];

            if (stringToVerify.Count() > 1) { stringToVerify = stringToVerify.Substring(1); }
            else stringToVerify = "";

            IEnumerable<Transition<T>> validTransitions = transitions.Where(x => x.Identifier == currentCharacter && x.FromState.Equals(state));
            
            foreach(Transition<T> transition in validTransitions)
            {
                stringIsAccepted = CheckNextNode(transition.ToState, stringToVerify);
                if (stringIsAccepted) { break; }
            }

            return stringIsAccepted;
        }

        /// <summary>
        /// Gets the minimum length of a language with this automaton.
        /// </summary>
        /// <returns>A integer that is equal to the mimimum length of the language.</returns>
        private int GetMinimumLanguageLength()
        {
            int length = 0;

            foreach(T state in startStates)
            {
                int returnedValue = CountNextState(length, state);

                if(returnedValue == 0) { continue; }

                if (length == 0) { length = returnedValue; }
                else if (length > returnedValue) { length = returnedValue; }
            }

            return length;
        }

        /// <summary>
        /// Recursively loop through the automaton
        /// </summary>
        /// <returns>An int that is the total count found while looping</returns>
        private int CountNextState(int previousCount, T state)
        {
            int newCount = previousCount + 1;

            if (finalStates.Contains(state))
            {
                return newCount;
            }
            else
            {
                IEnumerable<Transition<T>> transitionsForThisState = transitions.Where(x => x.FromState.Equals(state) && !x.FromState.Equals(x.ToState));

                if(transitionsForThisState.ToArray().Count() == 0) { return 0; }

                int nextCount = 0;

                foreach(Transition<T> transition in transitionsForThisState)
                {
                   int returnedValue = CountNextState(newCount, transition.ToState);

                   if (returnedValue == 0) { continue; }

                   if (nextCount == 0) { nextCount = returnedValue; }
                   else if(nextCount > returnedValue) { nextCount = returnedValue; }
                }

                return nextCount;
            }
        }
    }
}

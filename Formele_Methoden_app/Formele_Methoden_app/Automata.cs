﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Formele_Methoden_app
{
    public class Automata<T> where T : IComparable
    {
        public HashSet<T> StartStates { get; private set; }
        public HashSet<T> FinalStates { get; private set; }
        public HashSet<T> States { get; private set; }
        public HashSet<char> Symbols { get; private set; }
        public HashSet<Transition<T>> Transitions { get; private set; }

        public Automata(char[] chars) : this(new HashSet<char>(chars))
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="symbols"></param>
        public Automata(HashSet<char> symbols = null)
        {
            if (symbols == null) Symbols = new HashSet<char>();
            else Symbols = symbols;

            Transitions = new HashSet<Transition<T>>();
            States = new HashSet<T>();
            StartStates = new HashSet<T>();
            FinalStates = new HashSet<T>();
        }

        /// <summary>
        /// Add transition to the set
        /// </summary>
        /// <param name="transition"></param>
        public void AddTransition(Transition<T> transition)
        {
            Transitions.Add(transition);
            States.Add(transition.FromState);
            States.Add(transition.ToState);
        }

        /// <summary>
        /// Add state to startstates
        /// </summary>
        /// <param name="state">The state</param>
        public void DefineAsStartState(T state)
        {
            if (!DfaContainsState(state)) { States.Add(state); }
            StartStates.Add(state);
        }

        /// <summary>
        /// Add state to finalstates
        /// </summary>
        /// <param name="state">The state</param>
        public void DefineAsFinalState(T state)
        {
            if (!DfaContainsState(state)) { States.Add(state); }
            FinalStates.Add(state);
        }

        /// <summary>
        /// Print all transitions to console
        /// </summary>
        public void PrintTransitions()
        {
            foreach (Transition<T> transition in Transitions)
            {
                Console.WriteLine(transition);
            }
        }

        /// <summary>
        /// Check to see if this <see cref="Automata{T}"/> is a dfa.
        /// </summary>
        /// <returns>True if the <see cref="Automata{T}"/> is a DFA.</returns>
        public bool IsDfa()
        {
            bool dfa = true;

            if(States.Count == 0) { dfa = false; }

            foreach (T from in States)
            {
                List<Transition<T>> transitionsWithSymbol = Transitions.Where(x => x.FromState.Equals(from)).ToList();
                if(transitionsWithSymbol.Count != 2)
                {
                    dfa = false;
                    break;
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

            foreach(Transition<T> transition in Transitions)
            {
                if(transition.FromState.Equals(from) && transition.Identifier.Equals(symbol)) { toStates.Add(transition.ToState); }
            }

            return toStates;
        }

        /// <summary>
        /// Checks if a string is valid for this automaton
        /// </summary>
        /// <returns>A boolean indicating if the string is acceptable.</returns>
        public bool IsStringAcceptable(string stringToVerify)
        {
            if (IsDfa()) { return IsStringAcceptableDfa(stringToVerify); }
            else { return IsStringAcceptableNdfa(stringToVerify); }
        }

        /// <summary>
        /// Checks whether the given string fits within this automata
        /// </summary>
        /// <param name="stringToVerify">The string to check.</param>
        /// <returns>A boolean indicator that is true if the string can be formed by this automata by traversing it's nodes.</returns>
        private bool IsStringAcceptableDfa(string stringToVerify)
        {
            bool stringIsAccepted = false;

            //Pak start states + eerste element in char array
            char charToCheckFor = stringToVerify[0];
            HashSet<T> startingStates = StartStates;

            //Verwijder eerste character
            if (stringToVerify.Count() > 1) { stringToVerify = stringToVerify.Substring(1); }
            else stringToVerify = string.Empty;

            //Voor iedere uitgaande transitie check of het kan met het gegeven character
            foreach (T state in startingStates)
            {
                if(stringToVerify == string.Empty)
                {
                    stringIsAccepted = FinalStates.Contains(state);
                    break;
                }

                IEnumerable<Transition<T>> validTransitions = Transitions.Where(x => x.Identifier == charToCheckFor && x.FromState.Equals(state));

                foreach (Transition<T> transition in validTransitions)
                {
                    stringIsAccepted = CheckNextNodeDfa(transition.ToState, stringToVerify);
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
        private bool CheckNextNodeDfa(T state, string stringToVerify)
        {
            bool stringIsAccepted = false;

            if (stringToVerify == string.Empty) //Early escape if we're finished
            {
                stringIsAccepted = FinalStates.Contains(state);
                return stringIsAccepted;
            } 

            char currentCharacter = stringToVerify[0];

            if (stringToVerify.Count() > 1) { stringToVerify = stringToVerify.Substring(1); }
            else stringToVerify = string.Empty;

            IEnumerable<Transition<T>> validTransitions = Transitions.Where(x => x.Identifier == currentCharacter && x.FromState.Equals(state));
            
            foreach(Transition<T> transition in validTransitions)
            {
                stringIsAccepted = CheckNextNodeDfa(transition.ToState, stringToVerify);
                if (stringIsAccepted) { break; }
            }

            return stringIsAccepted;
        }

        /// <summary>
        /// Checks whether the given string fits within this automata
        /// </summary>
        /// <param name="stringToVerify">The string to check.</param>
        /// <returns>A boolean indicator that is true if the string can be formed by this automata by traversing it's nodes.</returns>
        private bool IsStringAcceptableNdfa(string stringToVerify)
        {
            bool stringIsAccepted = false;

            //Pak start states + eerste element in char array
            char charToCheckFor = stringToVerify[0];
            HashSet<T> startingStates = StartStates;

            //Verwijder eerste character
            if (stringToVerify.Count() > 1) { stringToVerify = stringToVerify.Substring(1); }
            else stringToVerify = string.Empty;

            //Voor iedere uitgaande transitie check of het kan met het gegeven character
            foreach (T state in startingStates)
            {
                if (stringToVerify == string.Empty)
                {
                    stringIsAccepted = FinalStates.Contains(state);
                    break;
                }

                IEnumerable<Transition<T>> validTransitions = Transitions.Where(x => (x.Identifier == charToCheckFor || x.Identifier == '$') && x.FromState.Equals(state));

                foreach (Transition<T> transition in validTransitions)
                {
                    if(transition.Identifier != '$') { stringIsAccepted = CheckNextNodeNdfa(transition, stringToVerify); }
                    else { stringIsAccepted = CheckNextNodeNdfa(transition, charToCheckFor + stringToVerify); }

                    if (stringIsAccepted) { break; }
                }
            }
            //Als er een terug komt met true, dan bestaat de string
            return stringIsAccepted;
        }

        /// <summary>
        /// Recursively checks to see if we can form the string
        /// </summary>
        /// <param name="givenTransition">the transition to check</param>
        /// <param name="remainingString">the remainder of the string to check</param>
        /// <returns>A boolean that is true if the string has been fully formed</returns>
        private bool CheckNextNodeNdfa(Transition<T> givenTransition, string stringToVerify)
        {
            bool stringIsAccepted = false;

            if (stringToVerify == string.Empty) //Early escape if we're finished
            {
                stringIsAccepted = FinalStates.Contains(givenTransition.ToState);
                if (stringIsAccepted) { return stringIsAccepted; }

                IEnumerable<Transition<T>> possibleTransitionsLeadingToEndState = Transitions.Where(x => x.Identifier == '$' && x.FromState.Equals(givenTransition.ToState));
                stringIsAccepted = CheckEmptyTransitions(possibleTransitionsLeadingToEndState);

                return stringIsAccepted;
            }

            char currentCharacter = stringToVerify[0];

            if (stringToVerify.Count() > 1) { stringToVerify = stringToVerify.Substring(1); }
            else stringToVerify = string.Empty;

            IEnumerable<Transition<T>> validTransitions = Transitions.Where(x => (x.Identifier == currentCharacter || x.Identifier == '$') && x.FromState.Equals(givenTransition.ToState));

            foreach (Transition<T> transition in validTransitions)
            {
                if (transition.Identifier != '$') { stringIsAccepted = CheckNextNodeNdfa(transition, stringToVerify); }
                else { stringIsAccepted = CheckNextNodeNdfa(transition, currentCharacter + stringToVerify); }

                if (stringIsAccepted) { break; }
            }
            return stringIsAccepted;
        }

        /// <summary>
        /// Checks if by traversing the given transitions, then following all other empty transitions.
        /// </summary>
        /// <param name="givenTransitions">The transitions to check.</param>
        /// <returns>A boolean that is true if we can traverse an empty path.</returns>
        private bool CheckEmptyTransitions(IEnumerable<Transition<T>> givenTransitions)
        {
            bool foundEmptyTransitionToFinalState = false;

            //Check if we can jump to the final state
            foreach(Transition<T> transition in givenTransitions)
            {
                if (FinalStates.Contains(transition.ToState)) { foundEmptyTransitionToFinalState = true; }
                if (foundEmptyTransitionToFinalState) { break; }
            }

            //If not, try the other states or return when we can't traverse any further.
            if(!foundEmptyTransitionToFinalState)
            {
                List<T> newFromStates = new List<T>();
                foreach(Transition<T> transition in givenTransitions) { newFromStates.Add(transition.ToState); }

                IEnumerable<Transition<T>> transitionsToCheck = Transitions.Where(x => x.Identifier == '$' && newFromStates.Contains(x.FromState));

                if(transitionsToCheck.Count() <= 0) { return foundEmptyTransitionToFinalState; }
                else { foundEmptyTransitionToFinalState = CheckEmptyTransitions(transitionsToCheck); }
            }

            return foundEmptyTransitionToFinalState;
        }

        /// <summary>
        /// Generates a language with the given length. Language might be a tad suspciously the same everytime due to the exclusion of rng
        /// </summary>
        /// <param name="length">The length of the string to generate</param>
        /// <returns>An error string if the string can't be generated and a word if there is a valid word with the given value</returns>
        public string GenerateLanguageOfGivenLength(int length)
        {
            string initialString = "";
            string errorString = "Problem encountered so no valid language could be generated.";

            if(length < GetMinimumLanguageLength()) { return errorString; } //Early return for when the given length is shorter than the lenght of the shortest word

            Random rng = new Random();
            T startState = StartStates.ElementAt(rng.Next(0, (StartStates.Count - 1))); //Substracting one to prevent out of bounds errors
            string result = AddLetterToLanguage(length, initialString, startState);

            result = String.IsNullOrEmpty(result) ? errorString : result;

            return result;
        }

        /// <summary>
        /// Adds a letter to a language, then returns the string
        /// </summary>
        /// <param name="remainingLength">The remaining length for the string</param>
        /// <param name="stringSoFar">The string generated so far.</param>
        /// <param name="state">The state from which to generate a letter.</param>
        /// <returns>Either returns a valid string, or an empty string if there is a non valid string.</returns>
        private string AddLetterToLanguage(int remainingLength, string stringSoFar, T state)
        {
            string resultingString = string.Empty;

            if(remainingLength > 0)
            {
                remainingLength = remainingLength - 1;

                IEnumerable<Transition<T>> validTransitions = Transitions.Where(x => x.FromState.Equals(state));

                foreach (Transition<T> transition in validTransitions)
                {
                    string newString = string.Concat(stringSoFar, transition.Identifier);

                    resultingString = AddLetterToLanguage(remainingLength, newString, transition.ToState);

                    if (string.IsNullOrEmpty(resultingString)) { continue; }
                    else { break; }
                }
            }
            else
            {
                resultingString = FinalStates.Contains(state) ? stringSoFar : string.Empty;
            }

            return resultingString;
        }

        /// <summary>
        /// Gets the minimum length of a language with this automaton.
        /// </summary>
        /// <returns>A integer that is equal to the mimimum length of the language.</returns>
        private int GetMinimumLanguageLength()
        {
            int length = 0;

            foreach(T state in StartStates)
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

            if (FinalStates.Contains(state))
            {
                return newCount;
            }
            else
            {
                IEnumerable<Transition<T>> transitionsForThisState = Transitions.Where(x => x.FromState.Equals(state) && !x.FromState.Equals(x.ToState));

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

        /// <summary>
        /// Comparer that checks if states are equal
        /// </summary>
        /// <param name="state1">The first state</param>
        /// <param name="state2">The second state</param>
        /// <returns>A boolean that is true if the states are equal</returns>
        private bool StatesAreEqual(T state1, T state2)
        {
            string[] splitStates1 = state1.ToString().Split('-');
            List<T> splitStates1T = new List<T>(); //List of all states contained in tis state
            foreach (string stateAsString in splitStates1)
            {
                T stateT = (T)Convert.ChangeType(stateAsString, typeof(T));
                splitStates1T.Add(stateT);
            }

            string[] splitStates2 = state2.ToString().Split('-');
            List<T> splitStates2T = new List<T>(); //List of all states contained in tis state
            foreach (string stateAsString in splitStates2)
            {
                T stateT = (T)Convert.ChangeType(stateAsString, typeof(T));
                splitStates2T.Add(stateT);
            }

            IEnumerable<T> splitStatesLeft = splitStates1T.Except(splitStates2T);

            if (splitStatesLeft.Count() <= 0) { return true; }//The states are equal
            else { return false; }//States aren't equal
        }

        /// <summary>
        /// Check to see if an dfa already contains a state.
        /// </summary>
        /// <param name="state">The state to search for.</param>
        /// <returns>A boolean that is true if the state has been found</returns>
        private bool DfaContainsState(T state)
        {
            bool dfaContainsState = false;

            foreach (T dfaState in States)
            {
                if (StatesAreEqual(dfaState, state))
                {
                    dfaContainsState = true;
                    break;
                }
            }

            return dfaContainsState;
        }
    }
}

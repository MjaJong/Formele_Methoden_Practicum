using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formele_Methoden_app
{
    //contains all operations that mutate one or multiple dfa with tupels into another dfa.
    public class DfaMutation<T> where T : IComparable
    {
        Dictionary<T, List<KeyValuePair<char, T>>> combinedTransitionsMap = new Dictionary<T, List<KeyValuePair<char, T>>>();
        List<T> startStatesForDfa = new List<T>();

        public DfaMutation()
        {

        }

        /// <summary>
        /// Combines two <see cref="Automata{T}"/> into a single automata if they are both dfa's
        /// </summary>
        /// <param name="firstDfa">The first dfa to use</param>
        /// <param name="secondDfa">The other dfa to use</param>
        /// <returns>A dfa that consists that takes the conditions for both dfa's into account</returns>
        public Automata<T> CombineAutomataAnd(Automata<T> firstDfa, Automata<T> secondDfa)
        {
            if (!firstDfa.IsDfa() || !secondDfa.IsDfa()) { throw new ArgumentException("One of the given automata is not a dfa"); }

            IEnumerable<char> combinedAlphabet = firstDfa.Symbols.Union(secondDfa.Symbols);
            Automata<T> combinedDfa = new Automata<T>(combinedAlphabet.ToArray());

            startStatesForDfa.Clear();
            combinedTransitionsMap.Clear();

            //Read the starting states
            SetStartStatesForAndOrOr(firstDfa, secondDfa);

            //Gather all states that follow
            MapReachableStatesForGivenDFA(firstDfa, secondDfa);

            //Generate the dfa, starting with the start states (no really sherlock)
            GenerateCombinedDfa(combinedDfa, startStatesForDfa);

            //Set start states
            foreach(T startState in startStatesForDfa) { combinedDfa.DefineAsStartState(startState); }

            //And end state, for the and it is a combination. So if is is a F and E, G then the endstates should contain F and either E or G.
            //So generate the end state by combining them in the same way as the start states, and then check if the dfa has the state.
            List<T> endStates = new List<T>();

            foreach (T firstEnd in firstDfa.FinalStates)
            {
                string firstAsString = firstEnd.ToString();
                foreach (T secondEnd in secondDfa.FinalStates)
                {
                    string secondAsString = secondEnd.ToString();
                    string newState = firstAsString + "-" + secondAsString;
                    T newStateT = (T)Convert.ChangeType(newState, typeof(T));
                    endStates.Add(newStateT);
                }
            }

            foreach(T endState in endStates)
            {
                foreach (T state in combinedDfa.States)
                {
                    if (state.Equals(endState)) { combinedDfa.DefineAsFinalState(endState); }
                }
            }
            
            return combinedDfa;
        }

        /// <summary>
        /// Combines two <see cref="Automata{T}"/> into a single automata if they are both dfa's
        /// </summary>
        /// <param name="firstDfa">The first dfa to use</param>
        /// <param name="secondDfa">The other dfa to use</param>
        /// <returns>A dfa that consists that takes the conditions for both dfa's into account</returns>
        public Automata<T> CombinaAutomataOr(Automata<T> firstDfa, Automata<T> secondDfa)
        {
            if (!firstDfa.IsDfa() || !secondDfa.IsDfa()) { throw new ArgumentException("One of the given automata is not a dfa"); }

            IEnumerable<char> combinedAlphabet = firstDfa.Symbols.Union(secondDfa.Symbols);
            Automata<T> combinedDfa = new Automata<T>(combinedAlphabet.ToArray());

            startStatesForDfa.Clear();
            combinedTransitionsMap.Clear();

            //Read the starting states
            SetStartStatesForAndOrOr(firstDfa, secondDfa);

            //Gather all states that follow
            MapReachableStatesForGivenDFA(firstDfa, secondDfa);

            //Generate the dfa, starting with the start states (no really sherlock)
            GenerateCombinedDfa(combinedDfa, startStatesForDfa);

            //Set start states
            foreach (T startState in startStatesForDfa) { combinedDfa.DefineAsStartState(startState); }

            //And end state, for the or it should contain one. So if is is a F and E, G then any state with a E, F or G is an end state.
            List<T> endStates = new List<T>();
            endStates.AddRange(firstDfa.FinalStates);
            foreach(T state in secondDfa.FinalStates)
            {
                foreach(T endState in endStates) { if (!StatesAreEqual(state, endState)) { endStates.Add(state); break; } }
            }

            foreach (T state in combinedDfa.States)
            {
                string[] splitStates = state.ToString().Split('-');
                List<T> separateStatesT = new List<T>();
                foreach(string splitState in splitStates) { separateStatesT.Add((T)Convert.ChangeType(splitState, typeof(T))); }

                //Now check if any of the seperates states are in endStates (so if intersect leaves atleast one element)
                int elementsLeft = separateStatesT.Intersect(endStates).ToList().Count;

                if(elementsLeft > 0) { combinedDfa.DefineAsFinalState(state); }
            }

            return combinedDfa;
        }

        /// <summary>
        /// Turns a dfa into a form in which the original is not accepted.
        /// </summary>
        /// <param name="originalDfa">The dfa to "invert"</param>
        /// <returns>A dfa that is !the original dfa</returns>
        public Automata<T> NotDfa(Automata<T> originalDfa)
        {
            if (!originalDfa.IsDfa()) { throw new ArgumentException("Given automata is not a DFA."); }

            Automata<T> notDfa = new Automata<T>(originalDfa.Symbols);
            //Alles wat geen end state is word een end, en omgekeerd.
            //Filter all end states from all the states
            IEnumerable<T> newEndStates = originalDfa.States.Except(originalDfa.FinalStates);

            //Set everthing in the new dfa
            foreach(Transition<T> transition in originalDfa.Transitions) { notDfa.AddTransition(transition); }
            foreach(T startState in originalDfa.StartStates) { notDfa.DefineAsStartState(startState); }
            foreach(T finalState in newEndStates) { notDfa.DefineAsFinalState(finalState); }

            return notDfa;
        }

        /// <summary>
        /// Turns a dfa into a form in which the original is not accepted.
        /// </summary>
        /// <param name="originalDfa">The dfa to make a reverse of</param>
        /// <returns>A dfa that is a reverse of the original dfa</returns>
        public Automata<T> ReverseDfa(Automata<T> originalDfa)
        {
            if (!originalDfa.IsDfa()) { throw new ArgumentException("Given automata is not a DFA."); }

            Automata<T> reversedDfa = new Automata<T>(originalDfa.Symbols);

            //Every transition is swapped as follows: new transition(original end state, symbol, original start state)
            foreach (Transition<T> transition in originalDfa.Transitions) { reversedDfa.AddTransition(new Transition<T>(transition.ToState, transition.FromState, transition.Identifier)); }
            //All start states become end states
            foreach(T startState in originalDfa.StartStates) { reversedDfa.DefineAsFinalState(startState); }
            //All end states become start states
            foreach (T endState in originalDfa.FinalStates) { reversedDfa.DefineAsStartState(endState); }

            return reversedDfa;
        }

        /// <summary>
        /// Sets up all start states for the combined dfa's
        /// </summary>
        /// <param name="firstDfa">The first dfa to use</param>
        /// <param name="secondDfa">The second dfa to use</param>
        private void SetStartStatesForAndOrOr(Automata<T> firstDfa, Automata<T> secondDfa)
        {
            List<T> startStates = new List<T>();

            foreach(T firstState in firstDfa.StartStates)
            {
                string firstAsString = firstState.ToString();
                foreach(T secondState in secondDfa.StartStates)
                {
                    string secondAsString = secondState.ToString();
                    string newState = firstAsString + "-" + secondAsString;
                    T newStateT = (T)Convert.ChangeType(newState, typeof(T));
                    startStates.Add(newStateT);
                }
            }

            startStatesForDfa = startStates;
        }

        /// <summary>
        /// Maps all reachable states by their from state. This is the mapping process where A-A leads to A-B and B-A for example.
        /// </summary>
        /// <param name="firstDfa">The first dfa to use</param>
        /// <param name="secondDfa">The second dfa to use</param>
        private void MapReachableStatesForGivenDFA(Automata<T> firstDfa, Automata<T> secondDfa)
        {
            Dictionary<T, List<KeyValuePair<char, T>>> transtionMap = new Dictionary<T, List<KeyValuePair<char, T>>>();
            IEnumerable<char> combinedSymbols = firstDfa.Symbols.Intersect(secondDfa.Symbols); //As we need to share the letters to actually go somewhere

            foreach (T firstState in firstDfa.States)
            {
                string firstAsString = firstState.ToString();
                foreach (T secondState in secondDfa.States)
                {
                    string secondAsString = secondState.ToString();
                    string fromState = firstAsString + "-" + secondAsString;
                    T fromStateT = (T)Convert.ChangeType(fromState, typeof(T));

                    List<KeyValuePair<char, T>> toStateBySymbol = new List<KeyValuePair<char, T>>();

                    foreach (char symbol in combinedSymbols)
                    {
                        //Both of these should be one transition due to one dfa, so using first is not a problem (unless it defaults of course...)
                        IEnumerable<Transition<T>> iFirstTransition = firstDfa.Transitions.Where(x => x.FromState.Equals(firstState) && x.Identifier == symbol);
                        IEnumerable<Transition<T>> iSecondTransition = secondDfa.Transitions.Where(x => x.FromState.Equals(secondState) && x.Identifier == symbol);

                        Transition<T> firstTransition = iFirstTransition.FirstOrDefault();
                        Transition<T> secondTransition = iSecondTransition.FirstOrDefault();

                        if(firstTransition == default(Transition<T>) || secondTransition == default(Transition<T>)) { throw new NullReferenceException("One of the transitions is it's default value."); }

                        string toState = firstTransition.ToState.ToString() + "-" + secondTransition.ToState.ToString();
                        T toStateT = (T)Convert.ChangeType(toState, typeof(T));

                        toStateBySymbol.Add(new KeyValuePair<char, T>(symbol, toStateT));
                    }

                    transtionMap.Add(fromStateT, toStateBySymbol);
                }
            }

            combinedTransitionsMap = transtionMap;
        }

        /// <summary>
        /// Traverses the <see cref="Dictionary{TKey, TValue}"/> to generate a dfa. This recursively happen untill there are no more states to traverse.
        /// </summary>
        /// <param name="combinedDfa">The combined dfa, modified and passed every loop.</param>
        /// <param name="statesToTraverse">The next states to traverse.</param>
        private void GenerateCombinedDfa(Automata<T> combinedDfa, List<T> statesToTraverse)
        {
            List<T> statesToTraverseNext = new List<T>();
            //Add all the new transitions to the combined dfa.
            foreach (T state in statesToTraverse)
            {
                List<KeyValuePair<char, T>> transitionsFromThisState = combinedTransitionsMap[state];
                
                foreach(KeyValuePair<char, T> symbolAndToState in transitionsFromThisState)
                {
                    if (DfaContainsStateCombinedDfa(combinedDfa, symbolAndToState.Value))
                    {
                        combinedDfa.AddTransition(new Transition<T>(state, symbolAndToState.Value, symbolAndToState.Key));
                        continue;
                    } //Skip this state if it is already in the dfa. But do add a transition

                    if (state.Equals(symbolAndToState.Value))
                    {
                        combinedDfa.AddTransition(new Transition<T>(state, symbolAndToState.Key));
                    }
                    else
                    {
                        combinedDfa.AddTransition(new Transition<T>(state, symbolAndToState.Value, symbolAndToState.Key));
                        statesToTraverseNext.Add(symbolAndToState.Value);
                    }
                }
            }

            //And a zero means that we have reached the end.
            if(statesToTraverseNext.Count <= 0) { return; }
            else { GenerateCombinedDfa(combinedDfa, statesToTraverseNext); }
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
        /// <param name="dfa">The dfa to check.</param>
        /// <param name="state">The state to search for.</param>
        /// <returns>A boolean that is true if the state has been found</returns>
        private bool DfaContainsStateCombinedDfa(Automata<T> dfa, T state)
        {
            bool dfaContainsState = false;

            foreach(T dfaState in dfa.States)
            {
                if (dfaState.Equals(state))
                {
                    dfaContainsState = true;
                    break;
                }
            }

            return dfaContainsState;
        }
    }
}

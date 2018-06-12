using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formele_Methoden_app
{
    public class NdfaToDfa<T> where T : IComparable
    {
        private Automata<T> ndfa;
        private Automata<T> dfa;
        Dictionary<T, List<KeyValuePair<char, HashSet<T>>>> reachableStates;
        private T emptyStateT = (T)Convert.ChangeType("$", typeof(T));

        public NdfaToDfa()
        {

        }

        /// <summary>
        /// Turns the given <see cref="Automata{T}"/> ndfa into it's dfa
        /// </summary>
        /// <param name="ndfaToTransform">the <see cref="Automata{T}"/> representing the ndfa</param>
        /// <returns>The <see cref="Automata{T}"/> dfa that can be build from the given ndfa</returns>
        public Automata<T> TransformNdfaIntoDfa(Automata<T> ndfaToTransform)
        {
            if (ndfaToTransform.IsDfa()) { Console.WriteLine("Given ndfa is actually a dfa."); return ndfaToTransform; }

            ndfa = ndfaToTransform;

            //Determine the states to go to with a and b, including empty transitions.
            reachableStates = GetAllReachableStatesByFromState();

            //Build the automata
            GenerateDFA();

            //See if we actually have a DFA
            if (!dfa.IsDfa()) { Console.WriteLine("Congrats, it isn't a DFA..."); return new Automata<T>(); }

            return dfa;
        }

        /// <summary>
        /// Creates a dictionary that contains a key which is the state to go from and a list of key value pairs, with the key being the identifier, and the value being
        /// a list of states that can be reached by folloing said identifier or a empty transition.
        /// </summary>
        /// <returns>A <see cref="Dictionary<T, List<KeyValuePair<char, HashSet<T>>>>"/> which contains all reachable states.</returns>
        private Dictionary<T, List<KeyValuePair<char, HashSet<T>>>> GetAllReachableStatesByFromState()
        {
            Dictionary<T, List<KeyValuePair<char, HashSet<T>>>> reachableStatesByFromState = new Dictionary<T, List<KeyValuePair<char, HashSet<T>>>>();

            //Get all transitons with this state
            foreach(T state in ndfa.States)
            {
                List<KeyValuePair<char, HashSet<T>>> reachableStatesBySymbol = new List<KeyValuePair<char, HashSet<T>>>();

                //Sorted by the symbol used
                foreach (char symbol in ndfa.Symbols)
                {
                    HashSet<T> reachableStates = new HashSet<T>();
                    foreach(Transition<T> transition in ndfa.Transitions)
                    {
                        if(transition.FromState.Equals(state) && (transition.Identifier == symbol || transition.Identifier == '$')) { reachableStates.Add(transition.ToState); }
                    }

                    reachableStatesBySymbol.Add(new KeyValuePair<char, HashSet<T>>(symbol, reachableStates));
                }

                reachableStatesByFromState.Add(state, reachableStatesBySymbol);
            }

            return reachableStatesByFromState;
        }

        /// <summary>
        /// Generates the dfa for the given dictionary of states
        /// </summary>
        private void GenerateDFA()
        {
            dfa = new Automata<T>();
            //Start building the DFA as normal, with the state names being the combined states to be reached in one string, with a - as delimiter.
                //Starting with the start state, foreach symbol create two string builder objects, build up the string in the state + - format
                //Create a transition with the symbol and the states.
                //Should we reach an empty collection, create the $ state, and for each symbol in the alphabet make a transition to this state

            //Begin with the start state and all transitions
            List<Transition<T>> dfaStart = GenerateDfaStart();
            //Keep track of states that we still need to visit
            HashSet<T> statesToTraverse = new HashSet<T>();

            foreach(Transition<T> transition in dfaStart)
            {
                dfa.AddTransition(transition);
                statesToTraverse.Add(transition.ToState);
            }

            //Sets all the transitions for the following states
            GenerateOtherTransitions(statesToTraverse);

            ////Removed duplicates that have been generated, which happens
            //RemoveDuplicatesFromDfa(); Keeping this in in case of fuckups down the line

            //After that, for all states check if it's a start state or end state.
            MarkStatesAsStartOrEnd();
        }

        /// <summary>
        /// Gets the start of the dfa.
        /// </summary>
        /// <returns>A <see cref="List{Transition{T}}"/> where the from state is the start state.</returns>
        private List<Transition<T>> GenerateDfaStart()
        {
            List<Transition<T>> transitions = new List<Transition<T>>();

            foreach(char symbol in ndfa.Symbols)
            {
                HashSet<T> reachableStatesWithThisSymbol = new HashSet<T>();

                //Build our from state and get all reachabe states with this symbol
                StringBuilder fromState = new StringBuilder();
                foreach (T state in ndfa.StartStates)
                {
                    //Append to the string
                    fromState.Append(state.ToString() + '-');
                    //And remove last - TODO debug this
                    fromState.Remove(fromState.Length - 1, 1);

                    //Filter this states transitions by the symbol
                    IEnumerable<KeyValuePair<char, HashSet<T>>> keyValuePairs = reachableStates[state].Where(x => x.Key == symbol);
                    foreach(KeyValuePair<char, HashSet<T>> pair in keyValuePairs)
                    {
                        foreach(T reachableState in pair.Value) { reachableStatesWithThisSymbol.Add(reachableState); } //And add every reachable state to the set
                    }
                }

                StringBuilder toState = new StringBuilder();
                if(reachableStatesWithThisSymbol.Count == 0) { toState.Append("$"); }//Create the empty to state
                else
                {
                    foreach (T state in reachableStatesWithThisSymbol)
                    {
                        //Append to the string
                        toState.Append(state.ToString() + '-');
                        //And remove last - TODO debug this
                        toState.Remove(toState.Length - 1, 1);
                    }
                }
                
                //Ho boy, do not pay much attention to this atrocity
                T fromStateT = (T)Convert.ChangeType(fromState.ToString(), typeof(T));
                T toStateT = (T)Convert.ChangeType(toState.ToString(), typeof(T));

                transitions.Add(new Transition<T>(fromStateT, toStateT, symbol));
            }

            return transitions;
        }

        /// <summary>
        /// Uses the given dictionary and the given list of states to get more transitions
        /// </summary>
        /// <param name="statesToLoopThrough">The states to loop through</param>
        /// <returns></returns>
        private List<Transition<T>> GenerateOtherTransitions(HashSet<T> statesToLoopThrough)
        {
            List<Transition<T>> transitions = new List<Transition<T>>();
            HashSet<T> statesToTraverseNext = new HashSet<T>();
            
            foreach (T currentStateLoopingThrough in statesToLoopThrough)
            {
                if(currentStateLoopingThrough.Equals(emptyStateT))
                {
                    foreach(char symbol in ndfa.Symbols)
                    {
                        transitions.Add(new Transition<T>(emptyStateT, symbol));
                    }
                    continue;
                }

                //For the state (which may be a composite) get all single states
                string[] splitStates = currentStateLoopingThrough.ToString().Split('-');
                List<T> splitStatesT = new List<T>(); //List of all states contained in tis state
                foreach (string stateAsString in splitStates)
                {
                    T stateT = (T)Convert.ChangeType(stateAsString, typeof(T));
                    splitStatesT.Add(stateT);
                }

                foreach (char symbol in ndfa.Symbols)
                {
                    HashSet<T> reachableStatesWithThisSymbol = new HashSet<T>();
                    foreach (T stateToLookFor in splitStatesT)
                    {
                        //Then gather all states this can go to
                        HashSet<T> statesReachedByGivenState = reachableStates[stateToLookFor].Find(x => x.Key == symbol).Value; //Find the states reached with this state-symbol combination
                        foreach(T foundState in statesReachedByGivenState) { reachableStatesWithThisSymbol.Add(foundState); }
                    }

                    //For which the collection forms a single to state
                    StringBuilder toState = new StringBuilder();
                    if (reachableStatesWithThisSymbol.Count == 0) { toState.Append("$"); }//Create the empty to state
                    else
                    {
                        foreach (T reachableState in reachableStatesWithThisSymbol)
                        {
                            //Append to the string
                            toState.Append(reachableState.ToString() + '-');
                        }

                        //And remove last - TODO debug this
                        toState.Remove(toState.Length - 1, 1);
                    }

                    T toStateT = (T)Convert.ChangeType(toState.ToString(), typeof(T));

                    if (StatesAreEqual(currentStateLoopingThrough, toStateT))
                    {
                        //Should the toState and the current state be equal, we can point to ourself
                        transitions.Add(new Transition<T>(currentStateLoopingThrough, symbol));
                    }
                    else
                    {
                        //Which then gives us the transition of: 
                        //state as the from state,
                        //The collection as the to state
                        //And the symbol as the identifier
                        statesToTraverseNext.Add(toStateT);
                        transitions.Add(new Transition<T>(currentStateLoopingThrough, toStateT, symbol));
                    }
                }
            }

            foreach(Transition<T> transition in transitions) { dfa.AddTransition(transition); }

            //Check if there still are states left to traverse, which we haven't traversed yet
            HashSet<T> statesAlreadyTraversed = new HashSet<T>();
            foreach(Transition<T> transition in dfa.Transitions) { statesAlreadyTraversed.Add(transition.FromState); }

            //Quick filter all states that happen to be the exact same (for example, C-D-E might be in the to traverse and the already traversed as the same string)
            statesToTraverseNext = new HashSet<T>(statesToTraverseNext.Except(statesAlreadyTraversed));

            if (statesToTraverseNext.Count <= 0) { return transitions; }
            else
            {
                List<Transition<T>> deeperTransitions = GenerateOtherTransitions(statesToTraverseNext);
                return transitions.Union(deeperTransitions).ToList();
            }
        }

        /// <summary>
        /// Removes all the duplicate states from the dfa, and the associated transitions
        /// </summary>
        private void RemoveDuplicatesFromDfa()
        {
            List<T> duplicateStates = new List<T>();
            List<T> statesSeenInCheck = new List<T>();

            foreach (T state in dfa.States)//Check every state left after the first check
            {
                bool stateIsDuplicate = false;
                foreach (T stateSeen in statesSeenInCheck)//With all states that we have seen
                {
                    if (StatesAreEqual(state, stateSeen))
                    {
                        stateIsDuplicate = true;
                        break;
                    }
                }

                if (stateIsDuplicate) { duplicateStates.Add(state); }//If it is a dupe, add to the list to remove
                else { statesSeenInCheck.Add(state); }//Otherwise, add it to the seen states, as it is the first of it's kind
            }

            foreach(T duplicateState in duplicateStates)
            {
                dfa.States.Remove(duplicateState);
                dfa.Transitions.RemoveWhere(x => x.FromState.Equals(duplicateState));
                List<Transition<T>> transitionsToRemove = new List<Transition<T>>();

                //foreach(Transition<T> transition in dfa.Transitions)
                //{
                //    if (transition.FromState.Equals(duplicateState)) { transitionsToRemove.Add(transition); }
                //}

                ////Right comparing two transitions goes infinite due to Equals(((Transition<T>)other)) && Identifier == (((Transition<T>)other).Identifier);
                //foreach (Transition<T> transitionToRemove in transitionsToRemove) { dfa.Transitions.Remove(transitionToRemove); }
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

            if(splitStatesLeft.Count() <= 0) { return true; }//The states are equal
            else { return false; }//States aren't equal
        }

        /// <summary>
        /// Marks all states that contain a start or end state as the respective state in the dfa
        /// </summary>
        private void MarkStatesAsStartOrEnd()
        {
            foreach(T state in dfa.States)
            {
                string[] splitStates = state.ToString().Split('-');
                List<T> splitStatesT = new List<T>(); //List of all states contained in tis state
                foreach (string stateAsString in splitStates)
                {
                    T stateT = (T)Convert.ChangeType(stateAsString, typeof(T));
                    splitStatesT.Add(stateT);
                }

                foreach(T stateT in splitStatesT) //Do this for each state, should a composite have both the ndfa start state and end state
                {
                    bool isStartState = false;
                    bool isFinalState = false;

                    isStartState = ndfa.StartStates.Contains(stateT);
                    isFinalState = ndfa.FinalStates.Contains(stateT);

                    if (isStartState) { dfa.StartStates.Add(state); }
                    if (isFinalState) { dfa.FinalStates.Add(state); }
                }
            }
        }
    }
}

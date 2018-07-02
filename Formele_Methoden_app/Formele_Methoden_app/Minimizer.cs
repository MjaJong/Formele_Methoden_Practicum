using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formele_Methoden_app
{
    class Minimizer<T> where T : IComparable
    {
        private DfaMutation<T> mutator = new DfaMutation<T>();
        private NdfaToDfa<T> transformer = new NdfaToDfa<T>();

        public Minimizer()
        {

        }

        /// <summary>
        /// Minimize a dfa by using Brozozowski's algorithm
        /// </summary>
        /// <param name="dfaToMinimize">The dfa to minimize.</param>
        /// <returns>The minimal dfa.</returns>
        public Automata<T> MinimizeUsingBrzozowski(Automata<T> dfaToMinimize)
        {
            Automata<T> reversedDfa = mutator.ReverseDfa(dfaToMinimize);
            Automata<T> remappedDfa = RemapStates(reversedDfa);
            Automata<T> firstPassDfa = transformer.TransformNdfaIntoDfa(remappedDfa);

            Automata<T> reversedFirstPass = mutator.ReverseDfa(firstPassDfa);
            Automata<T> remappedPass = RemapStates(reversedFirstPass);
            Automata<T> minimalDfa = transformer.TransformNdfaIntoDfa(remappedPass);

            return minimalDfa;
        }

        /// <summary>
        /// A bloody lazy method to fix something i really don't have the patience to fix right now. This makes sure only a single - will end up between states.
        /// </summary>
        /// <param name="dfaToRemap">The dfa to remap the states for</param>
        /// <returns></returns>
        private Automata<T> RemapStates(Automata<T> dfaToRemap)
        {
            Dictionary<T, T> stateMap = new Dictionary<T, T>();

            for(int i = 0; i < dfaToRemap.States.Count; i++)
            {
                string newState = "q" + i;
                T newStateT = (T)Convert.ChangeType(newState, typeof(T));
                stateMap.Add(dfaToRemap.States.ElementAt(i), newStateT);
            }

            Console.WriteLine("Now printing the remap dictionary:");
            foreach(KeyValuePair<T, T> map in stateMap) { Console.WriteLine("Mapped {0} as {1}", map.Key, map.Value); }
            Console.WriteLine("-----------------------------------------");

            Automata<T> remappedDfa = new Automata<T>(dfaToRemap.Symbols);

            foreach(Transition<T> transition in dfaToRemap.Transitions) { remappedDfa.AddTransition(new Transition<T>(stateMap[transition.FromState], stateMap[transition.ToState], transition.Identifier)); }
            foreach(T startState in dfaToRemap.StartStates) { remappedDfa.DefineAsStartState(stateMap[startState]); }
            foreach(T finalState in dfaToRemap.FinalStates) { remappedDfa.DefineAsFinalState(stateMap[finalState]); }

            return remappedDfa;
        }
    }
}

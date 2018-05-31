using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formele_Methoden_app
{
    /// <summary>
    /// Turns a Regex into a <see cref="Automata{string}"/>.
    /// </summary>
    public class ThompsonConstruction
    {
        struct ThompsonPart
        {
            public HashSet<Transition<string>> transitions;
            public HashSet<string> states;

            public string startState;
            public string finalState;
        }

        /// <summary>
        /// 
        /// </summary>
        public ThompsonConstruction()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Automata<string> GenerateNDFA(RegExp expressionToTranslate)
        {


            return null;
        }

        /// <summary>
        /// Turns a given string into a part of the NDFA
        /// </summary>
        /// <param name="regexAsString">The string to translate. This can be the whole <see cref="RegExp"/> or a sub string</param>
        /// <returns>A single <see cref="ThompsonPart"/> containing all data.</returns>
        private ThompsonPart GenerateThompsonPart(string regexAsString)
        {
            ThompsonPart generatedPart = new ThompsonPart();

            //Apply magic here, prolly a search algorithm for the most relevant char + switch case

            return generatedPart;
        }

        /// <summary>
        /// Translates a single symbol to a part of an NDFA
        /// </summary>
        /// <param name="symbol">The symbol to translate.</param>
        /// <returns>A <see cref="ThompsonPart"/> that has all states and transitions.</returns>
        private ThompsonPart GenerateSingleSymbol(char symbol)
        {
            ThompsonPart newPart = new ThompsonPart();

            Transition<string> transition = new Transition<string>("q0", "q1", symbol);
            newPart.transitions.Add(transition);

            newPart.states.Add(transition.FromState);
            newPart.startState = transition.FromState;

            newPart.states.Add(transition.ToState);
            newPart.finalState = transition.ToState;

            return newPart;
        }

        /// <summary>
        /// Generates an empty pass in the NDFA
        /// </summary>
        /// <returns>A <see cref="ThompsonPart"/> that has all states and transitions.</returns>
        private ThompsonPart GenerateEmptyTransition()
        {
            ThompsonPart newPart = new ThompsonPart();

            Transition<string> transition = new Transition<string>("q0", "q1");
            newPart.transitions.Add(transition);

            newPart.states.Add(transition.FromState);
            newPart.startState = transition.FromState;

            newPart.states.Add(transition.ToState);
            newPart.finalState = transition.ToState;

            return newPart;
        }

        /// <summary>
        /// Turns two regex substring into the dot operator equivalent NDFA part.
        /// </summary>
        /// <param name="leftSubstring">The substring that should be between state q0 and q1.</param>
        /// <param name="rightSubstring">The substring that should be between state q2 and q3.</param>
        /// <returns>A <see cref="ThompsonPart"/> that has all states and transitions.</returns>
        private ThompsonPart GenerateDotOperatorConstruction(string leftSubstring, string rightSubstring)
        {
            ThompsonPart newPart = new ThompsonPart();
            ThompsonPart leftPart = GenerateThompsonPart(leftSubstring);
            ThompsonPart rightPart = GenerateThompsonPart(rightSubstring);

            #region State consolidation
            //Consolidate states here so that all states are unique (this is not useable by multiple constructions, since those differ in contained elements
            int leftStatesCount = leftPart.states.Count;
            int leftStatesCountMinusOne = leftStatesCount - 1;
            int rightStatesCount = rightPart.states.Count;
            int rightStatesCountMinusOne = rightStatesCount - 1;

            leftPart.startState = "q0";                                                                                                 //Left part start state = q0
            leftPart.finalState = "q" + leftStatesCountMinusOne;                                                                        //Left part final state = all left states count - 1
            leftPart.states = new HashSet<string> { leftPart.startState, leftPart.finalState};
            for (int i = 1; i < leftStatesCountMinusOne; i++) { leftPart.states.Add("q" + i); }                                         //States in between: for(int i = 1, i < count - 1, i++)

            if(leftPart.states.Count != leftStatesCount) { throw new Exception("Mismatch in list sizes of left part"); }

            rightPart.startState = "q" + leftStatesCount;                                                                               //Right part start state = all left states count
            rightPart.finalState = "q" + (leftStatesCount + rightStatesCountMinusOne);                                                  //Right part final state = left state count + all right states count - Read also as 6 elements + 6 elements to get the last position, which zero indexed is 11
            leftPart.states = new HashSet<string> { rightPart.startState, rightPart.finalState };
            for (int i = 1 + leftStatesCount; i < (rightStatesCountMinusOne + leftStatesCount); i++) { rightPart.states.Add("q" + 1); } //States in between: for(int i = 1, i < count - 1, i++)

            if (rightPart.states.Count != rightStatesCount) { throw new Exception("Mismatch in list sizes of right part"); }
            #endregion

            //After consolidation, use data to set values
            Transition<string> transition = new Transition<string>(leftPart.finalState, rightPart.startState);                          //Epsilon transition between the two parts.

            newPart.transitions.Add(transition);
            IEnumerable<Transition<string>> containedTransitions = leftPart.transitions.Concat(rightPart.transitions);
            newPart.transitions.Concat(containedTransitions);

            newPart.startState = leftPart.startState;
            newPart.finalState = rightPart.finalState;

            IEnumerable<string> containedStates = leftPart.states.Concat(rightPart.states);
            newPart.states = new HashSet<string>(containedStates); 

            return newPart;
        }

        /// <summary>
        /// Turns two regex substring into the or operator equivalent NDFA part.
        /// </summary>
        /// <param name="leftSubstring">The substring that should be between state q2 and q3.</param>
        /// <param name="rightSubstring">The substring that should be between state q4 and q5.</param>
        /// <returns>A <see cref="ThompsonPart"/> that has all states and transitions.</returns>
        private ThompsonPart GenerateOrOperatorConstruction(string leftSubstring, string rightSubstring)
        {
            ThompsonPart newPart = new ThompsonPart();
            ThompsonPart leftPart = GenerateThompsonPart(leftSubstring);
            ThompsonPart rightPart = GenerateThompsonPart(rightSubstring);

            #region State consolidation
            //Consolidate states here so that all states are unique (this is not useable by multiple constructions, since those differ in contained elements
            int leftStatesCount = leftPart.states.Count;
            int leftStatesCountPlusOne = leftStatesCount + 1;
            int rightStatesCount = rightPart.states.Count;

            leftPart.startState = "q1";//Left part start state = q1, q0 is our start state for the or
            leftPart.finalState = "q" + (leftStatesCount + 1);//Left part final state = all left states count, due to being shifted one over
            leftPart.states = new HashSet<string> { leftPart.startState, leftPart.finalState };
            for (int i = 2; i < leftStatesCountPlusOne; i++) { }//States in between: for(int i = 1, i < count - 1, i++)

            //Right part start state = all left states count
            //Right part final state =  all right states count - 1
            //States in between: for(int i = 1, i < count - 1, i++)
            #endregion

            ////After consolidation, use data to set values
            //newPart.startState = "q0";
            //newPart.finalState = "q1";

            //newPart.states = new HashSet<string> { newPart.startState, newPart.finalState};

            //newPart.transitions = new HashSet<Transition<string>>
            //{
            //    new Transition<string>("q0", leftPart.startState),
            //    new Transition<string>("q0", rightPart.startState),
            //    new Transition<string>(leftPart.finalState, "");
            //};

            return newPart;
        }


        private ThompsonPart GeneratelusOperatorConstruction(string leftSubstring, string rightSubstring)
        {
            ThompsonPart newPart = new ThompsonPart();

            //Apply magic here

            //Consolidate states here so that all states are unique (this is not useable by multiple constructions, since those differ in contained elements

            //Left part start state = q0
            //Left part final state = all left states count - 1
            //States in between: for(int i = 1, i < count - 1, i++)

            //Right part start state = all left states count
            //Right part final state =  all right states count - 1
            //States in between: for(int i = 1, i < count - 1, i++)

            return newPart;
        }


        private ThompsonPart GenerateAsterixOperatorConstruction(string leftSubstring, string rightSubstring)
        {
            ThompsonPart newPart = new ThompsonPart();

            //Apply magic here

            //Consolidate states here so that all states are unique (this is not useable by multiple constructions, since those differ in contained elements

            //Left part start state = q0
            //Left part final state = all left states count - 1
            //States in between: for(int i = 1, i < count - 1, i++)

            //Right part start state = all left states count
            //Right part final state =  all right states count - 1
            //States in between: for(int i = 1, i < count - 1, i++)

            return newPart;
        }
    }
}

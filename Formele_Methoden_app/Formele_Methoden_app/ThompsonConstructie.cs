using System;
using System.Collections.Generic;
using System.Linq;

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

        public ThompsonConstruction()
        {

        }

        /// <summary>
        /// Turns a <see cref="RegExp"/> into a <see cref="Automata{string}"/>, which is a NDFA.
        /// </summary>
        /// <param name="expressionToTranslate">The <see cref="RegExp"/> to translate.</param>
        /// <returns>A <see cref="Automata{string}"/>, which is a NDFA or null if there was a problem</returns>
        public Automata<string> GenerateNDFA(RegExp expressionToTranslate)
        {
            HashSet<char> alphabet = new HashSet<char>();
            String regexAsString = expressionToTranslate.ToString();

            foreach(char c in regexAsString) { if (IsUsableCharacter(c)) { alphabet.Add(c); } };

            ThompsonPart completeNdfaAsThompson = GenerateThompsonPart(regexAsString); 

            if(completeNdfaAsThompson.Equals(new ThompsonPart())) { return null; }

            Automata<string> NDFA = new Automata<string>(alphabet.ToArray());
            
            foreach(Transition<string> thompsonTransition in completeNdfaAsThompson.transitions) { NDFA.AddTransition(thompsonTransition); }
            NDFA.DefineAsStartState(completeNdfaAsThompson.startState);
            NDFA.DefineAsFinalState(completeNdfaAsThompson.finalState);

            return NDFA;
        }

        /// <summary>
        /// Turns a given string into a part of the NDFA
        /// </summary>
        /// <param name="regexAsString">The string to translate. This can be the whole <see cref="RegExp"/> or a sub string</param>
        /// <returns>A single <see cref="ThompsonPart"/> containing all data or an empty thompson construction if the regex can't be parsed.</returns>
        private ThompsonPart GenerateThompsonPart(string regexAsString)
        {
            if (!CheckForEvenBrackets(regexAsString)) { return new ThompsonPart(); }

            ThompsonPart generatedPart = new ThompsonPart();

            int bracketCount = 0;
            int openingBracketPosition = -1;
            int closingBracketPosition = -1;
            char regexOperator = '$';

            foreach(Char c in regexAsString) { if (c == '(' || c == ')') bracketCount++; } //Count all brackets, important for later.

            //Iterate over string, check for brackets
            for (int i = 0; i < regexAsString.Length; i++)
            {
                //Special character checks
                if (regexAsString[i] == '(' && openingBracketPosition == -1) { openingBracketPosition = i; }
                if (regexAsString[i] == ')' && closingBracketPosition == -1) { closingBracketPosition = i; }
                if (regexAsString[i] == '.' && regexOperator == '$') { regexOperator = '.'; }
                if (regexAsString[i] == '|' && regexOperator == '$') { regexOperator = '|'; }
                if (regexAsString[i] == '+' && regexOperator == '$') { regexOperator = '+'; }
                if (regexAsString[i] == '*' && regexOperator == '$') { regexOperator = '*'; }

                if(bracketCount != 0) //Thus we have brackets in our regex
                {
                    //Check to see if we should break, or that we should continue (should one of the operators be contained within brackets)
                    if (regexOperator != '$' && (openingBracketPosition == -1 || closingBracketPosition == -1))//This means that we have a contained operator
                    {
                        regexOperator = '$';
                    }
                    else
                    {
                        generatedPart = TranslateOperator(openingBracketPosition, closingBracketPosition, regexAsString, regexOperator);
                    } //Translates the operator that has been found
                }
                else
                {
                    //Translates the operator that has been found, with opening and closing positions being -1
                    if (regexOperator != '$')
                    {
                        //TODO split string according (take aa*b as example) with the Translate operator transition
                    }
                    //We've found no brackets and operators, thus we can turn the characters into transitions
                    else
                    {
                        foreach(Char c in regexAsString)
                        {
                            generatedPart =  GenerateMultipleSymbolTransition(regexAsString);
                        }
                    } 
                }
            }
            return generatedPart;
        }

        /// <summary>
        /// Checks if the given regex can be parsed.
        /// </summary>
        /// <param name="regex">The regex to check.</param>
        /// <returns>A boolean indicator that is true if the amount of brackets is even, otherwise false.</returns>
        private bool CheckForEvenBrackets(string regex)
        {
            int bracketBalance = 0;

            foreach(Char c in regex)
            {
                if(c == '(') { bracketBalance++; }
                else if (c == ')') { bracketBalance--; }
            }

            return bracketBalance == 0 ? true : false;
        }

        /// <summary>
        /// Translates a operator to a thompson part.
        /// </summary>
        /// <param name="openingBracketPosition">Position of the opening bracket.</param>
        /// <param name="closingBracketPosition">Position of the closing bracket.</param>
        /// <param name="regex">The regex to translate.</param>
        /// <param name="regexOperator">The operator found</param>
        /// <returns>A <see cref="ThompsonPart"/> that contains all data.</returns>
        private ThompsonPart TranslateOperator(int openingBracketPosition, int closingBracketPosition, string regex, char regexOperator)
        {
            //Build in check to translate character without brackets
            //Found special substring, create a fitting thompson part. String between brackets is left, everthing else is right
            int leftLength = (closingBracketPosition) - (openingBracketPosition + 1);
            string leftExpression = regex.Substring(openingBracketPosition + 1, leftLength);
            string rightExpression = regex.Substring(closingBracketPosition + 1);

            ThompsonPart generatedPart = new ThompsonPart();

            //And generate something for the operator. Note that the first to split the string, while the second two do not, so the second two take the whole string
            switch (regexOperator)
            {
                case '.':
                    generatedPart = GenerateDotOperatorConstruction(leftExpression, rightExpression);
                    break;
                case '|':
                    generatedPart = GenerateOrOperatorConstruction(leftExpression, rightExpression);
                    break;
                case '+':
                    generatedPart = GeneratePlusOperatorConstruction(regex);
                    break;
                case '*':
                    generatedPart = GenerateAsterixOperatorConstruction(regex);
                    break;
            }

            return generatedPart;
        }

        /// <summary>
        /// Turns multiple characters into transitions.
        /// </summary>
        /// <param name="symbols">The characters to translate</param>
        /// <returns>A <see cref="ThompsonPart"/> that has all states and transitions.</returns>
        private ThompsonPart GenerateMultipleSymbolTransition(string symbols)
        {
            ThompsonPart generatedPart = new ThompsonPart();

            foreach(char c in symbols)
            {
                if(generatedPart.states.DefaultIfEmpty() == null)
                {
                    generatedPart = GenerateSingleSymbolTransition(c);
                }
                else //Create an extra part and concat both transitions and the states. The new end state is the latest end state, thus the endstate of the extra part.
                {
                    ThompsonPart extraPart = GenerateSingleSymbolTransition(c);

                    generatedPart.states.Concat(extraPart.states);
                    generatedPart.transitions.Concat(extraPart.transitions);
                    generatedPart.finalState = extraPart.finalState;
                }
            }

            return generatedPart;
        }

        /// <summary>
        /// Translates a single symbol to a part of an NDFA.
        /// </summary>
        /// <param name="symbol">The symbol to translate.</param>
        /// <returns>A <see cref="ThompsonPart"/> that has all states and transitions.</returns>
        private ThompsonPart GenerateSingleSymbolTransition(char symbol)
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
            rightPart.states = new HashSet<string> { rightPart.startState, rightPart.finalState };
            for (int i = 1 + leftStatesCount; i < (rightStatesCountMinusOne + leftStatesCount); i++) { rightPart.states.Add("q" + i); } //States in between: for(int i = 1, i < count - 1, i++)

            if (rightPart.states.Count != rightStatesCount) { throw new Exception("Mismatch in list sizes of right part"); }
            #endregion

            //After consolidation, use data to set values
            Transition<string> transition = new Transition<string>(leftPart.finalState, rightPart.startState);                          //Epsilon transition between the two parts.

            newPart.transitions.Add(transition);
            IEnumerable<Transition<string>> containedTransitions = leftPart.transitions.Concat(rightPart.transitions);
            IEnumerable<Transition<string>> allTransistions =  newPart.transitions.Concat(containedTransitions);
            newPart.transitions = new HashSet<Transition<string>>(allTransistions);

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

            leftPart.startState = "q2";                                                                                                         //Left part start state = q2, q0 is our start state for the or and q1 the end state
            leftPart.finalState = "q" + leftStatesCountPlusOne;                                                                                 //Left part final state = all left states count, due to being shifted one over
            leftPart.states = new HashSet<string> { leftPart.startState, leftPart.finalState };
            for (int i = 2; i < leftStatesCountPlusOne; i++) { leftPart.states.Add("q" + i); }                                                  //States in between: for(int i = 1, i < count - 1, i++)

            if (leftPart.states.Count != leftStatesCount) { throw new Exception("Mismatch in list sizes of left part"); }

            rightPart.startState = "q" + (leftStatesCountPlusOne + 1);                                                                          //Right part start state = all left states count
            rightPart.finalState = "q" + (leftStatesCountPlusOne + rightStatesCount);                                                           //Right part final state =  all right states count - 1
            rightPart.states = new HashSet<string> { rightPart.startState, rightPart.finalState };
            for (int i = (leftStatesCountPlusOne + 2); i < (leftStatesCountPlusOne + rightStatesCount); i++) { rightPart.states.Add("q" + i); } //States in between: for(int i = 1, i < count - 1, i++)

            if (rightPart.states.Count != rightStatesCount) { throw new Exception("Mismatch in list sizes of right part"); }
            #endregion

            //After consolidation, use data to set values. First are the states
            newPart.startState = "q0";
            newPart.finalState = "q1";

            newPart.states = new HashSet<string> { newPart.startState, newPart.finalState };

            IEnumerable<string> containedStates = leftPart.states.Concat(rightPart.states);
            newPart.states.Concat(containedStates);

            //Followed by the transitions
            newPart.transitions = new HashSet<Transition<string>>
            {
                new Transition<string>("q0", leftPart.startState),
                new Transition<string>("q0", rightPart.startState),
                new Transition<string>(leftPart.finalState, "q1"),
                new Transition<string>(rightPart.finalState, "q1")
            };

            IEnumerable<Transition<string>> containedTransitions = leftPart.transitions.Concat(rightPart.transitions);
            IEnumerable<Transition<string>> allTransitions = newPart.transitions.Concat(containedTransitions);
            newPart.transitions = new HashSet<Transition<string>>(allTransitions);

            return newPart;
        }

        /// <summary>
        /// Turns the given substring with the plus operator into the equivalent NDFA part.
        /// </summary>
        /// <param name="subString">The new part of the Regex to contain within this part.</param>
        /// <returns>A <see cref="ThompsonPart"/> that has all states and transitions.</returns>
        private ThompsonPart GeneratePlusOperatorConstruction(string subString)
        {
            ThompsonPart newPart = new ThompsonPart();
            ThompsonPart containedPart = GenerateThompsonPart(subString);

            #region State consolidation
            //Consolidate states here so that all states are unique (this is not useable by multiple constructions, since those differ in contained elements
            int containedStatesCount = containedPart.states.Count;
            int leftStatesCountPlusOne = containedStatesCount + 1;

            containedPart.startState = "q2";                                                                    //Contained part start state = q2, q0 is our start state for the or and q1 the end state
            containedPart.finalState = "q" + leftStatesCountPlusOne;                                            //Contained part final state = all contained states count, due to being shifted one over
            containedPart.states = new HashSet<string> { containedPart.startState, containedPart.finalState };
            for (int i = 2; i < leftStatesCountPlusOne; i++) { containedPart.states.Add("q" + i); }             //States in between: for(int i = 1, i < count - 1, i++)

            if (containedPart.states.Count != containedStatesCount) { throw new Exception("Mismatch in list sizes of left part"); }
            #endregion

            //After consolidation, use data to set values. First are the states
            newPart.startState = "q0";
            newPart.finalState = "q1";

            newPart.states = new HashSet<string> { newPart.startState, newPart.finalState };

            newPart.states.Concat(containedPart.states);

            //Followed by the transitions
            newPart.transitions = new HashSet<Transition<string>>
            {
                new Transition<string>("q0", containedPart.startState),
                new Transition<string>(containedPart.finalState, "q1"),
                new Transition<string>(containedPart.startState, containedPart.finalState)
            };

            IEnumerable<Transition<string>> allTransitions = newPart.transitions.Concat(containedPart.transitions);
            newPart.transitions = new HashSet<Transition<string>>(allTransitions);

            return newPart;
        }

        /// <summary>
        /// Turns the given substring with the plus operator into the equivalent NDFA part.
        /// </summary>
        /// <param name="subString">The new part of the Regex to contain within this part.</param>
        /// <returns>A <see cref="ThompsonPart"/> that has all states and transitions.</returns>
        private ThompsonPart GenerateAsterixOperatorConstruction(string subString)
        {
            ThompsonPart newPart = new ThompsonPart();
            ThompsonPart containedPart = GenerateThompsonPart(subString);

            #region State consolidation
            //Consolidate states here so that all states are unique (this is not useable by multiple constructions, since those differ in contained elements
            int containedStatesCount = containedPart.states.Count;
            int leftStatesCountPlusOne = containedStatesCount + 1;

            containedPart.startState = "q2";                                                                     //Contained part start state = q2, q0 is our start state for the or and q1 the end state
            containedPart.finalState = "q" + leftStatesCountPlusOne;                                             //Contained part final state = all contained states count, due to being shifted one over
            containedPart.states = new HashSet<string> { containedPart.startState, containedPart.finalState };
            for (int i = 2; i < leftStatesCountPlusOne; i++) { containedPart.states.Add("q" + i); }              //States in between: for(int i = 1, i < count - 1, i++)

            if (containedPart.states.Count != containedStatesCount) { throw new Exception("Mismatch in list sizes of left part"); }
            #endregion

            //After consolidation, use data to set values. First are the states
            newPart.startState = "q0";
            newPart.finalState = "q1";

            newPart.states = new HashSet<string> { newPart.startState, newPart.finalState };

            newPart.states.Concat(containedPart.states);

            //Followed by the transitions
            newPart.transitions = new HashSet<Transition<string>>
            {
                new Transition<string>("q0", containedPart.startState),
                new Transition<string>(containedPart.finalState, "q1"),
                new Transition<string>(containedPart.startState, containedPart.finalState),
                new Transition<string>(newPart.startState, newPart.finalState)
            };

            IEnumerable<Transition<string>> allTransitions = newPart.transitions.Concat(containedPart.transitions);
            newPart.transitions = new HashSet<Transition<string>>(allTransitions);

            return newPart;
        }

        /// <summary>
        /// Checks to see if the given character is a letter between a-z, A-Z or 0-9
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>A boolean that is true if the character fits in one of the three sets.</returns>
        private bool IsUsableCharacter(char c) { return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'); }
    }
}

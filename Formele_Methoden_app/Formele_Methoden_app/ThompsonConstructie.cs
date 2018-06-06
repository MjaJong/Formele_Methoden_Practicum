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
        private class ThompsonPart
        {
            public HashSet<Transition<string>> transitions = new HashSet<Transition<string>>();
            public HashSet<string> states = new HashSet<string>();

            public string startState = string.Empty;
            public string finalState = string.Empty;
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

            bool regexHasBrackets = false;
            int bracketBalance = 0; //Checks to see if we're in balance in the brackets, thus passing enough brackets to actually close.
            bool foundEnclosedSubstring = false;

            char regexOperator = '$';
            int operatorPosition = -1;

            //Iterate over string, check for brackets
            for (int i = 0; i < regexAsString.Length; i++)
            {
                //Special character checks
                //Check to see if we have an enclosed section which will be important
                if (regexAsString[i] == '(')
                {
                    foundEnclosedSubstring = true;
                    regexHasBrackets = true;
                    bracketBalance++;
                }
                if (regexAsString[i] == ')')
                {
                    bracketBalance--;
                    if (bracketBalance == 0) foundEnclosedSubstring = false;
                }

                //And save the special operator
                if (regexAsString[i] == '.' && regexOperator == '$') { regexOperator = '.'; operatorPosition = i; } //Yes I know two statements as a one liner. Couldn't be bothered to take up the extra space.
                if (regexAsString[i] == '|' && regexOperator == '$') { regexOperator = '|'; operatorPosition = i; }
                if (regexAsString[i] == '+' && regexOperator == '$') { regexOperator = '+'; operatorPosition = i; }
                if (regexAsString[i] == '*' && regexOperator == '$') { regexOperator = '*'; operatorPosition = i; }

                if(regexHasBrackets) //Thus we have brackets in our regex
                {
                    //Check to see if we should break, or that we should continue (should one of the operators be contained within brackets)
                    if (regexOperator != '$' && foundEnclosedSubstring)//This means that we have a contained operator
                    {
                        regexOperator = '$';
                        operatorPosition = -1;
                    }
                    else if(regexOperator != '$') //Translates the operator that has been found
                    {
                        generatedPart = TranslateOperator(regexAsString, regexOperator, operatorPosition, regexHasBrackets);
                    }
                }
                else if(i == regexAsString.Length - 1)//When that last character has been passed, dive into this part
                {
                    //Translates the operator that has been found when there are no more brackets
                    if (regexOperator != '$')
                    {
                        generatedPart = TranslateOperator(regexAsString, regexOperator, operatorPosition, regexHasBrackets);
                    }
                    //We've found no brackets and operators, thus we can turn the characters into transitions
                    else
                    {
                        generatedPart = GenerateMultipleSymbolTransition(regexAsString);
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
        /// <param name="regex">The regex to translate.</param>
        /// <param name="regexOperator">The operator found</param>
        /// <param name="operatorPosition">Position of the operator to translate. Used to some substring stuff.</param>
        /// <returns>A <see cref="ThompsonPart"/> that contains all data.</returns>
        private ThompsonPart TranslateOperator(string regex, char regexOperator, int operatorPosition, bool regexHasBrackets)
        {
            //Build in check to translate character without brackets TODO: check for -1 in either position, if so assume there are no more brackets
            string leftExpression = "";
            string rightExpression = "";

            leftExpression = regex.Substring(0, operatorPosition); //Also doubles as the whole regex minus the operator
            if(operatorPosition + 1 < regex.Length) { rightExpression = regex.Substring(operatorPosition + 1, (regex.Length - leftExpression.Length - 1)); }

           if(regexHasBrackets)
            {
                leftExpression = StripRegexOfOuterBrackets(leftExpression);
                if (rightExpression != "") { rightExpression = StripRegexOfOuterBrackets(rightExpression); }
            }

            ThompsonPart generatedPart = new ThompsonPart();

            //And generate something for the operator. Note that the first two split the string, while the second two do not, so the second two take the whole string
            switch (regexOperator)
            {
                case '.':
                    generatedPart = GenerateDotOperatorConstruction(leftExpression, rightExpression);
                    break;
                case '|':
                    generatedPart = GenerateOrOperatorConstruction(leftExpression, rightExpression);
                    break;
                case '+':
                    generatedPart = GeneratePlusOperatorConstruction(leftExpression);
                    break;
                case '*':
                    generatedPart = GenerateAsterixOperatorConstruction(leftExpression);
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
                if(generatedPart.states.Count <= 0)
                {
                    generatedPart = GenerateSingleSymbolTransition(c);
                }
                else //Create an extra part and concat both transitions and the states. The new end state is the latest end state, thus the endstate of the extra part.
                {
                    ThompsonPart extraPart = GenerateSingleSymbolTransition(c);

                    Dictionary<string, string> remappedStates = new Dictionary<string, string>();
                    int stateIndex = generatedPart.states.Count - 1;

                    foreach(string state in extraPart.states)
                    {
                        remappedStates.Add(state, "q" + stateIndex);
                        stateIndex++;
                        generatedPart.states.Add(remappedStates[state]);
                    }

                    foreach(Transition<string> transition in extraPart.transitions)
                    {
                        generatedPart.transitions.Add(new Transition<string>(remappedStates[transition.FromState], remappedStates[transition.ToState], transition.Identifier));
                    }

                    generatedPart.finalState = remappedStates[extraPart.finalState];
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
        private ThompsonPart GenerateDotOperatorConstruction(string leftSubstring, string rightSubstring)//Works
        {
            ThompsonPart newPart = new ThompsonPart();
            ThompsonPart leftPart = GenerateThompsonPart(leftSubstring);
            ThompsonPart rightPart = GenerateThompsonPart(rightSubstring);

            #region State consolidation
            //Consolidate states here so that all states are unique (this is not useable by multiple constructions, since those differ in contained elements
            int leftStatesCount = leftPart.states.Count;
            int rightStatesCount = rightPart.states.Count;

            Dictionary<string, string> remappedStates = new Dictionary<string, string>();
            int stateIndex = 0;

            foreach (string state in leftPart.states)
            {
                remappedStates.Add(state, "q" + stateIndex);
                stateIndex++;
                newPart.states.Add(remappedStates[state]);
            }

            leftPart.startState = remappedStates[leftPart.startState];
            leftPart.finalState = remappedStates[leftPart.finalState];

            foreach (Transition<string> transition in leftPart.transitions)
            {
                newPart.transitions.Add(new Transition<string>(remappedStates[transition.FromState], remappedStates[transition.ToState], transition.Identifier));
            }

            if (leftPart.states.Count != leftStatesCount) { throw new Exception("Mismatch in list sizes of left part"); }

            remappedStates.Clear();

            foreach (string state in rightPart.states)
            {
                remappedStates.Add(state, "q" + stateIndex);
                stateIndex++;
                newPart.states.Add(remappedStates[state]);
            }

            rightPart.startState = remappedStates[rightPart.startState];
            rightPart.finalState = remappedStates[rightPart.finalState];

            foreach (Transition<string> transition in rightPart.transitions)
            {
                newPart.transitions.Add(new Transition<string>(remappedStates[transition.FromState], remappedStates[transition.ToState], transition.Identifier));
            }

            if (rightPart.states.Count != rightStatesCount) { throw new Exception("Mismatch in list sizes of right part"); }
            #endregion

            //After consolidation, use data to set values
            Transition<string> epsilonTransition = new Transition<string>(leftPart.finalState, rightPart.startState);                          //Epsilon transition between the two parts.

            newPart.transitions.Add(epsilonTransition);

            newPart.startState = leftPart.startState;
            newPart.finalState = rightPart.finalState;

            return newPart;
        }

        /// <summary>
        /// Turns two regex substring into the or operator equivalent NDFA part.
        /// </summary>
        /// <param name="leftSubstring">The substring that should be between state q2 and q3.</param>
        /// <param name="rightSubstring">The substring that should be between state q4 and q5.</param>
        /// <returns>A <see cref="ThompsonPart"/> that has all states and transitions.</returns>
        private ThompsonPart GenerateOrOperatorConstruction(string leftSubstring, string rightSubstring)//Works
        {
            ThompsonPart newPart = new ThompsonPart();
            ThompsonPart leftPart = GenerateThompsonPart(leftSubstring);
            ThompsonPart rightPart = GenerateThompsonPart(rightSubstring);

            #region State consolidation
            //Consolidate states here so that all states are unique (this is not useable by multiple constructions, since those differ in contained elements. now also handles states
            int leftStatesCount = leftPart.states.Count;
            int rightStatesCount = rightPart.states.Count;

            Dictionary<string, string> remappedStates = new Dictionary<string, string>();
            int stateIndex = 2;

            foreach (string state in leftPart.states)
            {
                remappedStates.Add(state, "q" + stateIndex);
                stateIndex++;
                newPart.states.Add(remappedStates[state]);
            }

            leftPart.startState = remappedStates[leftPart.startState];
            leftPart.finalState = remappedStates[leftPart.finalState];

            foreach (Transition<string> transition in leftPart.transitions)
            {
                newPart.transitions.Add(new Transition<string>(remappedStates[transition.FromState], remappedStates[transition.ToState], transition.Identifier));
            }

            if (leftPart.states.Count != leftStatesCount) { throw new Exception("Mismatch in list sizes of left part"); }

            remappedStates.Clear();

            foreach (string state in rightPart.states)
            {
                remappedStates.Add(state, "q" + stateIndex);
                stateIndex++;
                newPart.states.Add(remappedStates[state]);
            }

            rightPart.startState = remappedStates[rightPart.startState];
            rightPart.finalState = remappedStates[rightPart.finalState];

            foreach (Transition<string> transition in rightPart.transitions)
            {
                newPart.transitions.Add(new Transition<string>(remappedStates[transition.FromState], remappedStates[transition.ToState], transition.Identifier));
            }

            if (rightPart.states.Count != rightStatesCount) { throw new Exception("Mismatch in list sizes of right part"); }
            #endregion

            //After consolidation, use data to set values. First are the states
            newPart.startState = "q0";
            newPart.finalState = "q1";

            string[] newStates = { newPart.startState, newPart.finalState };
            foreach(string state in newStates) { newPart.states.Add(state); }

            //Followed by the transitions
            Transition<string>[] newTransitions= 
            {
                new Transition<string>("q0", leftPart.startState),
                new Transition<string>("q0", rightPart.startState),
                new Transition<string>(leftPart.finalState, "q1"),
                new Transition<string>(rightPart.finalState, "q1")
            };

            foreach (Transition<string> transition in newTransitions) { newPart.transitions.Add(transition); }

            return newPart;
        }

        /// <summary>
        /// Turns the given substring with the plus operator into the equivalent NDFA part.
        /// </summary>
        /// <param name="subString">The new part of the Regex to contain within this part.</param>
        /// <returns>A <see cref="ThompsonPart"/> that has all states and transitions.</returns>
        private ThompsonPart GeneratePlusOperatorConstruction(string subString)//Works
        {
            ThompsonPart newPart = new ThompsonPart();
            ThompsonPart containedPart = GenerateThompsonPart(subString);

            #region State consolidation
            //Consolidate states here so that all states are unique (this is not useable by multiple constructions, since those differ in contained elements
            int containedStatesCount = containedPart.states.Count;
            Dictionary<string, string> remappedStates = new Dictionary<string, string>();
            int stateIndex = 2;

            foreach (string state in containedPart.states)
            {
                remappedStates.Add(state, "q" + stateIndex);
                stateIndex++;
                newPart.states.Add(remappedStates[state]);
            }

            containedPart.startState = remappedStates[containedPart.startState];
            containedPart.finalState = remappedStates[containedPart.finalState];

            foreach (Transition<string> transition in containedPart.transitions)
            {
                newPart.transitions.Add(new Transition<string>(remappedStates[transition.FromState], remappedStates[transition.ToState], transition.Identifier));
            }

            if (containedPart.states.Count != containedStatesCount) { throw new Exception("Mismatch in list sizes of left part"); }
            #endregion

            //After consolidation, use data to set values. First are the states
            newPart.startState = "q0";
            newPart.finalState = "q1";

            string[] newStates = { newPart.startState, newPart.finalState };
            foreach (string state in newStates) { newPart.states.Add(state); }

            //Followed by the transitions
            Transition<string>[] newTransitions =
            {
                new Transition<string>("q0", containedPart.startState),
                new Transition<string>(containedPart.finalState, "q1"),
                new Transition<string>(containedPart.finalState, containedPart.startState)
            };

            foreach (Transition<string> transition in newTransitions) { newPart.transitions.Add(transition); }

            return newPart;
        }

        /// <summary>
        /// Turns the given substring with the plus operator into the equivalent NDFA part.
        /// </summary>
        /// <param name="subString">The new part of the Regex to contain within this part.</param>
        /// <returns>A <see cref="ThompsonPart"/> that has all states and transitions.</returns>
        private ThompsonPart GenerateAsterixOperatorConstruction(string subString)//Works
        {
            ThompsonPart newPart = new ThompsonPart();
            ThompsonPart containedPart = GenerateThompsonPart(subString);

            #region State consolidation
            //Consolidate states here so that all states are unique (this is not useable by multiple constructions, since those differ in contained elements
            int containedStatesCount = containedPart.states.Count;
            Dictionary<string, string> remappedStates = new Dictionary<string, string>();
            int stateIndex = 2;

            foreach (string state in containedPart.states)
            {
                remappedStates.Add(state, "q" + stateIndex);
                stateIndex++;
                newPart.states.Add(remappedStates[state]);
            }

            containedPart.startState = remappedStates[containedPart.startState];
            containedPart.finalState = remappedStates[containedPart.finalState];

            foreach (Transition<string> transition in containedPart.transitions)
            {
                newPart.transitions.Add(new Transition<string>(remappedStates[transition.FromState], remappedStates[transition.ToState], transition.Identifier));
            }

            if (containedPart.states.Count != containedStatesCount) { throw new Exception("Mismatch in list sizes of left part"); }
            #endregion

            //After consolidation, use data to set values. First are the states
            newPart.startState = "q0";
            newPart.finalState = "q1";

            string[] newStates = { newPart.startState, newPart.finalState };
            foreach (string state in newStates) { newPart.states.Add(state); }

            //Followed by the transitions
            Transition<string>[] newTransitions =
            {
                new Transition<string>("q0", containedPart.startState),
                new Transition<string>(containedPart.finalState, "q1"),
                new Transition<string>(containedPart.finalState, containedPart.startState),
                new Transition<string>(newPart.startState, newPart.finalState)
            };

            foreach (Transition<string> transition in newTransitions) { newPart.transitions.Add(transition); }

            return newPart;
        }

        /// <summary>
        /// Checks to see if the given character is a letter between a-z, A-Z or 0-9
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>A boolean that is true if the character fits in one of the three sets.</returns>
        private bool IsUsableCharacter(char c) { return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'); }

        /// <summary>
        /// Removes the outer brackets of a regex.
        /// </summary>
        /// <param name="regex">The string regex to strip</param>
        /// <returns>The regex without the brackets</returns>
        private string StripRegexOfOuterBrackets(string regex)
        {
            List<int> openingBrackets = new List<int>();
            List<int> closingBrackets = new List<int>();

            for(int i = 0; i < regex.Length; i++)
            {
                if (regex[i] == '(') { openingBrackets.Add(i); }
                if (regex[i] == ')') { closingBrackets.Add(i); }
            }

            return regex.Substring(openingBrackets.First() + 1, (closingBrackets.Last() - openingBrackets.First() - 1));
        }
    }
}

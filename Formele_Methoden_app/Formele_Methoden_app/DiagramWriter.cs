using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formele_Methoden_app
{
    public class DiagramWriter
    {
        public void WriteToGVFile<T>(Automata<T> automata, string filename, string path, bool leftRight = false) where T : IComparable
        {
            Dictionary<T, int> stateNumbers = new Dictionary<T, int>();
            int i = 1;
            foreach (T state in automata.states)
            {
                stateNumbers.Add(state, i);
                i++;
            }
            
            using (StreamWriter writer = new StreamWriter($"{path}{filename}.gv"))
            {
                //start
                writer.WriteLine($"digraph {filename} {{ ");
                if (leftRight)
                    writer.WriteLine("rankdir=LR;");

                //labels
                writer.WriteLine(@"NOTHING [label="""", shape=none]");

                foreach (KeyValuePair<T, int> state in stateNumbers)
                {
                    if (automata.startStates.Contains(state.Key))
                        writer.WriteLine($@"{state.Value} [label=""{state.Key.ToString()}"", shape=ellipse, style=filled, color=lightblue]");
                    else if(automata.finalStates.Contains(state.Key))
                        writer.WriteLine($@"{state.Value} [label=""{state.Key.ToString()}"", shape=ellipse, peripheries=2, style=filled, color=yellowgreen]");
                    else
                        writer.WriteLine($@"{state.Value} [label=""{state.Key.ToString()}"", shape=ellipse]");
                }

                writer.WriteLine("");

                //transitions
                foreach (T start in automata.startStates)
                    writer.WriteLine($@"NOTHING -> {stateNumbers.FirstOrDefault(x => x.Key.Equals(start)).Value}");

                foreach (Transition<T> transition in automata.transitions)
                {
                    int from = stateNumbers.FirstOrDefault(x => x.Key.Equals(transition.FromState)).Value;
                    int to = stateNumbers.FirstOrDefault(x => x.Key.Equals(transition.ToState)).Value;
                    char symbol = transition.Identifier;

                    writer.WriteLine($@"{from} -> {to} [label=""{symbol}""]");
                }
                    

                writer.WriteLine("}");
            }
        }
    }
}

using System;

namespace Formele_Methoden_app
{
    class Transition<T> : IComparable<Transition<T>>
    {
        //TODO: Need to check the typing on these for further conflicts
        public Transition<T> FromState { get; private set; }
        public Transition<T> ToState { get; private set; }
        public char Identifier { get; private set; }

        private static char EPSILON = '$';

        /// <summary>
        /// Creates a standard <see cref="Transition{T}"/>
        /// </summary>
        /// <param name="from">From who the transition originates.</param>
        /// <param name="to">To who the transition goes.</param>
        /// <param name="id">The identifier for this transition.</param>
        public Transition(T from, T to, char id)
        {
            FromState = from;
            ToState = to;
            Identifier = id;
        }

        /// <summary>
        /// Creates a loop <see cref="Transition{T}"/>, sharing the source and target
        /// </summary>
        /// <param name="fromAndTo">Target and source for this transition</param>
        /// <param name="id">The identifier for this transition.</param>
        public Transition(T fromAndTo, char id)
        {
            FromState = fromAndTo;
            ToState = fromAndTo;
            Identifier = id;
        }

        /// <summary>
        /// Creates a <see cref="Transition{T}"/> without an identifying character
        /// </summary>
        /// <param name="from">From who the transition originates.</param>
        /// <param name="to">To who the transition goes.</param>
        public Transition(T from, T to)
        {
            FromState = from;
            ToState = to;
            Identifier = EPSILON;
        }

        /// <summary>
        /// Override of the equals method for <see cref="Transition{T}"/>
        /// </summary>
        /// <param name="other">The <see cref="Transition{T}"/> to compare to.</param>
        /// <returns>A boolean indicator that is true if the objects equal each other.</returns>
        public override bool Equals(Object other)
        {
            if (other == null)
            {
                return false;
            }
            else if (ReferenceEquals(this, other))
            {
                return Equals(((Transition<T>)other)) && Identifier == (((Transition<T>)other).Identifier);
            }
            else return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Compares two <see cref="Transition{T}"/> objects.
        /// </summary>
        /// <param name="other">The <see cref="Transition{T}"/> to compare to.</param>
        /// <returns></returns>
        public int CompareTo(Transition<T> other)
        {
            int fromCompare = FromState.CompareTo(other.FromState);
            int idCompare = Identifier.CompareTo(other.Identifier);
            int toCompare = ToState.CompareTo(other.ToState);

            //And now, for some magic
            return (fromCompare != 0 ? fromCompare : (idCompare != 0 ? idCompare : toCompare));
        }

        /// <summary>
        /// Returns a string represitation of this <see cref="Transition{T}"/>.
        /// </summary>
        /// <returns>A string representation of this <see cref="Transition{T}"/></returns>
        public override string ToString()
        {
            return "(" + FromState + ", " + Identifier + ")" + "-->" + ToState;
        }
    }
}

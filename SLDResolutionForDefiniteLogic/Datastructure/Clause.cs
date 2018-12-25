using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SLDResolutionForDefiniteLogic.Datastructure
{

    [Serializable]
    public abstract class Statement
    {
        // Properties
        public List<Literal> literal;

        // Constructors
        public Statement()
        {
            this.literal = new List<Literal>();
        }

        // Methods
        public abstract String Compose();

    }

    [Serializable]
    public class Goal : Statement
    {

        // Constructors
        public Goal(List<Literal> list)
        {
            this.literal = list;
        }

        // Methods
        public bool IsEmpty()
        {
            return this.literal.Count == 0;
        }
        public override String Compose()
        {
            if (!this.IsEmpty())
            {
                String r = "← ";

                int i;
                for (i = 0; i < literal.Count - 1; i++)
                {
                    r += literal[i].Compose() + ", ";
                }
                r += literal[i].Compose();
                return r;
            }
            else
            {
                return "←";
            }

        }

        public static Goal Decompose(String s)
        {
            s = Regex.Replace(s, @"\s+", "");


            List<Literal> list = new List<Literal>();

            s = s.Substring(3);
            // s = "child_of(b, X), hate(Y, X), child_of(f2(f1(X)), f2(f1(X)))"

            Stack<int> OpenBracket = new Stack<int>();
            Stack<int> CloseBracket = new Stack<int>();

            int startOfCurrentLiteral = 0;
            for (int i = 0; i < s.Length; i++)
            {
                // Bracket identification
                if (s[i] == '(')
                {
                    OpenBracket.Push(i);
                }
                else
                {
                    if (s[i] == ')')
                    {
                        CloseBracket.Push(i);
                    }
                }

                if (OpenBracket.Count != 0 && OpenBracket.Count == CloseBracket.Count)
                {
                    list.Add(Literal.Decompose(s.Substring(startOfCurrentLiteral, CloseBracket.Peek() + 1 - startOfCurrentLiteral)));
                    for (; i < s.Length && s[i] != ','; i++)
                        ;

                    // Reset, start processing a new Literal
                    if (i + 1 < s.Length)
                    {
                        startOfCurrentLiteral = i + 1;
                    }
                    OpenBracket.Clear();
                    CloseBracket.Clear();
                }

            }


            return new Goal(list);
        }

        public static Goal ApplySubstitution(Goal X, Substitution T)
        {
            for (int i = 0; i < X.literal.Count; i++)
            {
                X.literal[i] = Literal.ApplySubstitution(X.literal[i], T);
            }

            return X;
        }
    }

    [Serializable]
    public class Clause : Statement
    {
        // Constructors
        public Clause(List<Literal> list)
        {
            this.literal = list;
        }

        // Methods
        public override String Compose()
        {

            if (this.literal.Count == 1)
            {
                return literal[0].Compose();
            }
            else
            {

                String r = literal[0].Compose() + " ← ";

                int i;
                for (i = 1; i < literal.Count - 1; i++)
                {
                    r += literal[i].Compose() + ", ";
                }
                r += literal[i].Compose();

                return r;
            }
        }

        public static Clause Decompose(String s)
        {
            s = Regex.Replace(s, @"\s+", "");


            List<Literal> list = new List<Literal>();

            int indexOfArrow = s.IndexOf('<');
            if (indexOfArrow != -1)
            {
                list.Add(Literal.Decompose(s.Substring(0, indexOfArrow)));


                s = s.Substring(indexOfArrow + 3);
                // s = "child_of(b, X), hate(Y, X), child_of(f2(f1(X)), f2(f1(X)))"

                Stack<int> OpenBracket = new Stack<int>();
                Stack<int> CloseBracket = new Stack<int>();

                int startOfCurrentLiteral = 0;
                for (int i = 0; i < s.Length; i++)
                {
                    // Bracket identification
                    if (s[i] == '(')
                    {
                        OpenBracket.Push(i);
                    }
                    else
                    {
                        if (s[i] == ')')
                        {
                            CloseBracket.Push(i);
                        }
                    }

                    if (OpenBracket.Count != 0 && OpenBracket.Count == CloseBracket.Count)
                    {
                        list.Add(Literal.Decompose(s.Substring(startOfCurrentLiteral, CloseBracket.Peek() + 1 - startOfCurrentLiteral)));
                        for (; i < s.Length && s[i] != ','; i++)
                            ;

                        // Reset, start processing a new Literal
                        if (i + 1 < s.Length)
                        {
                            startOfCurrentLiteral = i + 1;
                        }
                        OpenBracket.Clear();
                        CloseBracket.Clear();
                    }
                }
            }
            else
            {
                list.Add(Literal.Decompose(s));
            }

            return new Clause(list);
        }

        public static Clause ApplySubstitution(Clause X, Substitution T)
        {
            for (int i = 0; i < X.literal.Count; i++)
            {
                X.literal[i] = Literal.ApplySubstitution(X.literal[i], T);
            }

            return X;
        }
    }
}

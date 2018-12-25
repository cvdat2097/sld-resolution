using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SLDResolutionForDefiniteLogic.Datastructure
{
    [Serializable]
    public class Literal
    {
        // Properties
        public String name;
        public List<Element> element;

        // Constructors
        public Literal(String name, List<Element> e)
        {
            this.name = name;
            this.element = e;
        }

        // Methods
        public String Compose()
        {
            String r = this.name + "(";

            int i;
            for (i = 0; i < element.Count - 1; i++)
            {
                r += element[i].Compose() + ", ";
            }
            r += element[i].Compose() + ")";

            return r;
        }

        public static Literal Decompose(String s)
        {


            String name = s.Substring(0, s.IndexOf('('));

            s = s.Substring(s.IndexOf('(') + 1);

            List<Element> list = new List<Element>();

            while (s.Length > 0)
            {
                String text = "";
                int indexOfComma = s.IndexOf(',');
                if (indexOfComma != -1)
                {
                    text = s.Substring(0, indexOfComma);
                    s = s.Substring(indexOfComma + 1);
                }
                else
                {
                    text = s.Substring(0, s.Length - 1);
                    s = "";
                }

                list.Add(Element.CreateElement(text));
            }


            return new Literal(name, list);
        }

        public static Literal ApplySubstitution(Literal A, Substitution T)
        {
            String oldLiteral = A.Compose();

            for (int i = 0; i < T.replacementList.Count; i++)
            {
                Replacement R = T.replacementList[i];
                String varName = R.X.name;

                oldLiteral = oldLiteral.Replace(varName, R.t.Compose());
            }

            return Literal.Decompose(oldLiteral);
        }
    }
}

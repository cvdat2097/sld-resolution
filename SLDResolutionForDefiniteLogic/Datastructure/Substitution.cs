using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace SLDResolutionForDefiniteLogic.Datastructure
{
    public class Substitution
    {
        // Properties
        public List<Replacement> replacementList;

        // Constructors
        public Substitution(List<Replacement> list)
        {
            this.replacementList = list;
        }

        // Methods
        public String Compose()
        {
            if (this.replacementList.Count != 0)
            {


                String r = "{";

                int i;
                for (i = 0; i < replacementList.Count - 1; i++)
                {
                    r += replacementList[i].Compose() + ", ";
                }
                r += replacementList[i].Compose();

                return r + "}";
            }
            else
            {
                return "{}";
            }
        }

        public static Substitution Decompose(String s)
        {
            s = Regex.Replace(s, @"\s+", "");
            s = s.Substring(1, s.Length - 2);


            List<Replacement> list = new List<Replacement>();

            int indexOfComma;
            while ((indexOfComma = s.IndexOf(',')) != -1)
            {
                String currentReplacement = s.Substring(0, indexOfComma);
                list.Add(Replacement.Decompose(currentReplacement));
                s = s.Substring(indexOfComma + 1);
            }
            list.Add(Replacement.Decompose(s));

            return new Substitution(list);
        }

        public bool isValid()
        {
            for(int i =0;i < replacementList.Count;i++)
            {
                for(int j =i + 1;j < replacementList.Count;j++)
                {
                    if (replacementList[i].Compose() == replacementList[j].Compose())
                        return false;
                }
            }

            return true;
        }
    }

    public class Replacement
    {
        // Properties
        public Variable X;
        public Element t;

        // Constructors
        public Replacement(Variable X, Element t)
        {
            if (X == null && t == null)
            {
                this.X = null;
                this.t = null;
            }
            else
            {
                // OLD CODE
                //Variable var = new Variable(X.name);

                //Element e;

                //if (t.type == "Variable")
                //{
                //    e = new Variable(t.name);
                //}
                //else if (t.type == "Functor")
                //{
                //    e = new Functor(t.name, (t as Functor).parameter);
                //}
                //else
                //{
                //    e = new Constant(t.name);
                //}

                //this.X = var;
                //this.t = e;

                // Deep copy
                Variable newX = Lib.DeepClone<Variable>(X);
                Element newT = Lib.DeepClone<Element>(t);

                this.X = newX;
                this.t = newT;
            }
        }

        // Methods
        public String Compose()
        {
            if (X == null && t == null)
            {
                return "";
            }

            return X.Compose() + "/" + t.Compose();
        }

        public static Replacement Decompose(String s)
        {
            int indexOfSlash = s.IndexOf('/');

            if (indexOfSlash != -1)
            {
                Variable X = new Variable(s.Substring(0, indexOfSlash));
                Element t = Element.CreateElement(s.Substring(indexOfSlash + 1));

                return new Replacement(X, t);
            }
            else
            {
                return null;
            }
        }
    }
}

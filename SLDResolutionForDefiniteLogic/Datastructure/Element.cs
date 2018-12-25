using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SLDResolutionForDefiniteLogic.Datastructure;

namespace SLDResolutionForDefiniteLogic
{
    [Serializable]
    public abstract class Element
    {
        // Properties
        public String name;
        public String type;

        // Constructors
        public Element()
        {
            name = "undefined";
        }

        // Methods
        public abstract String Compose();
        public static Element CreateElement(String s)
        {
            s = Regex.Replace(s, @"\s+", "");
            if (s.IndexOf('(') != -1)
            {
                // Functor
                return Functor.Decompose(s);
            }
            else
            {
                if (Char.IsUpper(s[0]) && s.Length == 1)
                {
                    // Variable
                    return Variable.Decompose(s);
                }
                else
                {
                    // Constant
                    return Constant.Decompose(s);
                }
            }
        }

        public static Replacement Unify(Element A, Element B)
        {
            if ((A.type == "Constant" && B.type == "Constant") || (A.type == "Variable" && B.type=="Variable" && A.name == B.name))
            {
                if (A.name == B.name)
                {
                    return new Replacement(null, null);
                }
            }
            else if (B.type == "Variable")
            {
                return new Replacement(B as Variable, A);
            }
            else if (A.type == "Variable")
            {
                return new Replacement(A as Variable, B);
            }
            else if (A.type == "Functor" && B.type == "Functor")
            {
                if (A.name != B.name)
                {
                    return null;
                }
                return Unify((A as Functor).parameter, (B as Functor).parameter);
            }

            return null;
        }
    }

    [Serializable]
    public class Variable : Element
    {
        // Constructors
        public Variable(String name)
        {
            this.name = name;
            this.type = "Variable";
        }

        // Methods
        public override string Compose()
        {
            return this.name;
        }

        public static Variable Decompose(String text)
        {
            text = Regex.Replace(text, @"\s+", "");
            return new Variable(text);
        }
    }

    [Serializable]
    public class Functor : Element
    {
        // Properties
        public Element parameter;

        // Constructors
        public Functor(String name, Element param)
        {
            this.name = name;
            this.parameter = param;
            this.type = "Functor";
        }

        // Methods
        public override string Compose()
        {
            return this.name + "(" + parameter.Compose() + ")";
        }

        public static Functor Decompose(String text)
        {
            text = Regex.Replace(text, @"\s+", "");

            int indexOfFirstOpenBracket = text.IndexOf('(');
            String functorName = text.Substring(0, indexOfFirstOpenBracket);
            text = text.Substring(indexOfFirstOpenBracket + 1, text.LastIndexOf(')') - indexOfFirstOpenBracket - 1);

            return new Functor(functorName, CreateElement(text));
        }
    }

    [Serializable]
    public class Constant : Element
    {
        // Constructor
        public Constant(String name)
        {
            this.name = name;
            this.type = "Constant";
        }

        // Methods
        public override string Compose()
        {
            return this.name;
        }

        public static Constant Decompose(String text)
        {
            text = Regex.Replace(text, @"\s+", "");
            return new Constant(text);
        }
    }
}

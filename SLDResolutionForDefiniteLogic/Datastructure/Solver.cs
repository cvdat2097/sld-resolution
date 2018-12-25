using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SLDResolutionForDefiniteLogic.Datastructure
{
    public class SolutionStep
    {
        // Properties
        public String goal;
        public String clause;
        public String substitution;

        // Constructors
        public SolutionStep()
        {
            this.goal = "";
            this.clause = "";
            this.substitution = "";
        }

        public SolutionStep(Goal g, Clause c, Substitution theta)
        {
            this.goal = g.Compose();
            this.clause = c.Compose();
            this.substitution = theta.Compose();
        }

    }


    public static class Solver
    {
        // Properties
        public static List<SolutionStep> solution;
        public static List<Clause> logicProgram;
        public static Goal finalGoal;
        public static bool solutionFound = false;
        public static int _LOOP_IDENTIFIER_ = 11;

       

        // Unification
        public static Substitution Unify(Literal A, Literal B)
        {

            if (A.name != B.name || A.element.Count != B.element.Count )
            {
                // Can not Unify
                return null;
            }
            else
            {
                List<Replacement> list = new List<Replacement>();
                // May unify
                for (int i = 0; i < A.element.Count; i++)
                {
                    if (A.element[i].type == "Constant" && B.element[i].type == "Constant" && A.element[i].name != B.element[i].name)
                        return null;

                    Replacement r = Element.Unify(A.element[i], B.element[i]);
                    if (r != null)
                    {
                        if (r.t != null && r.X != null)
                        {
                            list.Add(r);
                        }
                    }
                }


                return new Substitution(list);
            }

        }

        // Combine substitution
        public static Substitution CombineSubstitution(List<Substitution> list)
        {
            return new Substitution(new List<Replacement>());
        }

        public static void Initialize(String knowledgeBase, String conclusion)
        {
            // Reset
            solution = new List<SolutionStep>();
            logicProgram = new List<Clause>();
            solutionFound = false;

            // Build Logic Program
            char[] delimiter = { '\r', '\n' };
            string[] knowledgeBaseList = knowledgeBase.Split(delimiter);

            for (int i = 0; i < knowledgeBaseList.Length; i++)
            {
                if (knowledgeBaseList[i] != "")
                {
                    logicProgram.Add(Clause.Decompose(knowledgeBaseList[i]));
                }
            }

            // Build Goal
            finalGoal = Goal.Decompose(conclusion);
        }

        public static void Solve(Goal goal)
        {
            // End condition
            if (_LOOP_IDENTIFIER_ <= 0)
            {
                //solutionFound = true;
                //solution = new List<SolutionStep>();
                //solution.Add(new SolutionStep());
                return;
            }
            else
            {
                if (goal.IsEmpty() && !solutionFound)
                {
                    solutionFound = true;
                    solution.Add(new SolutionStep());
                }

                // Find solution using back-tracking

                for (int i = 0; i < goal.literal.Count && !solutionFound; i++)
                {
                    Literal Ai = goal.literal[i];
                    for (int j = 0; j < logicProgram.Count && !solutionFound; j++)
                    {
                        Literal Bj = logicProgram[j].literal[0];

                        Substitution thetai = Solver.Unify(Ai, Bj);

                        if (thetai != null && thetai.isValid())
                        {
                            // Log solution step
                            solution.Add(new SolutionStep(goal, logicProgram[j], thetai));

                            // Backup current goal
                            //Literal backup = goal.literal[i];
                            Goal backupGoal = Lib.DeepClone<Goal>(goal);

                            // Change goal
                            goal.literal.RemoveAt(i);
                            goal.literal.InsertRange(i, logicProgram[j].literal.GetRange(1, logicProgram[j].literal.Count - 1));

                            goal = Goal.ApplySubstitution(goal, thetai);

                            _LOOP_IDENTIFIER_--;
                            // ====================== Recursive call 
                            Solve(goal);
                            // ====================== Recursive call 
                            _LOOP_IDENTIFIER_++;

                            if (!solutionFound)
                            {

                                // Restore goal
                                //goal.literal.RemoveRange(i, logicProgram[j].literal.Count - 1);
                                //goal.literal.Insert(i, backup);
                                goal = backupGoal;

                                // Remove solution step
                                solution.RemoveAt(solution.Count - 1);
                            }
                        }
                    }
                }
            }

        }
    }
}

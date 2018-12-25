using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SLDResolutionForDefiniteLogic.Datastructure;
using System.Diagnostics;

namespace SLDResolutionForDefiniteLogic
{
    public partial class Form1 : Form
    {
        public void ResetData()
        {
            Solver.Initialize(txtInput.Text, txtConclusion.Text);
            txtOutput.Text = "";
        }

        public void cmbSampleInit()
        {
            cmbExample.Items.Add("mother(Mary)");
            cmbExample.Items.Add("star(John)");
            cmbExample.Items.Add("p(X)");
            cmbExample.Items.Add("proud(X)");
            cmbExample.Items.Add("r(c)");
            cmbExample.Items.Add("grandfather(a, K)");

        }

        public void ShowResult()
        {
            if (Solver.solutionFound)
            {
                for (int i = 0; i < Solver.solution.Count; i++)
                {
                    txtOutput.Text += "[#" + (i + 1).ToString() + "] G" + i.ToString() + " = " + Solver.solution[i].goal;

                    if (Solver.solution[i].clause != "")
                    {
                        txtOutput.Text += "\r\n";
                        txtOutput.Text += "\tc" + i.ToString() + " = " + Solver.solution[i].clause;
                    }

                    if (Solver.solution[i].substitution != "")
                    {
                        txtOutput.Text += "\r\n";
                        txtOutput.Text += "\tt" + i.ToString() + " = " + Solver.solution[i].substitution;
                        txtOutput.Text += "\r\n";
                        txtOutput.Text += "\r\n";
                    }
                }
            }
            else
            {
                txtOutput.Text = "Can't solve.";
            }
        }
        public Form1()
        {
            InitializeComponent();
            cmbSampleInit();
            cmbExample.SelectedIndex = 0;

            Goal g = Goal.Decompose("<-- r(a), r(a)");
            Clause c = Clause.Decompose("r(c)");

            Substitution th = Solver.Unify(g.literal[0], c.literal[0]);

            //g = Goal.ApplySubstitution(g, th);

            //txtOutput.Text = g.Compose();

        }

        private void btnSolve_Click(object sender, EventArgs e)
        {
            ResetData();

            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            Solver.Solve(Solver.finalGoal);
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Arrow;

            ShowResult();
        }

        
        private void cmbExample_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtOutput.Text = "";
            switch (cmbExample.SelectedIndex)
            {
                case 0:
                    txtInput.Text = @"mother(Mary)
mother(Nancy)
child_of(Tom, Mary)
loves(X, Y) <-- mother(X), child_of(Y, X)";
                    txtConclusion.Text = @"<-- loves(Mary, Z)";
                    break;

                case 1:
                    txtInput.Text = @"star(X) <-- love(Mary, X)
play(Y) <-- star(Y)
study(John) <-- love(Mary, John)
love(Mary, John)";
                    txtConclusion.Text = @"<-- play(Z), study(Z)";
                    break;
                case 2:
                    txtInput.Text = @"p(X) <-- q(X,a), r(X)
q(X,Y) <-- p(X), r(Y)
q(X,Y) <-- p(Y), r(X)
p(a)
r(c)";
                    txtConclusion.Text = @"<-- p(X)";

                    break;

                case 3:
                    txtInput.Text = @"proud(X) <-- parent(X,Y), newborn(Y)
parent(X,Y) <-- father(X, Y)
parent(X,Y) <-- mother(X,Y)
father(adam, mary)
newborn(mary)";
                    //txtConclusion.Text = @"<-- p(X)";
                    txtConclusion.Text = @"<-- proud(Z)";

                    break;
                case 4:
                    txtInput.Text = @"r(c) <-- p(a, X)";
                    //txtConclusion.Text = @"<-- p(X)";
                    txtConclusion.Text = @"<-- r(a)";

                    break;
                case 5:
                    txtInput.Text = @"grandfather(X,Z) <-- father(X,Y), parent(Y,Z)
parent(X,Y) <-- father(X,Y)
parent(X,Y) <-- mother(X,Y)
father(a,b)
mother(b,c)";
                    //txtConclusion.Text = @"<-- p(X)";
                    txtConclusion.Text = @"<-- grandfather(a, K)";
                    
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtOutput.Text = "";
            txtInput.Text = "";
            txtConclusion.Text = "";
        }
    }
}

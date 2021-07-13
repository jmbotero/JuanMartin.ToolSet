using JuanMartin.Utilities.Euler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JuanMartin.EulerProject
{
    public class Program
    {
        private const string separator = "------------------------------------------------------------------------";

        public  static void Main(string[] args)
        {
            Models.Problem[] problems = UtilityEulerProjectSolver.problems;
            var cmd = new Kernel.Processors.CommandLine(string.Join(" ", args).ToString());

            if(cmd.Contains("help"))
            {
                Console.WriteLine(separator);
                Console.WriteLine(cmd["help"].Value);
                Console.WriteLine(separator);
                Console.WriteLine("<Press any key to continue...>");
                Console.ReadKey();
                return;
            }

            if (cmd.Contains("version"))
            {
                Console.WriteLine(separator);
                Console.WriteLine(string.Format("Current command line component version: {0}",cmd["version"].Value));
                Console.WriteLine(separator);
                Console.WriteLine("<Press any key to continue...>");
                Console.ReadKey();
            }

            // creating object of CultureInfo for string parsing
            //CultureInfo cultures = new CultureInfo("en-US");
            IEnumerable<int> problemIds = null;
            IEnumerable<int> skipProblems = null;
            var validateProblems = false;
            var testMode = false;
            try
            {
                if (cmd.Contains("test_mode"))
                    testMode = (bool)cmd["test_mode"].Value;
            }
            catch (Exception)
            {
                throw new ArgumentException("'test_mode' problems option from command line is not parseable as boolean.");
            }
            try
            {
                if (cmd.Contains("validate"))
                    validateProblems = (bool)cmd["validate"].Value;
            }
            catch (Exception)
            {
                throw new ArgumentException("'validate' problems option from command line is not parseable as boolean.");
            }

            try
            {
                if (cmd.Contains("problems"))
                    problemIds = (int[])cmd["problems"].Value; // args[1].Split(',').Select(i => Convert.ToInt32(i, cultures)).ToArray();
            }
            catch (Exception)
            {
                throw new ArgumentException("'problems' array option from command line is not parseable as an array of integers.");
            }

            try
            {
                if (cmd.Contains("skip"))
                    skipProblems = (int[])cmd["skip"].Value;
            }
            catch (Exception)
            {
                throw new ArgumentException("Problems to 'skip' array option from command line is not parseable as an array of integers.");
            }

            UtilityEulerProjectSolver.LoadAnswers(testMode);

            if (problemIds == null && !validateProblems)
            {
                // no list specified, then execute them all
                for (int i = 0; i < problems.Length; i++)
                {
                    if (skipProblems != null && skipProblems.Contains(i))
                        continue;
                    
                    var p = UtilityEulerProjectSolver.GetProblemById(i, testMode);
                    if (p == null)
                        Console.WriteLine(string.Format("{0}roblem {1} not found.",(testMode)?"Test p":"P", i));
                    else
                        UtilityEulerProjectSolver.Launch(p.Script, p,testMode);
                }
            }
            else if (problemIds != null)
            {
                foreach (int id in problemIds)
                {
                    if (id == 0)
                        continue;

                    if (skipProblems != null && skipProblems.Contains(id))
                        continue;

                    var p = UtilityEulerProjectSolver.GetProblemById(id,testMode);
                    if (p == null)
                        Console.WriteLine(string.Format("{0}roblem {1} not found.", (testMode) ? "Test p" : "P", id));
                    else
                        UtilityEulerProjectSolver.Launch(p.Script, p,testMode);
                }
            }

            if (validateProblems)
            {
                Console.WriteLine(separator);
                Console.WriteLine("Verifying problem answers...");
                UtilityEulerProjectSolver.ValidateProblems(problems, skipProblems);
            }
            Console.WriteLine(separator);
            Console.WriteLine("Complete <Press any key to continue...>");
            Console.ReadKey();
        }
    }
}

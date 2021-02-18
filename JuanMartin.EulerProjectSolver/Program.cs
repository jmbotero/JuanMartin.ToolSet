using JuanMartin.Utilities.Euler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JuanMartin.EulerProject
{
    internal class Program
    {
        private const string separator = "------------------------------------------------------------------------";

        private static void Main(string[] args)
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
            IEnumerable<int> problem_ids = null;
            IEnumerable<int> skip_problems = null;
            var validate_problems = false;
            var test_mode = false;
            try
            {
                if (cmd.Contains("test_mode"))
                    test_mode = (bool)cmd["test_mode"].Value;
            }
            catch (Exception)
            {
                throw new ArgumentException("'test_mode' problems option from command line is not parseable as boolean.");
            }
            try
            {
                if (cmd.Contains("validate"))
                    validate_problems = (bool)cmd["validate"].Value;
            }
            catch (Exception)
            {
                throw new ArgumentException("'validate' problems option from command line is not parseable as boolean.");
            }

            try
            {
                if (cmd.Contains("problems"))
                    problem_ids = (int[])cmd["problems"].Value; // args[1].Split(',').Select(i => Convert.ToInt32(i, cultures)).ToArray();
            }
            catch (Exception)
            {
                throw new ArgumentException("'problems' array option from command line is not parseable as an array of integers.");
            }

            try
            {
                if (cmd.Contains("skip"))
                    skip_problems = (int[])cmd["skip"].Value;
            }
            catch (Exception)
            {
                throw new ArgumentException("Problems to 'skip' array option from command line is not parseable as an array of integers.");
            }

            if (problem_ids == null && !validate_problems)
            {
                // no list specified, then execute them all
                for (int i = 0; i < problems.Length; i++)
                {
                    if (skip_problems != null && skip_problems.Contains(i))
                        continue;
                    
                    var p = UtilityEulerProjectSolver.GetProblemById(i, test_mode);
                    if (p == null)
                        Console.WriteLine(string.Format("{0}roblem {1} not found.",(test_mode)?"Test p":"P", i));
                    else
                        UtilityEulerProjectSolver.Launch(p.Script, p);
                }
            }
            else if (problem_ids != null)
            {
                foreach (int id in problem_ids)
                {
                    if (id == 0)
                        continue;

                    if (skip_problems != null && skip_problems.Contains(id))
                        continue;

                    var p = UtilityEulerProjectSolver.GetProblemById(id,test_mode);
                    if (p == null)
                        Console.WriteLine(string.Format("{0}roblem {1} not found.", (test_mode) ? "Test p" : "P", id));
                    else
                        UtilityEulerProjectSolver.Launch(p.Script, p);
                }
            }

            if (validate_problems)
            {
                Console.WriteLine(separator);
                Console.WriteLine("Verifying problem answers...");
                UtilityEulerProjectSolver.ValidateProblems(problems, skip_problems);
            }
            Console.WriteLine(separator);
            Console.WriteLine("Complete <Press any key to continue...>");
            Console.ReadKey();
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Lexical.Utils.Permutation;

namespace Lexical.Localization.Tests
{
    [TestClass]
    public class EverythingTests
    {
        public TestContext TestContext { get; set; }
        [TestMethod]
        public void Test()
        {
            // Setup
            PermutationSetup setup = new PermutationSetup();
            setup.initialParameters[nameof(TestContext)] = TestContext;

            List<Case> runCases = new List<Case>();

            // Search cases from assembly
            Type[] anchors = new Type[] { typeof(Anchor) };
            foreach (Type type in anchors.SelectMany(t => t.Assembly.GetTypes()))
            {
                Case @case = Case.ReadAnnotated(type);
                if (@case == null) continue;
                if (@case.RunFunc != null)
                    runCases.Add(@case);
                else
                    setup.Add(@case);
            }

            // Run cases
            Dictionary<Scenario, Exception> errors = new Dictionary<Scenario, Exception>();
            foreach (var scenario in setup.Scenarios)
            {
                Console.Out.WriteLine(scenario);
                Console.Out.Flush();
                try
                {
                    using (var run = scenario.Run().Initialize())
                    {
                        foreach (var runCase in runCases)
                        {
                            runCase.Initialize(run);
                            try
                            {
                                runCase.Run(run);
                            } finally
                            {
                                runCase.Cleanup(run);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"{scenario} {e.GetType().Name} {e.Message}");
                    Console.Error.Flush();
                    errors[scenario] = e;
                }
            }

            if (errors.Count == 1) throw new Exception(errors.First().Key.ToString(), errors.First().Value);
            if (errors.Count > 1) throw new AggregateException(errors.Values);
        }

    }
}

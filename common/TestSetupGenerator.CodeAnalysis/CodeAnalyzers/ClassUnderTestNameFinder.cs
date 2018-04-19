using System.Collections.Generic;

namespace TestSetupGenerator.CodeAnalysis.CodeAnalyzers
{
    public class ClassUnderTestNameFinder
    {
        public string GetClassUnderTestName(string testClassName)
        {
            if (string.IsNullOrWhiteSpace(testClassName))
            {
                return null;
            }

            var keys = new List<string>() { "UnitTest", "Test" };

            foreach (var key in keys)
            {
                var indexUnitTest = testClassName.IndexOf(key);
                if (indexUnitTest > -1)
                {
                    return testClassName.Substring(0, testClassName.Length - indexUnitTest);
                }
            }

            return testClassName;
        }
    }
}

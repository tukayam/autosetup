using System;

namespace LibraryUnderTest
{
    public interface IDependency
    {
    }

    public class Class1
    {
        private readonly IDependency _dependency;

        public Class1(IDependency dependency)
        {
            _dependency = dependency;
        }
    }
}

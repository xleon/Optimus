using System.Collections.Generic;
using NUnit.Framework;
using Optimus.Helpers;
using Shouldly;

namespace Optimus.Tests
{
    [TestFixture]
    public class ExtensionTests
    {
        [Test]
        public void Enumerable_IsNullOrEmpty()
        {
            IEnumerable<int> numbers = null;
            
            numbers.IsNullOrEmpty().ShouldBeTrue();
            new string[]{}.IsNullOrEmpty().ShouldBeTrue();
            new []{1,2}.IsNullOrEmpty().ShouldBeFalse();
        }
    }
}
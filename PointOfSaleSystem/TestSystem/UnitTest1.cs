using FlaUI.UIA3;
using FlaUI.Core.Conditions;
using FlaUI.Core;
using System.Diagnostics;

namespace TestSystem
{
    [TestClass]
    public class UnitTest1
    {
        private object window;

        [TestInitialize]


         public void Setup()

          {
                using (var automation = new UIA3Automation())
                {
                 ConditionFactory cf = new ConditionFactory(new UIA3PropertyLibrary());
                }
          }

        [TestMethod]
        public void TestMethod1()
        {

        }
    }
}
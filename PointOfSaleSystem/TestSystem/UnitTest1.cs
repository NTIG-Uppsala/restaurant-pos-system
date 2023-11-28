using FlaUI.UIA3;
using FlaUI.Core.Conditions;
using FlaUI.Core;
using System.Diagnostics;
using FlaUI.Core.AutomationElements;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Window = FlaUI.Core.AutomationElements.Window;
using System;
using System.IO;

namespace TestSystem
{
    [TestClass]
    public class UnitTest1
    {

        public Window window;
        public ConditionFactory cf;

        [TestInitialize]
        public void Setup()

        {
            using (var automation = new UIA3Automation())
            {
                var app = Application.Launch(GetSolutionFolderPath() + @"\PointOfSaleSystem\bin\Debug\net8.0-windows\PointOfSaleSystem.exe");
                window = app.GetMainWindow(automation);


                ConditionFactory cf = new ConditionFactory(new UIA3PropertyLibrary());
            }
        }

        private static string GetSolutionFolderPath()
        {
            // Assuming the solution folder is two levels above the executable
            string executablePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string solutionFolderPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(executablePath, @"..\..\..\..\.."));

            return solutionFolderPath;
        }

        [TestMethod]
        public void TestMethod1()
        {
        
        }
    }
}
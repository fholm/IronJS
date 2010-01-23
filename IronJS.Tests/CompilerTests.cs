using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace IronJS.Tests
{
    [TestClass]
    public class CompilerTests
    {
        [TestMethod]
        public void TestGenerateJQueryAST()
        {
            ScriptRunner.BuildAst(
                File.ReadAllText(
                    AppDomain.CurrentDomain.BaseDirectory + @"\Javascript\jquery-1.4.js"
                )
            );
        }

        [TestMethod]
        public void TestCompileJQuery()
        {
            ScriptRunner.Compile(
                File.ReadAllText(
                    AppDomain.CurrentDomain.BaseDirectory + @"\Javascript\jquery-1.4.js"
                )
            );
        }
    }
}

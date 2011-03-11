using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_11_Expressions_11_13_Assignment_Operators_11_13_1_Simple_Assignment : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\11_Expressions\11.13_Assignment_Operators\11.13.1_Simple_Assignment"); }
    [TestMethod] public void S11_13_1_A1_js() { RunFile(@"S11.13.1_A1.js"); }
    [TestMethod] public void S11_13_1_A2_1_T1_js() { RunFile(@"S11.13.1_A2.1_T1.js"); }
    [TestMethod] public void S11_13_1_A2_1_T2_js() { RunFile(@"S11.13.1_A2.1_T2.js"); }
    [TestMethod] public void S11_13_1_A2_1_T3_js() { RunFile(@"S11.13.1_A2.1_T3.js"); }
    [TestMethod] public void S11_13_1_A3_1_js() { RunFile(@"S11.13.1_A3.1.js"); }
    [TestMethod] public void S11_13_1_A3_2_js() { RunFile(@"S11.13.1_A3.2.js"); }
    [TestMethod] public void S11_13_1_A4_T1_js() { RunFile(@"S11.13.1_A4_T1.js"); }
    [TestMethod] public void S11_13_1_A4_T2_js() { RunFile(@"S11.13.1_A4_T2.js"); }
  }
}
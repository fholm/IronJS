using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_11_Expressions_11_14_Comma_Operator : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\11_Expressions\11.14_Comma_Operator"); }
    [TestMethod] public void S11_14_A1_js() { RunFile(@"S11.14_A1.js"); }
    [TestMethod] public void S11_14_A2_1_T1_js() { RunFile(@"S11.14_A2.1_T1.js"); }
    [TestMethod] public void S11_14_A2_1_T2_js() { RunFile(@"S11.14_A2.1_T2.js"); }
    [TestMethod] public void S11_14_A2_1_T3_js() { RunFile(@"S11.14_A2.1_T3.js"); }
    [TestMethod] public void S11_14_A3_js() { RunFile(@"S11.14_A3.js"); }
  }
}
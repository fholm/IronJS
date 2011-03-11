using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_11_Expressions_11_4_Unary_Operators_11_4_1_The_delete_Operator : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\11_Expressions\11.4_Unary_Operators\11.4.1_The_delete_Operator"); }
    [TestMethod] public void S11_4_1_A1_js() { RunFile(@"S11.4.1_A1.js"); }
    [TestMethod] public void S11_4_1_A2_1_js() { RunFile(@"S11.4.1_A2.1.js"); }
    [TestMethod] public void S11_4_1_A2_2_T1_js() { RunFile(@"S11.4.1_A2.2_T1.js"); }
    [TestMethod] public void S11_4_1_A2_2_T2_js() { RunFile(@"S11.4.1_A2.2_T2.js"); }
    [TestMethod] public void S11_4_1_A3_1_js() { RunFile(@"S11.4.1_A3.1.js"); }
    [TestMethod] public void S11_4_1_A3_2_js() { RunFile(@"S11.4.1_A3.2.js"); }
    [TestMethod] public void S11_4_1_A3_3_js() { RunFile(@"S11.4.1_A3.3.js"); }
    [TestMethod] public void S11_4_1_A4_js() { RunFile(@"S11.4.1_A4.js"); }
  }
}
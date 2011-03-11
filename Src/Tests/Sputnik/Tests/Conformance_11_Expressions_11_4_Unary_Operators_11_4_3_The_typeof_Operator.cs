using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_11_Expressions_11_4_Unary_Operators_11_4_3_The_typeof_Operator : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\11_Expressions\11.4_Unary_Operators\11.4.3_The_typeof_Operator"); }
    [TestMethod] public void S11_4_3_A1_js() { RunFile(@"S11.4.3_A1.js"); }
    [TestMethod] public void S11_4_3_A2_T1_js() { RunFile(@"S11.4.3_A2_T1.js"); }
    [TestMethod] public void S11_4_3_A2_T2_js() { RunFile(@"S11.4.3_A2_T2.js"); }
    [TestMethod] public void S11_4_3_A3_1_js() { RunFile(@"S11.4.3_A3.1.js"); }
    [TestMethod] public void S11_4_3_A3_2_js() { RunFile(@"S11.4.3_A3.2.js"); }
    [TestMethod] public void S11_4_3_A3_3_js() { RunFile(@"S11.4.3_A3.3.js"); }
    [TestMethod] public void S11_4_3_A3_4_js() { RunFile(@"S11.4.3_A3.4.js"); }
    [TestMethod] public void S11_4_3_A3_5_js() { RunFile(@"S11.4.3_A3.5.js"); }
    [TestMethod] public void S11_4_3_A3_6_js() { RunFile(@"S11.4.3_A3.6.js"); }
    [TestMethod] public void S11_4_3_A3_7_js() { RunFile(@"S11.4.3_A3.7.js"); }
  }
}
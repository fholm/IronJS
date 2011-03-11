using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_11_Expressions_11_4_Unary_Operators_11_4_7_Unary_minus_Operator : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\11_Expressions\11.4_Unary_Operators\11.4.7_Unary_minus_Operator"); }
    [TestMethod] public void S11_4_7_A1_js() { RunFile(@"S11.4.7_A1.js"); }
    [TestMethod] public void S11_4_7_A2_1_T1_js() { RunFile(@"S11.4.7_A2.1_T1.js"); }
    [TestMethod] public void S11_4_7_A2_1_T2_js() { RunFile(@"S11.4.7_A2.1_T2.js"); }
    [TestMethod] public void S11_4_7_A2_2_T1_js() { RunFile(@"S11.4.7_A2.2_T1.js"); }
    [TestMethod] public void S11_4_7_A3_T1_js() { RunFile(@"S11.4.7_A3_T1.js"); }
    [TestMethod] public void S11_4_7_A3_T2_js() { RunFile(@"S11.4.7_A3_T2.js"); }
    [TestMethod] public void S11_4_7_A3_T3_js() { RunFile(@"S11.4.7_A3_T3.js"); }
    [TestMethod] public void S11_4_7_A3_T4_js() { RunFile(@"S11.4.7_A3_T4.js"); }
    [TestMethod] public void S11_4_7_A3_T5_js() { RunFile(@"S11.4.7_A3_T5.js"); }
    [TestMethod] public void S11_4_7_A4_1_js() { RunFile(@"S11.4.7_A4.1.js"); }
    [TestMethod] public void S11_4_7_A4_2_js() { RunFile(@"S11.4.7_A4.2.js"); }
  }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_11_Expressions_11_4_Unary_Operators_11_4_2_The_void_Operator : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\11_Expressions\11.4_Unary_Operators\11.4.2_The_void_Operator"); }
    [TestMethod] public void S11_4_2_A1_js() { RunFile(@"S11.4.2_A1.js"); }
    [TestMethod] public void S11_4_2_A2_T1_js() { RunFile(@"S11.4.2_A2_T1.js"); }
    [TestMethod] public void S11_4_2_A2_T2_js() { RunFile(@"S11.4.2_A2_T2.js"); }
    [TestMethod] public void S11_4_2_A4_T1_js() { RunFile(@"S11.4.2_A4_T1.js"); }
    [TestMethod] public void S11_4_2_A4_T2_js() { RunFile(@"S11.4.2_A4_T2.js"); }
    [TestMethod] public void S11_4_2_A4_T3_js() { RunFile(@"S11.4.2_A4_T3.js"); }
    [TestMethod] public void S11_4_2_A4_T4_js() { RunFile(@"S11.4.2_A4_T4.js"); }
    [TestMethod] public void S11_4_2_A4_T5_js() { RunFile(@"S11.4.2_A4_T5.js"); }
    [TestMethod] public void S11_4_2_A4_T6_js() { RunFile(@"S11.4.2_A4_T6.js"); }
  }
}
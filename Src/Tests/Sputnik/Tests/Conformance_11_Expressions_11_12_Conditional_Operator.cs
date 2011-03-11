using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_11_Expressions_11_12_Conditional_Operator : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\11_Expressions\11.12_Conditional_Operator"); }
    [TestMethod] public void S11_12_A1_js() { RunFile(@"S11.12_A1.js"); }
    [TestMethod] public void S11_12_A2_1_T1_js() { RunFile(@"S11.12_A2.1_T1.js"); }
    [TestMethod] public void S11_12_A2_1_T2_js() { RunFile(@"S11.12_A2.1_T2.js"); }
    [TestMethod] public void S11_12_A2_1_T3_js() { RunFile(@"S11.12_A2.1_T3.js"); }
    [TestMethod] public void S11_12_A2_1_T4_js() { RunFile(@"S11.12_A2.1_T4.js"); }
    [TestMethod] public void S11_12_A2_1_T5_js() { RunFile(@"S11.12_A2.1_T5.js"); }
    [TestMethod] public void S11_12_A2_1_T6_js() { RunFile(@"S11.12_A2.1_T6.js"); }
    [TestMethod] public void S11_12_A3_T1_js() { RunFile(@"S11.12_A3_T1.js"); }
    [TestMethod] public void S11_12_A3_T2_js() { RunFile(@"S11.12_A3_T2.js"); }
    [TestMethod] public void S11_12_A3_T3_js() { RunFile(@"S11.12_A3_T3.js"); }
    [TestMethod] public void S11_12_A3_T4_js() { RunFile(@"S11.12_A3_T4.js"); }
    [TestMethod] public void S11_12_A4_T1_js() { RunFile(@"S11.12_A4_T1.js"); }
    [TestMethod] public void S11_12_A4_T2_js() { RunFile(@"S11.12_A4_T2.js"); }
    [TestMethod] public void S11_12_A4_T3_js() { RunFile(@"S11.12_A4_T3.js"); }
    [TestMethod] public void S11_12_A4_T4_js() { RunFile(@"S11.12_A4_T4.js"); }
  }
}
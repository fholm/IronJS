using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_11_Expressions_11_9_Equality_Operators_11_9_5_The_Strict_Does_not_equals_Operator : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\11_Expressions\11.9_Equality_Operators\11.9.5_The_Strict_Does_not_equals_Operator"); }
    [TestMethod] public void S11_9_5_A1_js() { RunFile(@"S11.9.5_A1.js"); }
    [TestMethod] public void S11_9_5_A2_1_T1_js() { RunFile(@"S11.9.5_A2.1_T1.js"); }
    [TestMethod] public void S11_9_5_A2_1_T2_js() { RunFile(@"S11.9.5_A2.1_T2.js"); }
    [TestMethod] public void S11_9_5_A2_1_T3_js() { RunFile(@"S11.9.5_A2.1_T3.js"); }
    [TestMethod] public void S11_9_5_A2_4_T1_js() { RunFile(@"S11.9.5_A2.4_T1.js"); }
    [TestMethod] public void S11_9_5_A2_4_T2_js() { RunFile(@"S11.9.5_A2.4_T2.js"); }
    [TestMethod] public void S11_9_5_A2_4_T3_js() { RunFile(@"S11.9.5_A2.4_T3.js"); }
    [TestMethod] public void S11_9_5_A3_js() { RunFile(@"S11.9.5_A3.js"); }
    [TestMethod] public void S11_9_5_A4_1_T1_js() { RunFile(@"S11.9.5_A4.1_T1.js"); }
    [TestMethod] public void S11_9_5_A4_1_T2_js() { RunFile(@"S11.9.5_A4.1_T2.js"); }
    [TestMethod] public void S11_9_5_A4_2_js() { RunFile(@"S11.9.5_A4.2.js"); }
    [TestMethod] public void S11_9_5_A4_3_js() { RunFile(@"S11.9.5_A4.3.js"); }
    [TestMethod] public void S11_9_5_A5_js() { RunFile(@"S11.9.5_A5.js"); }
    [TestMethod] public void S11_9_5_A6_1_js() { RunFile(@"S11.9.5_A6.1.js"); }
    [TestMethod] public void S11_9_5_A6_2_js() { RunFile(@"S11.9.5_A6.2.js"); }
    [TestMethod] public void S11_9_5_A7_js() { RunFile(@"S11.9.5_A7.js"); }
    [TestMethod] public void S11_9_5_A8_T1_js() { RunFile(@"S11.9.5_A8_T1.js"); }
    [TestMethod] public void S11_9_5_A8_T2_js() { RunFile(@"S11.9.5_A8_T2.js"); }
    [TestMethod] public void S11_9_5_A8_T3_js() { RunFile(@"S11.9.5_A8_T3.js"); }
    [TestMethod] public void S11_9_5_A8_T4_js() { RunFile(@"S11.9.5_A8_T4.js"); }
    [TestMethod] public void S11_9_5_A8_T5_js() { RunFile(@"S11.9.5_A8_T5.js"); }
  }
}
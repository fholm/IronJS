using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_11_Expressions_11_9_Equality_Operators_11_9_2_The_Does_not_equals_Operator : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\11_Expressions\11.9_Equality_Operators\11.9.2_The_Does_not_equals_Operator"); }
    [TestMethod] public void S11_9_2_A1_js() { RunFile(@"S11.9.2_A1.js"); }
    [TestMethod] public void S11_9_2_A2_1_T1_js() { RunFile(@"S11.9.2_A2.1_T1.js"); }
    [TestMethod] public void S11_9_2_A2_1_T2_js() { RunFile(@"S11.9.2_A2.1_T2.js"); }
    [TestMethod] public void S11_9_2_A2_1_T3_js() { RunFile(@"S11.9.2_A2.1_T3.js"); }
    [TestMethod] public void S11_9_2_A2_4_T1_js() { RunFile(@"S11.9.2_A2.4_T1.js"); }
    [TestMethod] public void S11_9_2_A2_4_T2_js() { RunFile(@"S11.9.2_A2.4_T2.js"); }
    [TestMethod] public void S11_9_2_A2_4_T3_js() { RunFile(@"S11.9.2_A2.4_T3.js"); }
    [TestMethod] public void S11_9_2_A3_1_js() { RunFile(@"S11.9.2_A3.1.js"); }
    [TestMethod] public void S11_9_2_A3_2_js() { RunFile(@"S11.9.2_A3.2.js"); }
    [TestMethod] public void S11_9_2_A3_3_js() { RunFile(@"S11.9.2_A3.3.js"); }
    [TestMethod] public void S11_9_2_A4_1_T1_js() { RunFile(@"S11.9.2_A4.1_T1.js"); }
    [TestMethod] public void S11_9_2_A4_1_T2_js() { RunFile(@"S11.9.2_A4.1_T2.js"); }
    [TestMethod] public void S11_9_2_A4_2_js() { RunFile(@"S11.9.2_A4.2.js"); }
    [TestMethod] public void S11_9_2_A4_3_js() { RunFile(@"S11.9.2_A4.3.js"); }
    [TestMethod] public void S11_9_2_A5_1_js() { RunFile(@"S11.9.2_A5.1.js"); }
    [TestMethod] public void S11_9_2_A5_2_js() { RunFile(@"S11.9.2_A5.2.js"); }
    [TestMethod] public void S11_9_2_A5_3_js() { RunFile(@"S11.9.2_A5.3.js"); }
    [TestMethod] public void S11_9_2_A6_1_js() { RunFile(@"S11.9.2_A6.1.js"); }
    [TestMethod] public void S11_9_2_A6_2_T1_js() { RunFile(@"S11.9.2_A6.2_T1.js"); }
    [TestMethod] public void S11_9_2_A6_2_T2_js() { RunFile(@"S11.9.2_A6.2_T2.js"); }
    [TestMethod] public void S11_9_2_A7_1_js() { RunFile(@"S11.9.2_A7.1.js"); }
    [TestMethod] public void S11_9_2_A7_2_js() { RunFile(@"S11.9.2_A7.2.js"); }
    [TestMethod] public void S11_9_2_A7_3_js() { RunFile(@"S11.9.2_A7.3.js"); }
    [TestMethod] public void S11_9_2_A7_4_js() { RunFile(@"S11.9.2_A7.4.js"); }
    [TestMethod] public void S11_9_2_A7_5_js() { RunFile(@"S11.9.2_A7.5.js"); }
    [TestMethod] public void S11_9_2_A7_6_js() { RunFile(@"S11.9.2_A7.6.js"); }
    [TestMethod] public void S11_9_2_A7_7_js() { RunFile(@"S11.9.2_A7.7.js"); }
    [TestMethod] public void S11_9_2_A7_8_js() { RunFile(@"S11.9.2_A7.8.js"); }
    [TestMethod] public void S11_9_2_A7_9_js() { RunFile(@"S11.9.2_A7.9.js"); }
  }
}
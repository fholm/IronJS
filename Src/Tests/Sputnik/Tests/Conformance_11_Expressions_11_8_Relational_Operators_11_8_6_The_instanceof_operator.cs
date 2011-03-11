using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_11_Expressions_11_8_Relational_Operators_11_8_6_The_instanceof_operator : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\11_Expressions\11.8_Relational_Operators\11.8.6_The_instanceof_operator"); }
    [TestMethod] public void S11_8_6_A1_js() { RunFile(@"S11.8.6_A1.js"); }
    [TestMethod] public void S11_8_6_A2_1_T1_js() { RunFile(@"S11.8.6_A2.1_T1.js"); }
    [TestMethod] public void S11_8_6_A2_1_T2_js() { RunFile(@"S11.8.6_A2.1_T2.js"); }
    [TestMethod] public void S11_8_6_A2_1_T3_js() { RunFile(@"S11.8.6_A2.1_T3.js"); }
    [TestMethod] public void S11_8_6_A2_4_T1_js() { RunFile(@"S11.8.6_A2.4_T1.js"); }
    [TestMethod] public void S11_8_6_A2_4_T2_js() { RunFile(@"S11.8.6_A2.4_T2.js"); }
    [TestMethod] public void S11_8_6_A2_4_T3_js() { RunFile(@"S11.8.6_A2.4_T3.js"); }
    [TestMethod] public void S11_8_6_A3_js() { RunFile(@"S11.8.6_A3.js"); }
    [TestMethod] public void S11_8_6_A4_T1_js() { RunFile(@"S11.8.6_A4_T1.js"); }
    [TestMethod] public void S11_8_6_A4_T2_js() { RunFile(@"S11.8.6_A4_T2.js"); }
    [TestMethod] public void S11_8_6_A4_T3_js() { RunFile(@"S11.8.6_A4_T3.js"); }
    [TestMethod] public void S11_8_6_A5_T1_js() { RunFile(@"S11.8.6_A5_T1.js"); }
    [TestMethod] public void S11_8_6_A5_T2_js() { RunFile(@"S11.8.6_A5_T2.js"); }
    [TestMethod] public void S11_8_6_A6_T1_js() { RunFile(@"S11.8.6_A6_T1.js"); }
    [TestMethod] public void S11_8_6_A6_T2_js() { RunFile(@"S11.8.6_A6_T2.js"); }
    [TestMethod] public void S11_8_6_A6_T3_js() { RunFile(@"S11.8.6_A6_T3.js"); }
    [TestMethod] public void S11_8_6_A6_T4_js() { RunFile(@"S11.8.6_A6_T4.js"); }
    [TestMethod] public void S11_8_6_A7_T1_js() { RunFile(@"S11.8.6_A7_T1.js"); }
    [TestMethod] public void S11_8_6_A7_T2_js() { RunFile(@"S11.8.6_A7_T2.js"); }
    [TestMethod] public void S11_8_6_A7_T3_js() { RunFile(@"S11.8.6_A7_T3.js"); }
  }
}
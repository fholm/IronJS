using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_11_Expressions_11_8_Relational_Operators_11_8_7_The_in_operator : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\11_Expressions\11.8_Relational_Operators\11.8.7_The_in_operator"); }
    [TestMethod] public void S11_8_7_A1_js() { RunFile(@"S11.8.7_A1.js"); }
    [TestMethod] public void S11_8_7_A2_1_T1_js() { RunFile(@"S11.8.7_A2.1_T1.js"); }
    [TestMethod] public void S11_8_7_A2_1_T2_js() { RunFile(@"S11.8.7_A2.1_T2.js"); }
    [TestMethod] public void S11_8_7_A2_1_T3_js() { RunFile(@"S11.8.7_A2.1_T3.js"); }
    [TestMethod] public void S11_8_7_A2_4_T1_js() { RunFile(@"S11.8.7_A2.4_T1.js"); }
    [TestMethod] public void S11_8_7_A2_4_T2_js() { RunFile(@"S11.8.7_A2.4_T2.js"); }
    [TestMethod] public void S11_8_7_A2_4_T3_js() { RunFile(@"S11.8.7_A2.4_T3.js"); }
    [TestMethod] public void S11_8_7_A3_js() { RunFile(@"S11.8.7_A3.js"); }
    [TestMethod] public void S11_8_7_A4_js() { RunFile(@"S11.8.7_A4.js"); }
  }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_11_Expressions_11_11_Binary_Logical_Operators_11_11_2_Logical_OR_Operator : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\11_Expressions\11.11_Binary_Logical_Operators\11.11.2_Logical_OR_Operator"); }
    [TestMethod] public void S11_11_2_A1_js() { RunFile(@"S11.11.2_A1.js"); }
    [TestMethod] public void S11_11_2_A2_1_T1_js() { RunFile(@"S11.11.2_A2.1_T1.js"); }
    [TestMethod] public void S11_11_2_A2_1_T2_js() { RunFile(@"S11.11.2_A2.1_T2.js"); }
    [TestMethod] public void S11_11_2_A2_1_T3_js() { RunFile(@"S11.11.2_A2.1_T3.js"); }
    [TestMethod] public void S11_11_2_A2_1_T4_js() { RunFile(@"S11.11.2_A2.1_T4.js"); }
    [TestMethod] public void S11_11_2_A2_4_T1_js() { RunFile(@"S11.11.2_A2.4_T1.js"); }
    [TestMethod] public void S11_11_2_A2_4_T2_js() { RunFile(@"S11.11.2_A2.4_T2.js"); }
    [TestMethod] public void S11_11_2_A2_4_T3_js() { RunFile(@"S11.11.2_A2.4_T3.js"); }
    [TestMethod] public void S11_11_2_A3_T1_js() { RunFile(@"S11.11.2_A3_T1.js"); }
    [TestMethod] public void S11_11_2_A3_T2_js() { RunFile(@"S11.11.2_A3_T2.js"); }
    [TestMethod] public void S11_11_2_A3_T3_js() { RunFile(@"S11.11.2_A3_T3.js"); }
    [TestMethod] public void S11_11_2_A3_T4_js() { RunFile(@"S11.11.2_A3_T4.js"); }
    [TestMethod] public void S11_11_2_A4_T1_js() { RunFile(@"S11.11.2_A4_T1.js"); }
    [TestMethod] public void S11_11_2_A4_T2_js() { RunFile(@"S11.11.2_A4_T2.js"); }
    [TestMethod] public void S11_11_2_A4_T3_js() { RunFile(@"S11.11.2_A4_T3.js"); }
    [TestMethod] public void S11_11_2_A4_T4_js() { RunFile(@"S11.11.2_A4_T4.js"); }
  }
}
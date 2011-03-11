using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_12_Statement_12_6_Iteration_Statements_12_6_1_The_do_while_Statement : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\12_Statement\12.6_Iteration_Statements\12.6.1_The_do_while_Statement"); }
    [TestMethod] public void S12_6_1_A1_js() { RunFile(@"S12.6.1_A1.js"); }
    [TestMethod] public void S12_6_1_A10_js() { RunFile(@"S12.6.1_A10.js"); }
    [TestMethod] public void S12_6_1_A11_js() { RunFile(@"S12.6.1_A11.js"); }
    [TestMethod] public void S12_6_1_A12_js() { RunFile(@"S12.6.1_A12.js"); }
    [TestMethod] public void S12_6_1_A13_T1_js() { RunFile(@"S12.6.1_A13_T1.js"); }
    [TestMethod] public void S12_6_1_A13_T2_js() { RunFile(@"S12.6.1_A13_T2.js"); }
    [TestMethod] public void S12_6_1_A13_T3_js() { RunFile(@"S12.6.1_A13_T3.js"); }
    [TestMethod] public void S12_6_1_A14_T1_js() { RunFile(@"S12.6.1_A14_T1.js"); }
    [TestMethod] public void S12_6_1_A14_T2_js() { RunFile(@"S12.6.1_A14_T2.js"); }
    [TestMethod] public void S12_6_1_A15_js() { RunFile(@"S12.6.1_A15.js"); }
    [TestMethod] public void S12_6_1_A2_js() { RunFile(@"S12.6.1_A2.js"); }
    [TestMethod] public void S12_6_1_A3_js() { RunFile(@"S12.6.1_A3.js"); }
    [TestMethod] public void S12_6_1_A4_T1_js() { RunFile(@"S12.6.1_A4_T1.js"); }
    [TestMethod] public void S12_6_1_A4_T2_js() { RunFile(@"S12.6.1_A4_T2.js"); }
    [TestMethod] public void S12_6_1_A4_T3_js() { RunFile(@"S12.6.1_A4_T3.js"); }
    [TestMethod] public void S12_6_1_A4_T4_js() { RunFile(@"S12.6.1_A4_T4.js"); }
    [TestMethod] public void S12_6_1_A4_T5_js() { RunFile(@"S12.6.1_A4_T5.js"); }
    [TestMethod] public void S12_6_1_A5_js() { RunFile(@"S12.6.1_A5.js"); }
    [TestMethod] public void S12_6_1_A6_T1_js() { RunFile(@"S12.6.1_A6_T1.js"); }
    [TestMethod] public void S12_6_1_A6_T2_js() { RunFile(@"S12.6.1_A6_T2.js"); }
    [TestMethod] public void S12_6_1_A6_T3_js() { RunFile(@"S12.6.1_A6_T3.js"); }
    [TestMethod] public void S12_6_1_A6_T4_js() { RunFile(@"S12.6.1_A6_T4.js"); }
    [TestMethod] public void S12_6_1_A6_T5_js() { RunFile(@"S12.6.1_A6_T5.js"); }
    [TestMethod] public void S12_6_1_A6_T6_js() { RunFile(@"S12.6.1_A6_T6.js"); }
    [TestMethod] public void S12_6_1_A7_js() { RunFile(@"S12.6.1_A7.js"); }
    [TestMethod] public void S12_6_1_A8_js() { RunFile(@"S12.6.1_A8.js"); }
    [TestMethod] public void S12_6_1_A9_js() { RunFile(@"S12.6.1_A9.js"); }
  }
}
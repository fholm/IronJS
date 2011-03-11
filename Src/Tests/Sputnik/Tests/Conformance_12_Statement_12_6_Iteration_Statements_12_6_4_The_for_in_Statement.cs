using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_12_Statement_12_6_Iteration_Statements_12_6_4_The_for_in_Statement : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\12_Statement\12.6_Iteration_Statements\12.6.4_The_for_in_Statement"); }
    [TestMethod] public void S12_6_4_A1_js() { RunFile(@"S12.6.4_A1.js"); }
    [TestMethod] public void S12_6_4_A13_T1_js() { RunFile(@"S12.6.4_A13_T1.js"); }
    [TestMethod] public void S12_6_4_A13_T2_js() { RunFile(@"S12.6.4_A13_T2.js"); }
    [TestMethod] public void S12_6_4_A13_T3_js() { RunFile(@"S12.6.4_A13_T3.js"); }
    [TestMethod] public void S12_6_4_A14_T1_js() { RunFile(@"S12.6.4_A14_T1.js"); }
    [TestMethod] public void S12_6_4_A14_T2_js() { RunFile(@"S12.6.4_A14_T2.js"); }
    [TestMethod] public void S12_6_4_A15_js() { RunFile(@"S12.6.4_A15.js"); }
    [TestMethod] public void S12_6_4_A2_js() { RunFile(@"S12.6.4_A2.js"); }
    [TestMethod] public void S12_6_4_A3_1_js() { RunFile(@"S12.6.4_A3.1.js"); }
    [TestMethod] public void S12_6_4_A3_js() { RunFile(@"S12.6.4_A3.js"); }
    [TestMethod] public void S12_6_4_A4_1_js() { RunFile(@"S12.6.4_A4.1.js"); }
    [TestMethod] public void S12_6_4_A4_js() { RunFile(@"S12.6.4_A4.js"); }
    [TestMethod] public void S12_6_4_A5_1_js() { RunFile(@"S12.6.4_A5.1.js"); }
    [TestMethod] public void S12_6_4_A5_js() { RunFile(@"S12.6.4_A5.js"); }
    [TestMethod] public void S12_6_4_A6_1_js() { RunFile(@"S12.6.4_A6.1.js"); }
    [TestMethod] public void S12_6_4_A6_js() { RunFile(@"S12.6.4_A6.js"); }
    [TestMethod] public void S12_6_4_A7_T1_js() { RunFile(@"S12.6.4_A7_T1.js"); }
    [TestMethod] public void S12_6_4_A7_T2_js() { RunFile(@"S12.6.4_A7_T2.js"); }
  }
}
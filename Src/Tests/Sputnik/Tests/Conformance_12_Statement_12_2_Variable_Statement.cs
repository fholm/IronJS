using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_12_Statement_12_2_Variable_Statement : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\12_Statement\12.2_Variable_Statement"); }
    [TestMethod] public void S12_2_A1_js() { RunFile(@"S12.2_A1.js"); }
    [TestMethod] public void S12_2_A10_js() { RunFile(@"S12.2_A10.js"); }
    [TestMethod] public void S12_2_A11_js() { RunFile(@"S12.2_A11.js"); }
    [TestMethod] public void S12_2_A12_js() { RunFile(@"S12.2_A12.js"); }
    [TestMethod] public void S12_2_A2_js() { RunFile(@"S12.2_A2.js"); }
    [TestMethod] public void S12_2_A3_js() { RunFile(@"S12.2_A3.js"); }
    [TestMethod] public void S12_2_A4_js() { RunFile(@"S12.2_A4.js"); }
    [TestMethod] public void S12_2_A5_js() { RunFile(@"S12.2_A5.js"); }
    [TestMethod] public void S12_2_A6_T1_js() { RunFile(@"S12.2_A6_T1.js"); }
    [TestMethod] public void S12_2_A6_T2_js() { RunFile(@"S12.2_A6_T2.js"); }
    [TestMethod] public void S12_2_A7_js() { RunFile(@"S12.2_A7.js"); }
    [TestMethod] public void S12_2_A8_T1_js() { RunFile(@"S12.2_A8_T1.js"); }
    [TestMethod] public void S12_2_A8_T2_js() { RunFile(@"S12.2_A8_T2.js"); }
    [TestMethod] public void S12_2_A8_T3_js() { RunFile(@"S12.2_A8_T3.js"); }
    [TestMethod] public void S12_2_A8_T4_js() { RunFile(@"S12.2_A8_T4.js"); }
    [TestMethod] public void S12_2_A8_T5_js() { RunFile(@"S12.2_A8_T5.js"); }
    [TestMethod] public void S12_2_A8_T6_js() { RunFile(@"S12.2_A8_T6.js"); }
    [TestMethod] public void S12_2_A8_T7_js() { RunFile(@"S12.2_A8_T7.js"); }
    [TestMethod] public void S12_2_A8_T8_js() { RunFile(@"S12.2_A8_T8.js"); }
    [TestMethod] public void S12_2_A9_js() { RunFile(@"S12.2_A9.js"); }
  }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_12_Statement_12_7_The_continue_Statement : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\12_Statement\12.7_The_continue_Statement"); }
    [TestMethod] public void S12_7_A1_T1_js() { RunFile(@"S12.7_A1_T1.js"); }
    [TestMethod] public void S12_7_A1_T2_js() { RunFile(@"S12.7_A1_T2.js"); }
    [TestMethod] public void S12_7_A1_T3_js() { RunFile(@"S12.7_A1_T3.js"); }
    [TestMethod] public void S12_7_A1_T4_js() { RunFile(@"S12.7_A1_T4.js"); }
    [TestMethod] public void S12_7_A2_js() { RunFile(@"S12.7_A2.js"); }
    [TestMethod] public void S12_7_A3_js() { RunFile(@"S12.7_A3.js"); }
    [TestMethod] public void S12_7_A4_T1_js() { RunFile(@"S12.7_A4_T1.js"); }
    [TestMethod] public void S12_7_A4_T2_js() { RunFile(@"S12.7_A4_T2.js"); }
    [TestMethod] public void S12_7_A4_T3_js() { RunFile(@"S12.7_A4_T3.js"); }
    [TestMethod] public void S12_7_A5_T1_js() { RunFile(@"S12.7_A5_T1.js"); }
    [TestMethod] public void S12_7_A5_T2_js() { RunFile(@"S12.7_A5_T2.js"); }
    [TestMethod] public void S12_7_A5_T3_js() { RunFile(@"S12.7_A5_T3.js"); }
    [TestMethod] public void S12_7_A6_js() { RunFile(@"S12.7_A6.js"); }
    [TestMethod] public void S12_7_A7_js() { RunFile(@"S12.7_A7.js"); }
    [TestMethod] public void S12_7_A8_T1_js() { RunFile(@"S12.7_A8_T1.js"); }
    [TestMethod] public void S12_7_A8_T2_js() { RunFile(@"S12.7_A8_T2.js"); }
    [TestMethod] public void S12_7_A9_T1_js() { RunFile(@"S12.7_A9_T1.js"); }
    [TestMethod] public void S12_7_A9_T2_js() { RunFile(@"S12.7_A9_T2.js"); }
  }
}
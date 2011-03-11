using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_12_Statement_12_8_The_break_Statement : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\12_Statement\12.8_The_break_Statement"); }
    [TestMethod] public void S12_8_A1_T1_js() { RunFile(@"S12.8_A1_T1.js"); }
    [TestMethod] public void S12_8_A1_T2_js() { RunFile(@"S12.8_A1_T2.js"); }
    [TestMethod] public void S12_8_A1_T3_js() { RunFile(@"S12.8_A1_T3.js"); }
    [TestMethod] public void S12_8_A1_T4_js() { RunFile(@"S12.8_A1_T4.js"); }
    [TestMethod] public void S12_8_A2_js() { RunFile(@"S12.8_A2.js"); }
    [TestMethod] public void S12_8_A3_js() { RunFile(@"S12.8_A3.js"); }
    [TestMethod] public void S12_8_A4_T1_js() { RunFile(@"S12.8_A4_T1.js"); }
    [TestMethod] public void S12_8_A4_T2_js() { RunFile(@"S12.8_A4_T2.js"); }
    [TestMethod] public void S12_8_A4_T3_js() { RunFile(@"S12.8_A4_T3.js"); }
    [TestMethod] public void S12_8_A5_T1_js() { RunFile(@"S12.8_A5_T1.js"); }
    [TestMethod] public void S12_8_A5_T2_js() { RunFile(@"S12.8_A5_T2.js"); }
    [TestMethod] public void S12_8_A5_T3_js() { RunFile(@"S12.8_A5_T3.js"); }
    [TestMethod] public void S12_8_A6_js() { RunFile(@"S12.8_A6.js"); }
    [TestMethod] public void S12_8_A7_js() { RunFile(@"S12.8_A7.js"); }
    [TestMethod] public void S12_8_A8_T1_js() { RunFile(@"S12.8_A8_T1.js"); }
    [TestMethod] public void S12_8_A8_T2_js() { RunFile(@"S12.8_A8_T2.js"); }
    [TestMethod] public void S12_8_A9_T1_js() { RunFile(@"S12.8_A9_T1.js"); }
    [TestMethod] public void S12_8_A9_T2_js() { RunFile(@"S12.8_A9_T2.js"); }
  }
}
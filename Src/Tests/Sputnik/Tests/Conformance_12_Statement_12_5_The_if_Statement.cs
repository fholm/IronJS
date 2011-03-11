using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_12_Statement_12_5_The_if_Statement : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\12_Statement\12.5_The_if_Statement"); }
    [TestMethod] public void S12_5_A1_1_T1_js() { RunFile(@"S12.5_A1.1_T1.js"); }
    [TestMethod] public void S12_5_A1_1_T2_js() { RunFile(@"S12.5_A1.1_T2.js"); }
    [TestMethod] public void S12_5_A1_2_T1_js() { RunFile(@"S12.5_A1.2_T1.js"); }
    [TestMethod] public void S12_5_A1_2_T2_js() { RunFile(@"S12.5_A1.2_T2.js"); }
    [TestMethod] public void S12_5_A10_T1_js() { RunFile(@"S12.5_A10_T1.js"); }
    [TestMethod] public void S12_5_A10_T2_js() { RunFile(@"S12.5_A10_T2.js"); }
    [TestMethod] public void S12_5_A11_js() { RunFile(@"S12.5_A11.js"); }
    [TestMethod] public void S12_5_A12_T1_js() { RunFile(@"S12.5_A12_T1.js"); }
    [TestMethod] public void S12_5_A12_T2_js() { RunFile(@"S12.5_A12_T2.js"); }
    [TestMethod] public void S12_5_A12_T3_js() { RunFile(@"S12.5_A12_T3.js"); }
    [TestMethod] public void S12_5_A12_T4_js() { RunFile(@"S12.5_A12_T4.js"); }
    [TestMethod] public void S12_5_A1_T1_js() { RunFile(@"S12.5_A1_T1.js"); }
    [TestMethod] public void S12_5_A1_T2_js() { RunFile(@"S12.5_A1_T2.js"); }
    [TestMethod] public void S12_5_A2_js() { RunFile(@"S12.5_A2.js"); }
    [TestMethod] public void S12_5_A3_js() { RunFile(@"S12.5_A3.js"); }
    [TestMethod] public void S12_5_A4_js() { RunFile(@"S12.5_A4.js"); }
    [TestMethod] public void S12_5_A5_js() { RunFile(@"S12.5_A5.js"); }
    [TestMethod] public void S12_5_A6_T1_js() { RunFile(@"S12.5_A6_T1.js"); }
    [TestMethod] public void S12_5_A6_T2_js() { RunFile(@"S12.5_A6_T2.js"); }
    [TestMethod] public void S12_5_A7_js() { RunFile(@"S12.5_A7.js"); }
    [TestMethod] public void S12_5_A8_js() { RunFile(@"S12.5_A8.js"); }
    [TestMethod] public void S12_5_A9_T1_js() { RunFile(@"S12.5_A9_T1.js"); }
    [TestMethod] public void S12_5_A9_T2_js() { RunFile(@"S12.5_A9_T2.js"); }
    [TestMethod] public void S12_5_A9_T3_js() { RunFile(@"S12.5_A9_T3.js"); }
  }
}
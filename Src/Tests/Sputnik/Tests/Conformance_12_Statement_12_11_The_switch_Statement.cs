using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_12_Statement_12_11_The_switch_Statement : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\12_Statement\12.11_The_switch_Statement"); }
    [TestMethod] public void S12_11_A1_T1_js() { RunFile(@"S12.11_A1_T1.js"); }
    [TestMethod] public void S12_11_A1_T2_js() { RunFile(@"S12.11_A1_T2.js"); }
    [TestMethod] public void S12_11_A1_T3_js() { RunFile(@"S12.11_A1_T3.js"); }
    [TestMethod] public void S12_11_A1_T4_js() { RunFile(@"S12.11_A1_T4.js"); }
    [TestMethod] public void S12_11_A2_T1_js() { RunFile(@"S12.11_A2_T1.js"); }
    [TestMethod] public void S12_11_A3_T1_js() { RunFile(@"S12.11_A3_T1.js"); }
    [TestMethod] public void S12_11_A3_T2_js() { RunFile(@"S12.11_A3_T2.js"); }
    [TestMethod] public void S12_11_A3_T3_js() { RunFile(@"S12.11_A3_T3.js"); }
    [TestMethod] public void S12_11_A3_T4_js() { RunFile(@"S12.11_A3_T4.js"); }
    [TestMethod] public void S12_11_A3_T5_js() { RunFile(@"S12.11_A3_T5.js"); }
    [TestMethod] public void S12_11_A4_T1_js() { RunFile(@"S12.11_A4_T1.js"); }
  }
}
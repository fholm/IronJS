using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_12_Statement_12_13_The_throw_statement : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\12_Statement\12.13_The_throw_statement"); }
    [TestMethod] public void S12_13_A1_js() { RunFile(@"S12.13_A1.js"); }
    [TestMethod] public void S12_13_A2_T1_js() { RunFile(@"S12.13_A2_T1.js"); }
    [TestMethod] public void S12_13_A2_T2_js() { RunFile(@"S12.13_A2_T2.js"); }
    [TestMethod] public void S12_13_A2_T3_js() { RunFile(@"S12.13_A2_T3.js"); }
    [TestMethod] public void S12_13_A2_T4_js() { RunFile(@"S12.13_A2_T4.js"); }
    [TestMethod] public void S12_13_A2_T5_js() { RunFile(@"S12.13_A2_T5.js"); }
    [TestMethod] public void S12_13_A2_T6_js() { RunFile(@"S12.13_A2_T6.js"); }
    [TestMethod] public void S12_13_A2_T7_js() { RunFile(@"S12.13_A2_T7.js"); }
    [TestMethod] public void S12_13_A3_T1_js() { RunFile(@"S12.13_A3_T1.js"); }
    [TestMethod] public void S12_13_A3_T2_js() { RunFile(@"S12.13_A3_T2.js"); }
    [TestMethod] public void S12_13_A3_T3_js() { RunFile(@"S12.13_A3_T3.js"); }
    [TestMethod] public void S12_13_A3_T4_js() { RunFile(@"S12.13_A3_T4.js"); }
    [TestMethod] public void S12_13_A3_T5_js() { RunFile(@"S12.13_A3_T5.js"); }
    [TestMethod] public void S12_13_A3_T6_js() { RunFile(@"S12.13_A3_T6.js"); }
  }
}
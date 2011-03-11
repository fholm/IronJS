using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_12_Statement_12_9_The_return_Statement : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\12_Statement\12.9_The_return_Statement"); }
    [TestMethod] public void S12_9_A1_T1_js() { RunFile(@"S12.9_A1_T1.js"); }
    [TestMethod] public void S12_9_A1_T10_js() { RunFile(@"S12.9_A1_T10.js"); }
    [TestMethod] public void S12_9_A1_T2_js() { RunFile(@"S12.9_A1_T2.js"); }
    [TestMethod] public void S12_9_A1_T3_js() { RunFile(@"S12.9_A1_T3.js"); }
    [TestMethod] public void S12_9_A1_T4_js() { RunFile(@"S12.9_A1_T4.js"); }
    [TestMethod] public void S12_9_A1_T5_js() { RunFile(@"S12.9_A1_T5.js"); }
    [TestMethod] public void S12_9_A1_T6_js() { RunFile(@"S12.9_A1_T6.js"); }
    [TestMethod] public void S12_9_A1_T7_js() { RunFile(@"S12.9_A1_T7.js"); }
    [TestMethod] public void S12_9_A1_T8_js() { RunFile(@"S12.9_A1_T8.js"); }
    [TestMethod] public void S12_9_A1_T9_js() { RunFile(@"S12.9_A1_T9.js"); }
    [TestMethod] public void S12_9_A2_js() { RunFile(@"S12.9_A2.js"); }
    [TestMethod] public void S12_9_A3_js() { RunFile(@"S12.9_A3.js"); }
    [TestMethod] public void S12_9_A4_js() { RunFile(@"S12.9_A4.js"); }
    [TestMethod] public void S12_9_A5_js() { RunFile(@"S12.9_A5.js"); }
  }
}
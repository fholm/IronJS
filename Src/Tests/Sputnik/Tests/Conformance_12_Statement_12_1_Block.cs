using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_12_Statement_12_1_Block : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\12_Statement\12.1_Block"); }
    [TestMethod] public void S12_1_A1_js() { RunFile(@"S12.1_A1.js"); }
    [TestMethod] public void S12_1_A2_js() { RunFile(@"S12.1_A2.js"); }
    [TestMethod] public void S12_1_A4_T1_js() { RunFile(@"S12.1_A4_T1.js"); }
    [TestMethod] public void S12_1_A4_T2_js() { RunFile(@"S12.1_A4_T2.js"); }
    [TestMethod] public void S12_1_A5_js() { RunFile(@"S12.1_A5.js"); }
  }
}
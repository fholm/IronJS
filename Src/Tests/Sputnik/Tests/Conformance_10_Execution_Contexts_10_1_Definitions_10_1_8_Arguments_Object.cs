using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_10_Execution_Contexts_10_1_Definitions_10_1_8_Arguments_Object : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\10_Execution_Contexts\10.1_Definitions\10.1.8_Arguments_Object"); }
    [TestMethod] public void S10_1_8_A1_js() { RunFile(@"S10.1.8_A1.js"); }
    [TestMethod] public void S10_1_8_A2_js() { RunFile(@"S10.1.8_A2.js"); }
    [TestMethod] public void S10_1_8_A3_T1_js() { RunFile(@"S10.1.8_A3_T1.js"); }
    [TestMethod] public void S10_1_8_A3_T2_js() { RunFile(@"S10.1.8_A3_T2.js"); }
    [TestMethod] public void S10_1_8_A3_T3_js() { RunFile(@"S10.1.8_A3_T3.js"); }
    [TestMethod] public void S10_1_8_A3_T4_js() { RunFile(@"S10.1.8_A3_T4.js"); }
    [TestMethod] public void S10_1_8_A4_js() { RunFile(@"S10.1.8_A4.js"); }
    [TestMethod] public void S10_1_8_A5_T1_js() { RunFile(@"S10.1.8_A5_T1.js"); }
    [TestMethod] public void S10_1_8_A5_T2_js() { RunFile(@"S10.1.8_A5_T2.js"); }
    [TestMethod] public void S10_1_8_A5_T3_js() { RunFile(@"S10.1.8_A5_T3.js"); }
    [TestMethod] public void S10_1_8_A5_T4_js() { RunFile(@"S10.1.8_A5_T4.js"); }
    [TestMethod] public void S10_1_8_A6_js() { RunFile(@"S10.1.8_A6.js"); }
    [TestMethod] public void S10_1_8_A7_js() { RunFile(@"S10.1.8_A7.js"); }
  }
}
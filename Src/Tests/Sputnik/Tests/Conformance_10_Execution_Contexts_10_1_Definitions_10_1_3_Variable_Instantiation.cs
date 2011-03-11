using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_10_Execution_Contexts_10_1_Definitions_10_1_3_Variable_Instantiation : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\10_Execution_Contexts\10.1_Definitions\10.1.3_Variable_Instantiation"); }
    [TestMethod] public void S10_1_3_A1_js() { RunFile(@"S10.1.3_A1.js"); }
    [TestMethod] public void S10_1_3_A2_js() { RunFile(@"S10.1.3_A2.js"); }
    [TestMethod] public void S10_1_3_A3_js() { RunFile(@"S10.1.3_A3.js"); }
    [TestMethod] public void S10_1_3_A4_T1_js() { RunFile(@"S10.1.3_A4_T1.js"); }
    [TestMethod] public void S10_1_3_A4_T2_js() { RunFile(@"S10.1.3_A4_T2.js"); }
    [TestMethod] public void S10_1_3_A5_1_T1_js() { RunFile(@"S10.1.3_A5.1_T1.js"); }
    [TestMethod] public void S10_1_3_A5_1_T2_js() { RunFile(@"S10.1.3_A5.1_T2.js"); }
    [TestMethod] public void S10_1_3_A5_2_T1_js() { RunFile(@"S10.1.3_A5.2_T1.js"); }
  }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_10_Execution_Contexts_10_1_Definitions : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\10_Execution_Contexts\10.1_Definitions"); }
    [TestMethod] public void S10_1_1_A1_T1_js() { RunFile(@"S10.1.1_A1_T1.js"); }
    [TestMethod] public void S10_1_1_A1_T2_js() { RunFile(@"S10.1.1_A1_T2.js"); }
    [TestMethod] public void S10_1_1_A1_T3_js() { RunFile(@"S10.1.1_A1_T3.js"); }
    [TestMethod] public void S10_1_1_A2_T1_js() { RunFile(@"S10.1.1_A2_T1.js"); }
    [TestMethod] public void S10_1_6_A1_T1_js() { RunFile(@"S10.1.6_A1_T1.js"); }
    [TestMethod] public void S10_1_6_A1_T2_js() { RunFile(@"S10.1.6_A1_T2.js"); }
    [TestMethod] public void S10_1_6_A1_T3_js() { RunFile(@"S10.1.6_A1_T3.js"); }
    [TestMethod] public void S10_1_7_A1_T1_js() { RunFile(@"S10.1.7_A1_T1.js"); }
  }
}
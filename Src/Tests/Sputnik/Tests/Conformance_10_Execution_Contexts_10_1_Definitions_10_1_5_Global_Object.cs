using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_10_Execution_Contexts_10_1_Definitions_10_1_5_Global_Object : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\10_Execution_Contexts\10.1_Definitions\10.1.5_Global_Object"); }
    [TestMethod] public void S10_1_5_A1_1_T1_js() { RunFile(@"S10.1.5_A1.1_T1.js"); }
    [TestMethod] public void S10_1_5_A1_1_T2_js() { RunFile(@"S10.1.5_A1.1_T2.js"); }
    [TestMethod] public void S10_1_5_A1_1_T3_js() { RunFile(@"S10.1.5_A1.1_T3.js"); }
    [TestMethod] public void S10_1_5_A1_1_T4_js() { RunFile(@"S10.1.5_A1.1_T4.js"); }
    [TestMethod] public void S10_1_5_A1_2_T1_js() { RunFile(@"S10.1.5_A1.2_T1.js"); }
    [TestMethod] public void S10_1_5_A1_2_T2_js() { RunFile(@"S10.1.5_A1.2_T2.js"); }
    [TestMethod] public void S10_1_5_A1_2_T3_js() { RunFile(@"S10.1.5_A1.2_T3.js"); }
    [TestMethod] public void S10_1_5_A1_2_T4_js() { RunFile(@"S10.1.5_A1.2_T4.js"); }
    [TestMethod] public void S10_1_5_A1_3_T1_js() { RunFile(@"S10.1.5_A1.3_T1.js"); }
    [TestMethod] public void S10_1_5_A1_3_T2_js() { RunFile(@"S10.1.5_A1.3_T2.js"); }
    [TestMethod] public void S10_1_5_A1_3_T3_js() { RunFile(@"S10.1.5_A1.3_T3.js"); }
    [TestMethod] public void S10_1_5_A1_3_T4_js() { RunFile(@"S10.1.5_A1.3_T4.js"); }
    [TestMethod] public void S10_1_5_A2_1_T1_js() { RunFile(@"S10.1.5_A2.1_T1.js"); }
    [TestMethod] public void S10_1_5_A2_1_T2_js() { RunFile(@"S10.1.5_A2.1_T2.js"); }
    [TestMethod] public void S10_1_5_A2_1_T3_js() { RunFile(@"S10.1.5_A2.1_T3.js"); }
    [TestMethod] public void S10_1_5_A2_1_T4_js() { RunFile(@"S10.1.5_A2.1_T4.js"); }
    [TestMethod] public void S10_1_5_A2_2_T1_js() { RunFile(@"S10.1.5_A2.2_T1.js"); }
    [TestMethod] public void S10_1_5_A2_2_T2_js() { RunFile(@"S10.1.5_A2.2_T2.js"); }
    [TestMethod] public void S10_1_5_A2_2_T3_js() { RunFile(@"S10.1.5_A2.2_T3.js"); }
    [TestMethod] public void S10_1_5_A2_2_T4_js() { RunFile(@"S10.1.5_A2.2_T4.js"); }
    [TestMethod] public void S10_1_5_A2_3_T1_js() { RunFile(@"S10.1.5_A2.3_T1.js"); }
    [TestMethod] public void S10_1_5_A2_3_T2_js() { RunFile(@"S10.1.5_A2.3_T2.js"); }
    [TestMethod] public void S10_1_5_A2_3_T3_js() { RunFile(@"S10.1.5_A2.3_T3.js"); }
    [TestMethod] public void S10_1_5_A2_3_T4_js() { RunFile(@"S10.1.5_A2.3_T4.js"); }
  }
}
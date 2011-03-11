using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_10_Execution_Contexts_10_2_Entering_An_Execution_Context_10_2_2_Eval_Code : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\10_Execution_Contexts\10.2_Entering_An_Execution_Context\10.2.2_Eval_Code"); }
    [TestMethod] public void S10_2_2_A1_1_T1_js() { RunFile(@"S10.2.2_A1.1_T1.js"); }
    [TestMethod] public void S10_2_2_A1_1_T10_js() { RunFile(@"S10.2.2_A1.1_T10.js"); }
    [TestMethod] public void S10_2_2_A1_1_T11_js() { RunFile(@"S10.2.2_A1.1_T11.js"); }
    [TestMethod] public void S10_2_2_A1_1_T2_js() { RunFile(@"S10.2.2_A1.1_T2.js"); }
    [TestMethod] public void S10_2_2_A1_1_T3_js() { RunFile(@"S10.2.2_A1.1_T3.js"); }
    [TestMethod] public void S10_2_2_A1_1_T4_js() { RunFile(@"S10.2.2_A1.1_T4.js"); }
    [TestMethod] public void S10_2_2_A1_1_T5_js() { RunFile(@"S10.2.2_A1.1_T5.js"); }
    [TestMethod] public void S10_2_2_A1_1_T6_js() { RunFile(@"S10.2.2_A1.1_T6.js"); }
    [TestMethod] public void S10_2_2_A1_1_T7_js() { RunFile(@"S10.2.2_A1.1_T7.js"); }
    [TestMethod] public void S10_2_2_A1_1_T8_js() { RunFile(@"S10.2.2_A1.1_T8.js"); }
    [TestMethod] public void S10_2_2_A1_1_T9_js() { RunFile(@"S10.2.2_A1.1_T9.js"); }
    [TestMethod] public void S10_2_2_A1_2_T1_js() { RunFile(@"S10.2.2_A1.2_T1.js"); }
    [TestMethod] public void S10_2_2_A1_2_T10_js() { RunFile(@"S10.2.2_A1.2_T10.js"); }
    [TestMethod] public void S10_2_2_A1_2_T11_js() { RunFile(@"S10.2.2_A1.2_T11.js"); }
    [TestMethod] public void S10_2_2_A1_2_T2_js() { RunFile(@"S10.2.2_A1.2_T2.js"); }
    [TestMethod] public void S10_2_2_A1_2_T3_js() { RunFile(@"S10.2.2_A1.2_T3.js"); }
    [TestMethod] public void S10_2_2_A1_2_T4_js() { RunFile(@"S10.2.2_A1.2_T4.js"); }
    [TestMethod] public void S10_2_2_A1_2_T5_js() { RunFile(@"S10.2.2_A1.2_T5.js"); }
    [TestMethod] public void S10_2_2_A1_2_T6_js() { RunFile(@"S10.2.2_A1.2_T6.js"); }
    [TestMethod] public void S10_2_2_A1_2_T7_js() { RunFile(@"S10.2.2_A1.2_T7.js"); }
    [TestMethod] public void S10_2_2_A1_2_T8_js() { RunFile(@"S10.2.2_A1.2_T8.js"); }
    [TestMethod] public void S10_2_2_A1_2_T9_js() { RunFile(@"S10.2.2_A1.2_T9.js"); }
  }
}
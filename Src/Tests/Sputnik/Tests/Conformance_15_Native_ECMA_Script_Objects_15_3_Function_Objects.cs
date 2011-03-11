using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_3_Function_Objects : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.3_Function_Objects"); }
    [TestMethod] public void S15_3_1_A1_T1_js() { RunFile(@"S15.3.1_A1_T1.js"); }
    [TestMethod] public void S15_3_A1_js() { RunFile(@"S15.3_A1.js"); }
    [TestMethod] public void S15_3_A2_T1_js() { RunFile(@"S15.3_A2_T1.js"); }
    [TestMethod] public void S15_3_A2_T2_js() { RunFile(@"S15.3_A2_T2.js"); }
    [TestMethod] public void S15_3_A3_T1_js() { RunFile(@"S15.3_A3_T1.js"); }
    [TestMethod] public void S15_3_A3_T2_js() { RunFile(@"S15.3_A3_T2.js"); }
    [TestMethod] public void S15_3_A3_T3_js() { RunFile(@"S15.3_A3_T3.js"); }
    [TestMethod] public void S15_3_A3_T4_js() { RunFile(@"S15.3_A3_T4.js"); }
    [TestMethod] public void S15_3_A3_T5_js() { RunFile(@"S15.3_A3_T5.js"); }
    [TestMethod] public void S15_3_A3_T6_js() { RunFile(@"S15.3_A3_T6.js"); }
  }
}
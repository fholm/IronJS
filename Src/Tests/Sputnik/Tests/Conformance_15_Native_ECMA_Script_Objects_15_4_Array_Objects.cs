using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_4_Array_Objects : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.4_Array_Objects"); }
    [TestMethod] public void S15_4_A1_1_T1_js() { RunFile(@"S15.4_A1.1_T1.js"); }
    [TestMethod] public void S15_4_A1_1_T10_js() { RunFile(@"S15.4_A1.1_T10.js"); }
    [TestMethod] public void S15_4_A1_1_T2_js() { RunFile(@"S15.4_A1.1_T2.js"); }
    [TestMethod] public void S15_4_A1_1_T3_js() { RunFile(@"S15.4_A1.1_T3.js"); }
    [TestMethod] public void S15_4_A1_1_T4_js() { RunFile(@"S15.4_A1.1_T4.js"); }
    [TestMethod] public void S15_4_A1_1_T5_js() { RunFile(@"S15.4_A1.1_T5.js"); }
    [TestMethod] public void S15_4_A1_1_T6_js() { RunFile(@"S15.4_A1.1_T6.js"); }
    [TestMethod] public void S15_4_A1_1_T7_js() { RunFile(@"S15.4_A1.1_T7.js"); }
    [TestMethod] public void S15_4_A1_1_T8_js() { RunFile(@"S15.4_A1.1_T8.js"); }
    [TestMethod] public void S15_4_A1_1_T9_js() { RunFile(@"S15.4_A1.1_T9.js"); }
  }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_4_Array_Objects_15_4_2_The_Array_Constructor_15_4_2_2_new_Array_len : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.4_Array_Objects\15.4.2_The_Array_Constructor\15.4.2.2_new_Array_len"); }
    [TestMethod] public void S15_4_2_2_A1_1_T1_js() { RunFile(@"S15.4.2.2_A1.1_T1.js"); }
    [TestMethod] public void S15_4_2_2_A1_1_T2_js() { RunFile(@"S15.4.2.2_A1.1_T2.js"); }
    [TestMethod] public void S15_4_2_2_A1_1_T3_js() { RunFile(@"S15.4.2.2_A1.1_T3.js"); }
    [TestMethod] public void S15_4_2_2_A1_2_T1_js() { RunFile(@"S15.4.2.2_A1.2_T1.js"); }
    [TestMethod] public void S15_4_2_2_A2_1_T1_js() { RunFile(@"S15.4.2.2_A2.1_T1.js"); }
    [TestMethod] public void S15_4_2_2_A2_2_T1_js() { RunFile(@"S15.4.2.2_A2.2_T1.js"); }
    [TestMethod] public void S15_4_2_2_A2_2_T2_js() { RunFile(@"S15.4.2.2_A2.2_T2.js"); }
    [TestMethod] public void S15_4_2_2_A2_2_T3_js() { RunFile(@"S15.4.2.2_A2.2_T3.js"); }
    [TestMethod] public void S15_4_2_2_A2_3_T1_js() { RunFile(@"S15.4.2.2_A2.3_T1.js"); }
    [TestMethod] public void S15_4_2_2_A2_3_T2_js() { RunFile(@"S15.4.2.2_A2.3_T2.js"); }
    [TestMethod] public void S15_4_2_2_A2_3_T3_js() { RunFile(@"S15.4.2.2_A2.3_T3.js"); }
    [TestMethod] public void S15_4_2_2_A2_3_T4_js() { RunFile(@"S15.4.2.2_A2.3_T4.js"); }
    [TestMethod] public void S15_4_2_2_A2_3_T5_js() { RunFile(@"S15.4.2.2_A2.3_T5.js"); }
  }
}
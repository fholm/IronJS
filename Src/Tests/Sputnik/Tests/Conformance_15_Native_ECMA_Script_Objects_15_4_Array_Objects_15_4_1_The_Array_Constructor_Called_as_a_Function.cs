using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_4_Array_Objects_15_4_1_The_Array_Constructor_Called_as_a_Function : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.4_Array_Objects\15.4.1_The_Array_Constructor_Called_as_a_Function"); }
    [TestMethod] public void S15_4_1_A1_1_T1_js() { RunFile(@"S15.4.1_A1.1_T1.js"); }
    [TestMethod] public void S15_4_1_A1_1_T2_js() { RunFile(@"S15.4.1_A1.1_T2.js"); }
    [TestMethod] public void S15_4_1_A1_1_T3_js() { RunFile(@"S15.4.1_A1.1_T3.js"); }
    [TestMethod] public void S15_4_1_A1_2_T1_js() { RunFile(@"S15.4.1_A1.2_T1.js"); }
    [TestMethod] public void S15_4_1_A1_3_T1_js() { RunFile(@"S15.4.1_A1.3_T1.js"); }
    [TestMethod] public void S15_4_1_A2_1_T1_js() { RunFile(@"S15.4.1_A2.1_T1.js"); }
    [TestMethod] public void S15_4_1_A2_2_T1_js() { RunFile(@"S15.4.1_A2.2_T1.js"); }
    [TestMethod] public void S15_4_1_A3_1_T1_js() { RunFile(@"S15.4.1_A3.1_T1.js"); }
  }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_10_RegExp_Objects_15_10_3_The_RegExp_Constructor_Called_as_a_Function : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.10_RegExp_Objects\15.10.3_The_RegExp_Constructor_Called_as_a_Function"); }
    [TestMethod] public void S15_10_3_1_A1_T1_js() { RunFile(@"S15.10.3.1_A1_T1.js"); }
    [TestMethod] public void S15_10_3_1_A1_T2_js() { RunFile(@"S15.10.3.1_A1_T2.js"); }
    [TestMethod] public void S15_10_3_1_A1_T3_js() { RunFile(@"S15.10.3.1_A1_T3.js"); }
    [TestMethod] public void S15_10_3_1_A1_T4_js() { RunFile(@"S15.10.3.1_A1_T4.js"); }
    [TestMethod] public void S15_10_3_1_A1_T5_js() { RunFile(@"S15.10.3.1_A1_T5.js"); }
    [TestMethod] public void S15_10_3_1_A2_T1_js() { RunFile(@"S15.10.3.1_A2_T1.js"); }
    [TestMethod] public void S15_10_3_1_A2_T2_js() { RunFile(@"S15.10.3.1_A2_T2.js"); }
    [TestMethod] public void S15_10_3_1_A3_T1_js() { RunFile(@"S15.10.3.1_A3_T1.js"); }
    [TestMethod] public void S15_10_3_1_A3_T2_js() { RunFile(@"S15.10.3.1_A3_T2.js"); }
  }
}
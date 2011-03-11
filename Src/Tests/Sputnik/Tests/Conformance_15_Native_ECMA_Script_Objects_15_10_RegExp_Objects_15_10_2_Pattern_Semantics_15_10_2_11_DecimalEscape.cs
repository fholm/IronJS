using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_10_RegExp_Objects_15_10_2_Pattern_Semantics_15_10_2_11_DecimalEscape : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.10_RegExp_Objects\15.10.2_Pattern_Semantics\15.10.2.11_DecimalEscape"); }
    [TestMethod] public void S15_10_2_11_A1_T1_js() { RunFile(@"S15.10.2.11_A1_T1.js"); }
    [TestMethod] public void S15_10_2_11_A1_T2_js() { RunFile(@"S15.10.2.11_A1_T2.js"); }
    [TestMethod] public void S15_10_2_11_A1_T3_js() { RunFile(@"S15.10.2.11_A1_T3.js"); }
    [TestMethod] public void S15_10_2_11_A1_T4_js() { RunFile(@"S15.10.2.11_A1_T4.js"); }
    [TestMethod] public void S15_10_2_11_A1_T5_js() { RunFile(@"S15.10.2.11_A1_T5.js"); }
    [TestMethod] public void S15_10_2_11_A1_T6_js() { RunFile(@"S15.10.2.11_A1_T6.js"); }
    [TestMethod] public void S15_10_2_11_A1_T7_js() { RunFile(@"S15.10.2.11_A1_T7.js"); }
    [TestMethod] public void S15_10_2_11_A1_T8_js() { RunFile(@"S15.10.2.11_A1_T8.js"); }
    [TestMethod] public void S15_10_2_11_A1_T9_js() { RunFile(@"S15.10.2.11_A1_T9.js"); }
  }
}
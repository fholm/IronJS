using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_10_RegExp_Objects_15_10_2_Pattern_Semantics_15_10_2_10_CharacterEscape : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.10_RegExp_Objects\15.10.2_Pattern_Semantics\15.10.2.10_CharacterEscape"); }
    [TestMethod] public void S15_10_2_10_A1_1_T1_js() { RunFile(@"S15.10.2.10_A1.1_T1.js"); }
    [TestMethod] public void S15_10_2_10_A1_2_T1_js() { RunFile(@"S15.10.2.10_A1.2_T1.js"); }
    [TestMethod] public void S15_10_2_10_A1_3_T1_js() { RunFile(@"S15.10.2.10_A1.3_T1.js"); }
    [TestMethod] public void S15_10_2_10_A1_4_T1_js() { RunFile(@"S15.10.2.10_A1.4_T1.js"); }
    [TestMethod] public void S15_10_2_10_A1_5_T1_js() { RunFile(@"S15.10.2.10_A1.5_T1.js"); }
    [TestMethod] public void S15_10_2_10_A2_1_T1_js() { RunFile(@"S15.10.2.10_A2.1_T1.js"); }
    [TestMethod] public void S15_10_2_10_A2_1_T2_js() { RunFile(@"S15.10.2.10_A2.1_T2.js"); }
    [TestMethod] public void S15_10_2_10_A2_1_T3_js() { RunFile(@"S15.10.2.10_A2.1_T3.js"); }
    [TestMethod] public void S15_10_2_10_A3_1_T1_js() { RunFile(@"S15.10.2.10_A3.1_T1.js"); }
    [TestMethod] public void S15_10_2_10_A3_1_T2_js() { RunFile(@"S15.10.2.10_A3.1_T2.js"); }
    [TestMethod] public void S15_10_2_10_A4_1_T1_js() { RunFile(@"S15.10.2.10_A4.1_T1.js"); }
    [TestMethod] public void S15_10_2_10_A4_1_T2_js() { RunFile(@"S15.10.2.10_A4.1_T2.js"); }
    [TestMethod] public void S15_10_2_10_A4_1_T3_js() { RunFile(@"S15.10.2.10_A4.1_T3.js"); }
    [TestMethod] public void S15_10_2_10_A5_1_T1_js() { RunFile(@"S15.10.2.10_A5.1_T1.js"); }
  }
}
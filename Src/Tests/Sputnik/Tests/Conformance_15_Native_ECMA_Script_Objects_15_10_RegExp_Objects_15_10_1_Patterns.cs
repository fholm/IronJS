using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_10_RegExp_Objects_15_10_1_Patterns : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.10_RegExp_Objects\15.10.1_Patterns"); }
    [TestMethod] public void S15_10_1_A1_T1_js() { RunFile(@"S15.10.1_A1_T1.js"); }
    [TestMethod] public void S15_10_1_A1_T10_js() { RunFile(@"S15.10.1_A1_T10.js"); }
    [TestMethod] public void S15_10_1_A1_T11_js() { RunFile(@"S15.10.1_A1_T11.js"); }
    [TestMethod] public void S15_10_1_A1_T12_js() { RunFile(@"S15.10.1_A1_T12.js"); }
    [TestMethod] public void S15_10_1_A1_T13_js() { RunFile(@"S15.10.1_A1_T13.js"); }
    [TestMethod] public void S15_10_1_A1_T14_js() { RunFile(@"S15.10.1_A1_T14.js"); }
    [TestMethod] public void S15_10_1_A1_T15_js() { RunFile(@"S15.10.1_A1_T15.js"); }
    [TestMethod] public void S15_10_1_A1_T16_js() { RunFile(@"S15.10.1_A1_T16.js"); }
    [TestMethod] public void S15_10_1_A1_T2_js() { RunFile(@"S15.10.1_A1_T2.js"); }
    [TestMethod] public void S15_10_1_A1_T3_js() { RunFile(@"S15.10.1_A1_T3.js"); }
    [TestMethod] public void S15_10_1_A1_T4_js() { RunFile(@"S15.10.1_A1_T4.js"); }
    [TestMethod] public void S15_10_1_A1_T5_js() { RunFile(@"S15.10.1_A1_T5.js"); }
    [TestMethod] public void S15_10_1_A1_T6_js() { RunFile(@"S15.10.1_A1_T6.js"); }
    [TestMethod] public void S15_10_1_A1_T7_js() { RunFile(@"S15.10.1_A1_T7.js"); }
    [TestMethod] public void S15_10_1_A1_T8_js() { RunFile(@"S15.10.1_A1_T8.js"); }
    [TestMethod] public void S15_10_1_A1_T9_js() { RunFile(@"S15.10.1_A1_T9.js"); }
  }
}
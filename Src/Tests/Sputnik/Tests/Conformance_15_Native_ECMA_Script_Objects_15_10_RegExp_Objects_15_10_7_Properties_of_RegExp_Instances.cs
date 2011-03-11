using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_10_RegExp_Objects_15_10_7_Properties_of_RegExp_Instances : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.10_RegExp_Objects\15.10.7_Properties_of_RegExp_Instances"); }
    [TestMethod] public void S15_10_7_A1_T1_js() { RunFile(@"S15.10.7_A1_T1.js"); }
    [TestMethod] public void S15_10_7_A1_T2_js() { RunFile(@"S15.10.7_A1_T2.js"); }
    [TestMethod] public void S15_10_7_A2_T1_js() { RunFile(@"S15.10.7_A2_T1.js"); }
    [TestMethod] public void S15_10_7_A2_T2_js() { RunFile(@"S15.10.7_A2_T2.js"); }
    [TestMethod] public void S15_10_7_A3_T1_js() { RunFile(@"S15.10.7_A3_T1.js"); }
    [TestMethod] public void S15_10_7_A3_T2_js() { RunFile(@"S15.10.7_A3_T2.js"); }
  }
}
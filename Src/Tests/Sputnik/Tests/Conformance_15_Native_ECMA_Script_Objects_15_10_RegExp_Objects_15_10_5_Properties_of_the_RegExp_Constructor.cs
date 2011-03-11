using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_10_RegExp_Objects_15_10_5_Properties_of_the_RegExp_Constructor : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.10_RegExp_Objects\15.10.5_Properties_of_the_RegExp_Constructor"); }
    [TestMethod] public void S15_10_5_1_A1_js() { RunFile(@"S15.10.5.1_A1.js"); }
    [TestMethod] public void S15_10_5_1_A2_js() { RunFile(@"S15.10.5.1_A2.js"); }
    [TestMethod] public void S15_10_5_1_A3_js() { RunFile(@"S15.10.5.1_A3.js"); }
    [TestMethod] public void S15_10_5_1_A4_js() { RunFile(@"S15.10.5.1_A4.js"); }
    [TestMethod] public void S15_10_5_A1_js() { RunFile(@"S15.10.5_A1.js"); }
    [TestMethod] public void S15_10_5_A2_T1_js() { RunFile(@"S15.10.5_A2_T1.js"); }
    [TestMethod] public void S15_10_5_A2_T2_js() { RunFile(@"S15.10.5_A2_T2.js"); }
  }
}
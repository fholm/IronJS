using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_10_RegExp_Objects_15_10_7_Properties_of_RegExp_Instances_15_10_7_1_source : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.10_RegExp_Objects\15.10.7_Properties_of_RegExp_Instances\15.10.7.1_source"); }
    [TestMethod] public void S15_10_7_1_A10_js() { RunFile(@"S15.10.7.1_A10.js"); }
    [TestMethod] public void S15_10_7_1_A8_js() { RunFile(@"S15.10.7.1_A8.js"); }
    [TestMethod] public void S15_10_7_1_A9_js() { RunFile(@"S15.10.7.1_A9.js"); }
  }
}
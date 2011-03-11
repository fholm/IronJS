using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_10_RegExp_Objects_15_10_6_Properties_of_the_RegExp_Prototype_Object : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.10_RegExp_Objects\15.10.6_Properties_of_the_RegExp_Prototype_Object"); }
    [TestMethod] public void S15_10_6_1_A1_T1_js() { RunFile(@"S15.10.6.1_A1_T1.js"); }
    [TestMethod] public void S15_10_6_1_A1_T2_js() { RunFile(@"S15.10.6.1_A1_T2.js"); }
    [TestMethod] public void S15_10_6_A1_T1_js() { RunFile(@"S15.10.6_A1_T1.js"); }
    [TestMethod] public void S15_10_6_A1_T2_js() { RunFile(@"S15.10.6_A1_T2.js"); }
    [TestMethod] public void S15_10_6_A2_js() { RunFile(@"S15.10.6_A2.js"); }
  }
}
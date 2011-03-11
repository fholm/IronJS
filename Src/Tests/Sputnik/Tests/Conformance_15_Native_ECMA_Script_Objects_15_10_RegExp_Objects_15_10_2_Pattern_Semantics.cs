using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_10_RegExp_Objects_15_10_2_Pattern_Semantics : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.10_RegExp_Objects\15.10.2_Pattern_Semantics"); }
    [TestMethod] public void S15_10_2_A1_T1_js() { RunFile(@"S15.10.2_A1_T1.js"); }
  }
}
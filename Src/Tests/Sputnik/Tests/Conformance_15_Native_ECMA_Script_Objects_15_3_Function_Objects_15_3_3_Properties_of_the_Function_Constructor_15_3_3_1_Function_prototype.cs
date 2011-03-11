using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_3_Function_Objects_15_3_3_Properties_of_the_Function_Constructor_15_3_3_1_Function_prototype : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.3_Function_Objects\15.3.3_Properties_of_the_Function_Constructor\15.3.3.1_Function.prototype"); }
    [TestMethod] public void S15_3_3_1_A1_js() { RunFile(@"S15.3.3.1_A1.js"); }
    [TestMethod] public void S15_3_3_1_A2_js() { RunFile(@"S15.3.3.1_A2.js"); }
    [TestMethod] public void S15_3_3_1_A3_js() { RunFile(@"S15.3.3.1_A3.js"); }
  }
}
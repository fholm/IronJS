using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_2_Object_Objects_15_2_3_Properties_of_the_Object_Constructor_15_2_3_1_Object_prototype : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.2_Object_Objects\15.2.3_Properties_of_the_Object_Constructor\15.2.3.1_Object.prototype"); }
    [TestMethod] public void S15_2_3_1_A1_js() { RunFile(@"S15.2.3.1_A1.js"); }
    [TestMethod] public void S15_2_3_1_A2_js() { RunFile(@"S15.2.3.1_A2.js"); }
    [TestMethod] public void S15_2_3_1_A3_js() { RunFile(@"S15.2.3.1_A3.js"); }
  }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_7_Number_Objects_15_7_4_Properties_of_the_Number_Prototype_Object_15_7_4_1_Number_prototype_constructor : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.7_Number_Objects\15.7.4_Properties_of_the_Number_Prototype_Object\15.7.4.1_Number.prototype.constructor"); }
    [TestMethod] public void S15_7_4_1_A1_js() { RunFile(@"S15.7.4.1_A1.js"); }
  }
}
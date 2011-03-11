using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_4_Array_Objects_15_4_4_Properties_of_the_Array_Prototype_Object_15_4_4_1_Array_prototype_constructor : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.4_Array_Objects\15.4.4_Properties_of_the_Array_Prototype_Object\15.4.4.1_Array_prototype_constructor"); }
    [TestMethod] public void S15_4_4_1_A1_T1_js() { RunFile(@"S15.4.4.1_A1_T1.js"); }
    [TestMethod] public void S15_4_4_1_A2_js() { RunFile(@"S15.4.4.1_A2.js"); }
  }
}
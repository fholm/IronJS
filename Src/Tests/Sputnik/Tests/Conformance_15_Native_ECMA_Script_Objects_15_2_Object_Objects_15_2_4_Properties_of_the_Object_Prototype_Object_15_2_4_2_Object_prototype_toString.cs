using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_2_Object_Objects_15_2_4_Properties_of_the_Object_Prototype_Object_15_2_4_2_Object_prototype_toString : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.2_Object_Objects\15.2.4_Properties_of_the_Object_Prototype_Object\15.2.4.2_Object.prototype.toString"); }
    [TestMethod] public void S15_2_4_2_A1_js() { RunFile(@"S15.2.4.2_A1.js"); }
    [TestMethod] public void S15_2_4_2_A10_js() { RunFile(@"S15.2.4.2_A10.js"); }
    [TestMethod] public void S15_2_4_2_A11_js() { RunFile(@"S15.2.4.2_A11.js"); }
    [TestMethod] public void S15_2_4_2_A6_js() { RunFile(@"S15.2.4.2_A6.js"); }
    [TestMethod] public void S15_2_4_2_A7_js() { RunFile(@"S15.2.4.2_A7.js"); }
    [TestMethod] public void S15_2_4_2_A8_js() { RunFile(@"S15.2.4.2_A8.js"); }
    [TestMethod] public void S15_2_4_2_A9_js() { RunFile(@"S15.2.4.2_A9.js"); }
  }
}
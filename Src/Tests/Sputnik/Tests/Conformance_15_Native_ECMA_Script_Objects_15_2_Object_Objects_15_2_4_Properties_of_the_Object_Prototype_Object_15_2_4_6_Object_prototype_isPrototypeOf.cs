using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_2_Object_Objects_15_2_4_Properties_of_the_Object_Prototype_Object_15_2_4_6_Object_prototype_isPrototypeOf : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.2_Object_Objects\15.2.4_Properties_of_the_Object_Prototype_Object\15.2.4.6_Object.prototype.isPrototypeOf"); }
    [TestMethod] public void S15_2_4_6_A1_js() { RunFile(@"S15.2.4.6_A1.js"); }
    [TestMethod] public void S15_2_4_6_A10_js() { RunFile(@"S15.2.4.6_A10.js"); }
    [TestMethod] public void S15_2_4_6_A11_js() { RunFile(@"S15.2.4.6_A11.js"); }
    [TestMethod] public void S15_2_4_6_A6_js() { RunFile(@"S15.2.4.6_A6.js"); }
    [TestMethod] public void S15_2_4_6_A7_js() { RunFile(@"S15.2.4.6_A7.js"); }
    [TestMethod] public void S15_2_4_6_A8_js() { RunFile(@"S15.2.4.6_A8.js"); }
    [TestMethod] public void S15_2_4_6_A9_js() { RunFile(@"S15.2.4.6_A9.js"); }
  }
}
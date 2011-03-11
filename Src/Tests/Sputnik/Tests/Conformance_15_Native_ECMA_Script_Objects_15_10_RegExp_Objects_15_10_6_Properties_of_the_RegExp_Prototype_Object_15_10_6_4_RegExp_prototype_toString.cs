using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_10_RegExp_Objects_15_10_6_Properties_of_the_RegExp_Prototype_Object_15_10_6_4_RegExp_prototype_toString : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.10_RegExp_Objects\15.10.6_Properties_of_the_RegExp_Prototype_Object\15.10.6.4_RegExp.prototype.toString"); }
    [TestMethod] public void S15_10_6_4_A10_js() { RunFile(@"S15.10.6.4_A10.js"); }
    [TestMethod] public void S15_10_6_4_A11_js() { RunFile(@"S15.10.6.4_A11.js"); }
    [TestMethod] public void S15_10_6_4_A6_js() { RunFile(@"S15.10.6.4_A6.js"); }
    [TestMethod] public void S15_10_6_4_A7_js() { RunFile(@"S15.10.6.4_A7.js"); }
    [TestMethod] public void S15_10_6_4_A8_js() { RunFile(@"S15.10.6.4_A8.js"); }
    [TestMethod] public void S15_10_6_4_A9_js() { RunFile(@"S15.10.6.4_A9.js"); }
  }
}
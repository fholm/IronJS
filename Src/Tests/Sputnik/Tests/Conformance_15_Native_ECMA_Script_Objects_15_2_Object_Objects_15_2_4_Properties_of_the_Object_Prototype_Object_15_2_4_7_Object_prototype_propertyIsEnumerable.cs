using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_2_Object_Objects_15_2_4_Properties_of_the_Object_Prototype_Object_15_2_4_7_Object_prototype_propertyIsEnumerable : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.2_Object_Objects\15.2.4_Properties_of_the_Object_Prototype_Object\15.2.4.7_Object.prototype.propertyIsEnumerable"); }
    [TestMethod] public void S15_2_4_7_A10_js() { RunFile(@"S15.2.4.7_A10.js"); }
    [TestMethod] public void S15_2_4_7_A11_js() { RunFile(@"S15.2.4.7_A11.js"); }
    [TestMethod] public void S15_2_4_7_A1_T1_js() { RunFile(@"S15.2.4.7_A1_T1.js"); }
    [TestMethod] public void S15_2_4_7_A2_T1_js() { RunFile(@"S15.2.4.7_A2_T1.js"); }
    [TestMethod] public void S15_2_4_7_A2_T2_js() { RunFile(@"S15.2.4.7_A2_T2.js"); }
    [TestMethod] public void S15_2_4_7_A6_js() { RunFile(@"S15.2.4.7_A6.js"); }
    [TestMethod] public void S15_2_4_7_A7_js() { RunFile(@"S15.2.4.7_A7.js"); }
    [TestMethod] public void S15_2_4_7_A8_js() { RunFile(@"S15.2.4.7_A8.js"); }
    [TestMethod] public void S15_2_4_7_A9_js() { RunFile(@"S15.2.4.7_A9.js"); }
  }
}
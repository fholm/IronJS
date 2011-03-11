using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_5_String_Objects_15_5_4_Properties_of_the_String_Prototype_Object_15_5_4_9_String_prototype_localeCompare : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.5_String_Objects\15.5.4_Properties_of_the_String_Prototype_Object\15.5.4.9_String.prototype.localeCompare"); }
    [TestMethod] public void S15_5_4_9_A10_js() { RunFile(@"S15.5.4.9_A10.js"); }
    [TestMethod] public void S15_5_4_9_A11_js() { RunFile(@"S15.5.4.9_A11.js"); }
    [TestMethod] public void S15_5_4_9_A1_T1_js() { RunFile(@"S15.5.4.9_A1_T1.js"); }
    [TestMethod] public void S15_5_4_9_A1_T2_js() { RunFile(@"S15.5.4.9_A1_T2.js"); }
    [TestMethod] public void S15_5_4_9_A6_js() { RunFile(@"S15.5.4.9_A6.js"); }
    [TestMethod] public void S15_5_4_9_A7_js() { RunFile(@"S15.5.4.9_A7.js"); }
    [TestMethod] public void S15_5_4_9_A8_js() { RunFile(@"S15.5.4.9_A8.js"); }
    [TestMethod] public void S15_5_4_9_A9_js() { RunFile(@"S15.5.4.9_A9.js"); }
  }
}
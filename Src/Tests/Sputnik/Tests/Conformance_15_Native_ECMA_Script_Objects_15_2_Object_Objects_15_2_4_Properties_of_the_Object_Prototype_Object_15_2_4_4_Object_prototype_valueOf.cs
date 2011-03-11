using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_2_Object_Objects_15_2_4_Properties_of_the_Object_Prototype_Object_15_2_4_4_Object_prototype_valueOf : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.2_Object_Objects\15.2.4_Properties_of_the_Object_Prototype_Object\15.2.4.4_Object.prototype.valueOf"); }
    [TestMethod] public void S15_2_4_4_A10_js() { RunFile(@"S15.2.4.4_A10.js"); }
    [TestMethod] public void S15_2_4_4_A11_js() { RunFile(@"S15.2.4.4_A11.js"); }
    [TestMethod] public void S15_2_4_4_A1_T1_js() { RunFile(@"S15.2.4.4_A1_T1.js"); }
    [TestMethod] public void S15_2_4_4_A1_T2_js() { RunFile(@"S15.2.4.4_A1_T2.js"); }
    [TestMethod] public void S15_2_4_4_A1_T3_js() { RunFile(@"S15.2.4.4_A1_T3.js"); }
    [TestMethod] public void S15_2_4_4_A1_T4_js() { RunFile(@"S15.2.4.4_A1_T4.js"); }
    [TestMethod] public void S15_2_4_4_A1_T5_js() { RunFile(@"S15.2.4.4_A1_T5.js"); }
    [TestMethod] public void S15_2_4_4_A1_T6_js() { RunFile(@"S15.2.4.4_A1_T6.js"); }
    [TestMethod] public void S15_2_4_4_A1_T7_js() { RunFile(@"S15.2.4.4_A1_T7.js"); }
    [TestMethod] public void S15_2_4_4_A6_js() { RunFile(@"S15.2.4.4_A6.js"); }
    [TestMethod] public void S15_2_4_4_A7_js() { RunFile(@"S15.2.4.4_A7.js"); }
    [TestMethod] public void S15_2_4_4_A8_js() { RunFile(@"S15.2.4.4_A8.js"); }
    [TestMethod] public void S15_2_4_4_A9_js() { RunFile(@"S15.2.4.4_A9.js"); }
  }
}
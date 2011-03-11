using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_4_Array_Objects_15_4_4_Properties_of_the_Array_Prototype_Object_15_4_4_4_Array_prototype_concat : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.4_Array_Objects\15.4.4_Properties_of_the_Array_Prototype_Object\15.4.4.4_Array_prototype_concat"); }
    [TestMethod] public void S15_4_4_4_A1_T1_js() { RunFile(@"S15.4.4.4_A1_T1.js"); }
    [TestMethod] public void S15_4_4_4_A1_T2_js() { RunFile(@"S15.4.4.4_A1_T2.js"); }
    [TestMethod] public void S15_4_4_4_A1_T3_js() { RunFile(@"S15.4.4.4_A1_T3.js"); }
    [TestMethod] public void S15_4_4_4_A1_T4_js() { RunFile(@"S15.4.4.4_A1_T4.js"); }
    [TestMethod] public void S15_4_4_4_A2_T1_js() { RunFile(@"S15.4.4.4_A2_T1.js"); }
    [TestMethod] public void S15_4_4_4_A2_T2_js() { RunFile(@"S15.4.4.4_A2_T2.js"); }
    [TestMethod] public void S15_4_4_4_A3_T1_js() { RunFile(@"S15.4.4.4_A3_T1.js"); }
    [TestMethod] public void S15_4_4_4_A4_1_js() { RunFile(@"S15.4.4.4_A4.1.js"); }
    [TestMethod] public void S15_4_4_4_A4_2_js() { RunFile(@"S15.4.4.4_A4.2.js"); }
    [TestMethod] public void S15_4_4_4_A4_3_js() { RunFile(@"S15.4.4.4_A4.3.js"); }
    [TestMethod] public void S15_4_4_4_A4_4_js() { RunFile(@"S15.4.4.4_A4.4.js"); }
    [TestMethod] public void S15_4_4_4_A4_5_js() { RunFile(@"S15.4.4.4_A4.5.js"); }
    [TestMethod] public void S15_4_4_4_A4_6_js() { RunFile(@"S15.4.4.4_A4.6.js"); }
    [TestMethod] public void S15_4_4_4_A4_7_js() { RunFile(@"S15.4.4.4_A4.7.js"); }
  }
}
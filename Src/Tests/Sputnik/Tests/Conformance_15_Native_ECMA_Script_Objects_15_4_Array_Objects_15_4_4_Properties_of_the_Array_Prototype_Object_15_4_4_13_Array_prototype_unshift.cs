using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_4_Array_Objects_15_4_4_Properties_of_the_Array_Prototype_Object_15_4_4_13_Array_prototype_unshift : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.4_Array_Objects\15.4.4_Properties_of_the_Array_Prototype_Object\15.4.4.13_Array_prototype_unshift"); }
    [TestMethod] public void S15_4_4_13_A1_T1_js() { RunFile(@"S15.4.4.13_A1_T1.js"); }
    [TestMethod] public void S15_4_4_13_A1_T2_js() { RunFile(@"S15.4.4.13_A1_T2.js"); }
    [TestMethod] public void S15_4_4_13_A2_T1_js() { RunFile(@"S15.4.4.13_A2_T1.js"); }
    [TestMethod] public void S15_4_4_13_A2_T2_js() { RunFile(@"S15.4.4.13_A2_T2.js"); }
    [TestMethod] public void S15_4_4_13_A2_T3_js() { RunFile(@"S15.4.4.13_A2_T3.js"); }
    [TestMethod] public void S15_4_4_13_A3_T1_js() { RunFile(@"S15.4.4.13_A3_T1.js"); }
    [TestMethod] public void S15_4_4_13_A3_T2_js() { RunFile(@"S15.4.4.13_A3_T2.js"); }
    [TestMethod] public void S15_4_4_13_A3_T3_js() { RunFile(@"S15.4.4.13_A3_T3.js"); }
    [TestMethod] public void S15_4_4_13_A4_T1_js() { RunFile(@"S15.4.4.13_A4_T1.js"); }
    [TestMethod] public void S15_4_4_13_A4_T2_js() { RunFile(@"S15.4.4.13_A4_T2.js"); }
    [TestMethod] public void S15_4_4_13_A5_1_js() { RunFile(@"S15.4.4.13_A5.1.js"); }
    [TestMethod] public void S15_4_4_13_A5_2_js() { RunFile(@"S15.4.4.13_A5.2.js"); }
    [TestMethod] public void S15_4_4_13_A5_3_js() { RunFile(@"S15.4.4.13_A5.3.js"); }
    [TestMethod] public void S15_4_4_13_A5_4_js() { RunFile(@"S15.4.4.13_A5.4.js"); }
    [TestMethod] public void S15_4_4_13_A5_5_js() { RunFile(@"S15.4.4.13_A5.5.js"); }
    [TestMethod] public void S15_4_4_13_A5_6_js() { RunFile(@"S15.4.4.13_A5.6.js"); }
    [TestMethod] public void S15_4_4_13_A5_7_js() { RunFile(@"S15.4.4.13_A5.7.js"); }
  }
}
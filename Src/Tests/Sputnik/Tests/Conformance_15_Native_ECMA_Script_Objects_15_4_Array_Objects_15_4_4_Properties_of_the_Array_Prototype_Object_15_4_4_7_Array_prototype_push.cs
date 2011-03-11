using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_4_Array_Objects_15_4_4_Properties_of_the_Array_Prototype_Object_15_4_4_7_Array_prototype_push : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.4_Array_Objects\15.4.4_Properties_of_the_Array_Prototype_Object\15.4.4.7_Array_prototype_push"); }
    [TestMethod] public void S15_4_4_7_A1_T1_js() { RunFile(@"S15.4.4.7_A1_T1.js"); }
    [TestMethod] public void S15_4_4_7_A1_T2_js() { RunFile(@"S15.4.4.7_A1_T2.js"); }
    [TestMethod] public void S15_4_4_7_A2_T1_js() { RunFile(@"S15.4.4.7_A2_T1.js"); }
    [TestMethod] public void S15_4_4_7_A2_T2_js() { RunFile(@"S15.4.4.7_A2_T2.js"); }
    [TestMethod] public void S15_4_4_7_A2_T3_js() { RunFile(@"S15.4.4.7_A2_T3.js"); }
    [TestMethod] public void S15_4_4_7_A3_js() { RunFile(@"S15.4.4.7_A3.js"); }
    [TestMethod] public void S15_4_4_7_A4_T1_js() { RunFile(@"S15.4.4.7_A4_T1.js"); }
    [TestMethod] public void S15_4_4_7_A4_T2_js() { RunFile(@"S15.4.4.7_A4_T2.js"); }
    [TestMethod] public void S15_4_4_7_A4_T3_js() { RunFile(@"S15.4.4.7_A4_T3.js"); }
    [TestMethod] public void S15_4_4_7_A5_T1_js() { RunFile(@"S15.4.4.7_A5_T1.js"); }
    [TestMethod] public void S15_4_4_7_A6_1_js() { RunFile(@"S15.4.4.7_A6.1.js"); }
    [TestMethod] public void S15_4_4_7_A6_2_js() { RunFile(@"S15.4.4.7_A6.2.js"); }
    [TestMethod] public void S15_4_4_7_A6_3_js() { RunFile(@"S15.4.4.7_A6.3.js"); }
    [TestMethod] public void S15_4_4_7_A6_4_js() { RunFile(@"S15.4.4.7_A6.4.js"); }
    [TestMethod] public void S15_4_4_7_A6_5_js() { RunFile(@"S15.4.4.7_A6.5.js"); }
    [TestMethod] public void S15_4_4_7_A6_6_js() { RunFile(@"S15.4.4.7_A6.6.js"); }
    [TestMethod] public void S15_4_4_7_A6_7_js() { RunFile(@"S15.4.4.7_A6.7.js"); }
  }
}
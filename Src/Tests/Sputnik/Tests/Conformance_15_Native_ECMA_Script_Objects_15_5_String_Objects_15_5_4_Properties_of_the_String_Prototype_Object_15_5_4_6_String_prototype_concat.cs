using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_5_String_Objects_15_5_4_Properties_of_the_String_Prototype_Object_15_5_4_6_String_prototype_concat : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.5_String_Objects\15.5.4_Properties_of_the_String_Prototype_Object\15.5.4.6_String.prototype.concat"); }
    [TestMethod] public void S15_5_4_6_A10_js() { RunFile(@"S15.5.4.6_A10.js"); }
    [TestMethod] public void S15_5_4_6_A11_js() { RunFile(@"S15.5.4.6_A11.js"); }
    [TestMethod] public void S15_5_4_6_A1_T1_js() { RunFile(@"S15.5.4.6_A1_T1.js"); }
    [TestMethod] public void S15_5_4_6_A1_T10_js() { RunFile(@"S15.5.4.6_A1_T10.js"); }
    [TestMethod] public void S15_5_4_6_A1_T2_js() { RunFile(@"S15.5.4.6_A1_T2.js"); }
    [TestMethod] public void S15_5_4_6_A1_T3_js() { RunFile(@"S15.5.4.6_A1_T3.js"); }
    [TestMethod] public void S15_5_4_6_A1_T4_js() { RunFile(@"S15.5.4.6_A1_T4.js"); }
    [TestMethod] public void S15_5_4_6_A1_T5_js() { RunFile(@"S15.5.4.6_A1_T5.js"); }
    [TestMethod] public void S15_5_4_6_A1_T6_js() { RunFile(@"S15.5.4.6_A1_T6.js"); }
    [TestMethod] public void S15_5_4_6_A1_T7_js() { RunFile(@"S15.5.4.6_A1_T7.js"); }
    [TestMethod] public void S15_5_4_6_A1_T8_js() { RunFile(@"S15.5.4.6_A1_T8.js"); }
    [TestMethod] public void S15_5_4_6_A1_T9_js() { RunFile(@"S15.5.4.6_A1_T9.js"); }
    [TestMethod] public void S15_5_4_6_A2_js() { RunFile(@"S15.5.4.6_A2.js"); }
    [TestMethod] public void S15_5_4_6_A3_js() { RunFile(@"S15.5.4.6_A3.js"); }
    [TestMethod] public void S15_5_4_6_A4_T1_js() { RunFile(@"S15.5.4.6_A4_T1.js"); }
    [TestMethod] public void S15_5_4_6_A4_T2_js() { RunFile(@"S15.5.4.6_A4_T2.js"); }
    [TestMethod] public void S15_5_4_6_A6_js() { RunFile(@"S15.5.4.6_A6.js"); }
    [TestMethod] public void S15_5_4_6_A7_js() { RunFile(@"S15.5.4.6_A7.js"); }
    [TestMethod] public void S15_5_4_6_A8_js() { RunFile(@"S15.5.4.6_A8.js"); }
    [TestMethod] public void S15_5_4_6_A9_js() { RunFile(@"S15.5.4.6_A9.js"); }
  }
}
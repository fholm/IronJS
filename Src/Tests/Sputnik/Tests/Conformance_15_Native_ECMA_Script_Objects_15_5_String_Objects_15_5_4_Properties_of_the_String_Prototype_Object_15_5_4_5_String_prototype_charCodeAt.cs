using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_5_String_Objects_15_5_4_Properties_of_the_String_Prototype_Object_15_5_4_5_String_prototype_charCodeAt : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.5_String_Objects\15.5.4_Properties_of_the_String_Prototype_Object\15.5.4.5_String.prototype.charCodeAt"); }
    [TestMethod] public void S15_5_4_5_A1_1_js() { RunFile(@"S15.5.4.5_A1.1.js"); }
    [TestMethod] public void S15_5_4_5_A10_js() { RunFile(@"S15.5.4.5_A10.js"); }
    [TestMethod] public void S15_5_4_5_A11_js() { RunFile(@"S15.5.4.5_A11.js"); }
    [TestMethod] public void S15_5_4_5_A1_T1_js() { RunFile(@"S15.5.4.5_A1_T1.js"); }
    [TestMethod] public void S15_5_4_5_A1_T10_js() { RunFile(@"S15.5.4.5_A1_T10.js"); }
    [TestMethod] public void S15_5_4_5_A1_T2_js() { RunFile(@"S15.5.4.5_A1_T2.js"); }
    [TestMethod] public void S15_5_4_5_A1_T3_js() { RunFile(@"S15.5.4.5_A1_T3.js"); }
    [TestMethod] public void S15_5_4_5_A1_T4_js() { RunFile(@"S15.5.4.5_A1_T4.js"); }
    [TestMethod] public void S15_5_4_5_A1_T5_js() { RunFile(@"S15.5.4.5_A1_T5.js"); }
    [TestMethod] public void S15_5_4_5_A1_T6_js() { RunFile(@"S15.5.4.5_A1_T6.js"); }
    [TestMethod] public void S15_5_4_5_A1_T7_js() { RunFile(@"S15.5.4.5_A1_T7.js"); }
    [TestMethod] public void S15_5_4_5_A1_T8_js() { RunFile(@"S15.5.4.5_A1_T8.js"); }
    [TestMethod] public void S15_5_4_5_A1_T9_js() { RunFile(@"S15.5.4.5_A1_T9.js"); }
    [TestMethod] public void S15_5_4_5_A2_js() { RunFile(@"S15.5.4.5_A2.js"); }
    [TestMethod] public void S15_5_4_5_A3_js() { RunFile(@"S15.5.4.5_A3.js"); }
    [TestMethod] public void S15_5_4_5_A4_js() { RunFile(@"S15.5.4.5_A4.js"); }
    [TestMethod] public void S15_5_4_5_A6_js() { RunFile(@"S15.5.4.5_A6.js"); }
    [TestMethod] public void S15_5_4_5_A7_js() { RunFile(@"S15.5.4.5_A7.js"); }
    [TestMethod] public void S15_5_4_5_A8_js() { RunFile(@"S15.5.4.5_A8.js"); }
    [TestMethod] public void S15_5_4_5_A9_js() { RunFile(@"S15.5.4.5_A9.js"); }
  }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_5_String_Objects_15_5_4_Properties_of_the_String_Prototype_Object_15_5_4_11_String_prototype_replace : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.5_String_Objects\15.5.4_Properties_of_the_String_Prototype_Object\15.5.4.11_String.prototype.replace"); }
    [TestMethod] public void S15_5_4_11_A10_js() { RunFile(@"S15.5.4.11_A10.js"); }
    [TestMethod] public void S15_5_4_11_A11_js() { RunFile(@"S15.5.4.11_A11.js"); }
    [TestMethod] public void S15_5_4_11_A1_T1_js() { RunFile(@"S15.5.4.11_A1_T1.js"); }
    [TestMethod] public void S15_5_4_11_A1_T10_js() { RunFile(@"S15.5.4.11_A1_T10.js"); }
    [TestMethod] public void S15_5_4_11_A1_T11_js() { RunFile(@"S15.5.4.11_A1_T11.js"); }
    [TestMethod] public void S15_5_4_11_A1_T12_js() { RunFile(@"S15.5.4.11_A1_T12.js"); }
    [TestMethod] public void S15_5_4_11_A1_T13_js() { RunFile(@"S15.5.4.11_A1_T13.js"); }
    [TestMethod] public void S15_5_4_11_A1_T14_js() { RunFile(@"S15.5.4.11_A1_T14.js"); }
    [TestMethod] public void S15_5_4_11_A1_T15_js() { RunFile(@"S15.5.4.11_A1_T15.js"); }
    [TestMethod] public void S15_5_4_11_A1_T16_js() { RunFile(@"S15.5.4.11_A1_T16.js"); }
    [TestMethod] public void S15_5_4_11_A1_T17_js() { RunFile(@"S15.5.4.11_A1_T17.js"); }
    [TestMethod] public void S15_5_4_11_A1_T2_js() { RunFile(@"S15.5.4.11_A1_T2.js"); }
    [TestMethod] public void S15_5_4_11_A1_T3_js() { RunFile(@"S15.5.4.11_A1_T3.js"); }
    [TestMethod] public void S15_5_4_11_A1_T4_js() { RunFile(@"S15.5.4.11_A1_T4.js"); }
    [TestMethod] public void S15_5_4_11_A1_T5_js() { RunFile(@"S15.5.4.11_A1_T5.js"); }
    [TestMethod] public void S15_5_4_11_A1_T6_js() { RunFile(@"S15.5.4.11_A1_T6.js"); }
    [TestMethod] public void S15_5_4_11_A1_T7_js() { RunFile(@"S15.5.4.11_A1_T7.js"); }
    [TestMethod] public void S15_5_4_11_A1_T8_js() { RunFile(@"S15.5.4.11_A1_T8.js"); }
    [TestMethod] public void S15_5_4_11_A1_T9_js() { RunFile(@"S15.5.4.11_A1_T9.js"); }
    [TestMethod] public void S15_5_4_11_A2_T1_js() { RunFile(@"S15.5.4.11_A2_T1.js"); }
    [TestMethod] public void S15_5_4_11_A2_T10_js() { RunFile(@"S15.5.4.11_A2_T10.js"); }
    [TestMethod] public void S15_5_4_11_A2_T2_js() { RunFile(@"S15.5.4.11_A2_T2.js"); }
    [TestMethod] public void S15_5_4_11_A2_T3_js() { RunFile(@"S15.5.4.11_A2_T3.js"); }
    [TestMethod] public void S15_5_4_11_A2_T4_js() { RunFile(@"S15.5.4.11_A2_T4.js"); }
    [TestMethod] public void S15_5_4_11_A2_T5_js() { RunFile(@"S15.5.4.11_A2_T5.js"); }
    [TestMethod] public void S15_5_4_11_A2_T6_js() { RunFile(@"S15.5.4.11_A2_T6.js"); }
    [TestMethod] public void S15_5_4_11_A2_T7_js() { RunFile(@"S15.5.4.11_A2_T7.js"); }
    [TestMethod] public void S15_5_4_11_A2_T8_js() { RunFile(@"S15.5.4.11_A2_T8.js"); }
    [TestMethod] public void S15_5_4_11_A2_T9_js() { RunFile(@"S15.5.4.11_A2_T9.js"); }
    [TestMethod] public void S15_5_4_11_A3_T1_js() { RunFile(@"S15.5.4.11_A3_T1.js"); }
    [TestMethod] public void S15_5_4_11_A3_T2_js() { RunFile(@"S15.5.4.11_A3_T2.js"); }
    [TestMethod] public void S15_5_4_11_A3_T3_js() { RunFile(@"S15.5.4.11_A3_T3.js"); }
    [TestMethod] public void S15_5_4_11_A4_T1_js() { RunFile(@"S15.5.4.11_A4_T1.js"); }
    [TestMethod] public void S15_5_4_11_A4_T2_js() { RunFile(@"S15.5.4.11_A4_T2.js"); }
    [TestMethod] public void S15_5_4_11_A4_T3_js() { RunFile(@"S15.5.4.11_A4_T3.js"); }
    [TestMethod] public void S15_5_4_11_A4_T4_js() { RunFile(@"S15.5.4.11_A4_T4.js"); }
    [TestMethod] public void S15_5_4_11_A5_T1_js() { RunFile(@"S15.5.4.11_A5_T1.js"); }
    [TestMethod] public void S15_5_4_11_A6_js() { RunFile(@"S15.5.4.11_A6.js"); }
    [TestMethod] public void S15_5_4_11_A7_js() { RunFile(@"S15.5.4.11_A7.js"); }
    [TestMethod] public void S15_5_4_11_A8_js() { RunFile(@"S15.5.4.11_A8.js"); }
    [TestMethod] public void S15_5_4_11_A9_js() { RunFile(@"S15.5.4.11_A9.js"); }
  }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_5_String_Objects_15_5_4_Properties_of_the_String_Prototype_Object_15_5_4_15_String_prototype_substring : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.5_String_Objects\15.5.4_Properties_of_the_String_Prototype_Object\15.5.4.15_String.prototype.substring"); }
    [TestMethod] public void S15_5_4_15_A10_js() { RunFile(@"S15.5.4.15_A10.js"); }
    [TestMethod] public void S15_5_4_15_A11_js() { RunFile(@"S15.5.4.15_A11.js"); }
    [TestMethod] public void S15_5_4_15_A1_T1_js() { RunFile(@"S15.5.4.15_A1_T1.js"); }
    [TestMethod] public void S15_5_4_15_A1_T10_js() { RunFile(@"S15.5.4.15_A1_T10.js"); }
    [TestMethod] public void S15_5_4_15_A1_T11_js() { RunFile(@"S15.5.4.15_A1_T11.js"); }
    [TestMethod] public void S15_5_4_15_A1_T12_js() { RunFile(@"S15.5.4.15_A1_T12.js"); }
    [TestMethod] public void S15_5_4_15_A1_T13_js() { RunFile(@"S15.5.4.15_A1_T13.js"); }
    [TestMethod] public void S15_5_4_15_A1_T14_js() { RunFile(@"S15.5.4.15_A1_T14.js"); }
    [TestMethod] public void S15_5_4_15_A1_T15_js() { RunFile(@"S15.5.4.15_A1_T15.js"); }
    [TestMethod] public void S15_5_4_15_A1_T2_js() { RunFile(@"S15.5.4.15_A1_T2.js"); }
    [TestMethod] public void S15_5_4_15_A1_T3_js() { RunFile(@"S15.5.4.15_A1_T3.js"); }
    [TestMethod] public void S15_5_4_15_A1_T4_js() { RunFile(@"S15.5.4.15_A1_T4.js"); }
    [TestMethod] public void S15_5_4_15_A1_T5_js() { RunFile(@"S15.5.4.15_A1_T5.js"); }
    [TestMethod] public void S15_5_4_15_A1_T6_js() { RunFile(@"S15.5.4.15_A1_T6.js"); }
    [TestMethod] public void S15_5_4_15_A1_T7_js() { RunFile(@"S15.5.4.15_A1_T7.js"); }
    [TestMethod] public void S15_5_4_15_A1_T8_js() { RunFile(@"S15.5.4.15_A1_T8.js"); }
    [TestMethod] public void S15_5_4_15_A1_T9_js() { RunFile(@"S15.5.4.15_A1_T9.js"); }
    [TestMethod] public void S15_5_4_15_A2_T1_js() { RunFile(@"S15.5.4.15_A2_T1.js"); }
    [TestMethod] public void S15_5_4_15_A2_T10_js() { RunFile(@"S15.5.4.15_A2_T10.js"); }
    [TestMethod] public void S15_5_4_15_A2_T2_js() { RunFile(@"S15.5.4.15_A2_T2.js"); }
    [TestMethod] public void S15_5_4_15_A2_T3_js() { RunFile(@"S15.5.4.15_A2_T3.js"); }
    [TestMethod] public void S15_5_4_15_A2_T4_js() { RunFile(@"S15.5.4.15_A2_T4.js"); }
    [TestMethod] public void S15_5_4_15_A2_T5_js() { RunFile(@"S15.5.4.15_A2_T5.js"); }
    [TestMethod] public void S15_5_4_15_A2_T6_js() { RunFile(@"S15.5.4.15_A2_T6.js"); }
    [TestMethod] public void S15_5_4_15_A2_T7_js() { RunFile(@"S15.5.4.15_A2_T7.js"); }
    [TestMethod] public void S15_5_4_15_A2_T8_js() { RunFile(@"S15.5.4.15_A2_T8.js"); }
    [TestMethod] public void S15_5_4_15_A2_T9_js() { RunFile(@"S15.5.4.15_A2_T9.js"); }
    [TestMethod] public void S15_5_4_15_A3_T1_js() { RunFile(@"S15.5.4.15_A3_T1.js"); }
    [TestMethod] public void S15_5_4_15_A3_T10_js() { RunFile(@"S15.5.4.15_A3_T10.js"); }
    [TestMethod] public void S15_5_4_15_A3_T11_js() { RunFile(@"S15.5.4.15_A3_T11.js"); }
    [TestMethod] public void S15_5_4_15_A3_T2_js() { RunFile(@"S15.5.4.15_A3_T2.js"); }
    [TestMethod] public void S15_5_4_15_A3_T3_js() { RunFile(@"S15.5.4.15_A3_T3.js"); }
    [TestMethod] public void S15_5_4_15_A3_T4_js() { RunFile(@"S15.5.4.15_A3_T4.js"); }
    [TestMethod] public void S15_5_4_15_A3_T5_js() { RunFile(@"S15.5.4.15_A3_T5.js"); }
    [TestMethod] public void S15_5_4_15_A3_T6_js() { RunFile(@"S15.5.4.15_A3_T6.js"); }
    [TestMethod] public void S15_5_4_15_A3_T7_js() { RunFile(@"S15.5.4.15_A3_T7.js"); }
    [TestMethod] public void S15_5_4_15_A3_T8_js() { RunFile(@"S15.5.4.15_A3_T8.js"); }
    [TestMethod] public void S15_5_4_15_A3_T9_js() { RunFile(@"S15.5.4.15_A3_T9.js"); }
    [TestMethod] public void S15_5_4_15_A6_js() { RunFile(@"S15.5.4.15_A6.js"); }
    [TestMethod] public void S15_5_4_15_A7_js() { RunFile(@"S15.5.4.15_A7.js"); }
    [TestMethod] public void S15_5_4_15_A8_js() { RunFile(@"S15.5.4.15_A8.js"); }
    [TestMethod] public void S15_5_4_15_A9_js() { RunFile(@"S15.5.4.15_A9.js"); }
  }
}
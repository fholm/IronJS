using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_5_String_Objects_15_5_4_Properties_of_the_String_Prototype_Object_15_5_4_17_String_prototype_toLocaleLowerCase : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.5_String_Objects\15.5.4_Properties_of_the_String_Prototype_Object\15.5.4.17_String.prototype.toLocaleLowerCase"); }
    [TestMethod] public void S15_5_4_17_A10_js() { RunFile(@"S15.5.4.17_A10.js"); }
    [TestMethod] public void S15_5_4_17_A11_js() { RunFile(@"S15.5.4.17_A11.js"); }
    [TestMethod] public void S15_5_4_17_A1_T1_js() { RunFile(@"S15.5.4.17_A1_T1.js"); }
    [TestMethod] public void S15_5_4_17_A1_T10_js() { RunFile(@"S15.5.4.17_A1_T10.js"); }
    [TestMethod] public void S15_5_4_17_A1_T11_js() { RunFile(@"S15.5.4.17_A1_T11.js"); }
    [TestMethod] public void S15_5_4_17_A1_T12_js() { RunFile(@"S15.5.4.17_A1_T12.js"); }
    [TestMethod] public void S15_5_4_17_A1_T13_js() { RunFile(@"S15.5.4.17_A1_T13.js"); }
    [TestMethod] public void S15_5_4_17_A1_T14_js() { RunFile(@"S15.5.4.17_A1_T14.js"); }
    [TestMethod] public void S15_5_4_17_A1_T2_js() { RunFile(@"S15.5.4.17_A1_T2.js"); }
    [TestMethod] public void S15_5_4_17_A1_T3_js() { RunFile(@"S15.5.4.17_A1_T3.js"); }
    [TestMethod] public void S15_5_4_17_A1_T4_js() { RunFile(@"S15.5.4.17_A1_T4.js"); }
    [TestMethod] public void S15_5_4_17_A1_T5_js() { RunFile(@"S15.5.4.17_A1_T5.js"); }
    [TestMethod] public void S15_5_4_17_A1_T6_js() { RunFile(@"S15.5.4.17_A1_T6.js"); }
    [TestMethod] public void S15_5_4_17_A1_T7_js() { RunFile(@"S15.5.4.17_A1_T7.js"); }
    [TestMethod] public void S15_5_4_17_A1_T8_js() { RunFile(@"S15.5.4.17_A1_T8.js"); }
    [TestMethod] public void S15_5_4_17_A1_T9_js() { RunFile(@"S15.5.4.17_A1_T9.js"); }
    [TestMethod] public void S15_5_4_17_A2_T1_js() { RunFile(@"S15.5.4.17_A2_T1.js"); }
    [TestMethod] public void S15_5_4_17_A6_js() { RunFile(@"S15.5.4.17_A6.js"); }
    [TestMethod] public void S15_5_4_17_A7_js() { RunFile(@"S15.5.4.17_A7.js"); }
    [TestMethod] public void S15_5_4_17_A8_js() { RunFile(@"S15.5.4.17_A8.js"); }
    [TestMethod] public void S15_5_4_17_A9_js() { RunFile(@"S15.5.4.17_A9.js"); }
  }
}
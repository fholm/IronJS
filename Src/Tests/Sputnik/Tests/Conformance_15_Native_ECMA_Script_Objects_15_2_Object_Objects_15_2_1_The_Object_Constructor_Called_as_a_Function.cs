using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_2_Object_Objects_15_2_1_The_Object_Constructor_Called_as_a_Function : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.2_Object_Objects\15.2.1_The_Object_Constructor_Called_as_a_Function"); }
    [TestMethod] public void S15_2_1_1_A1_T1_js() { RunFile(@"S15.2.1.1_A1_T1.js"); }
    [TestMethod] public void S15_2_1_1_A1_T2_js() { RunFile(@"S15.2.1.1_A1_T2.js"); }
    [TestMethod] public void S15_2_1_1_A1_T3_js() { RunFile(@"S15.2.1.1_A1_T3.js"); }
    [TestMethod] public void S15_2_1_1_A1_T4_js() { RunFile(@"S15.2.1.1_A1_T4.js"); }
    [TestMethod] public void S15_2_1_1_A1_T5_js() { RunFile(@"S15.2.1.1_A1_T5.js"); }
    [TestMethod] public void S15_2_1_1_A2_T1_js() { RunFile(@"S15.2.1.1_A2_T1.js"); }
    [TestMethod] public void S15_2_1_1_A2_T10_js() { RunFile(@"S15.2.1.1_A2_T10.js"); }
    [TestMethod] public void S15_2_1_1_A2_T11_js() { RunFile(@"S15.2.1.1_A2_T11.js"); }
    [TestMethod] public void S15_2_1_1_A2_T12_js() { RunFile(@"S15.2.1.1_A2_T12.js"); }
    [TestMethod] public void S15_2_1_1_A2_T13_js() { RunFile(@"S15.2.1.1_A2_T13.js"); }
    [TestMethod] public void S15_2_1_1_A2_T14_js() { RunFile(@"S15.2.1.1_A2_T14.js"); }
    [TestMethod] public void S15_2_1_1_A2_T2_js() { RunFile(@"S15.2.1.1_A2_T2.js"); }
    [TestMethod] public void S15_2_1_1_A2_T3_js() { RunFile(@"S15.2.1.1_A2_T3.js"); }
    [TestMethod] public void S15_2_1_1_A2_T4_js() { RunFile(@"S15.2.1.1_A2_T4.js"); }
    [TestMethod] public void S15_2_1_1_A2_T5_js() { RunFile(@"S15.2.1.1_A2_T5.js"); }
    [TestMethod] public void S15_2_1_1_A2_T6_js() { RunFile(@"S15.2.1.1_A2_T6.js"); }
    [TestMethod] public void S15_2_1_1_A2_T7_js() { RunFile(@"S15.2.1.1_A2_T7.js"); }
    [TestMethod] public void S15_2_1_1_A2_T8_js() { RunFile(@"S15.2.1.1_A2_T8.js"); }
    [TestMethod] public void S15_2_1_1_A2_T9_js() { RunFile(@"S15.2.1.1_A2_T9.js"); }
    [TestMethod] public void S15_2_1_1_A3_T1_js() { RunFile(@"S15.2.1.1_A3_T1.js"); }
    [TestMethod] public void S15_2_1_1_A3_T2_js() { RunFile(@"S15.2.1.1_A3_T2.js"); }
    [TestMethod] public void S15_2_1_1_A3_T3_js() { RunFile(@"S15.2.1.1_A3_T3.js"); }
  }
}
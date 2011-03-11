using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_1_The_Global_Object_15_1_2_Function_Properties_of_the_Global_Object_15_1_2_1_eval : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.1_The_Global_Object\15.1.2_Function_Properties_of_the_Global_Object\15.1.2.1_eval"); }
    [TestMethod] public void S15_1_2_1_A1_1_T1_js() { RunFile(@"S15.1.2.1_A1.1_T1.js"); }
    [TestMethod] public void S15_1_2_1_A1_1_T2_js() { RunFile(@"S15.1.2.1_A1.1_T2.js"); }
    [TestMethod] public void S15_1_2_1_A1_2_T1_js() { RunFile(@"S15.1.2.1_A1.2_T1.js"); }
    [TestMethod] public void S15_1_2_1_A2_T1_js() { RunFile(@"S15.1.2.1_A2_T1.js"); }
    [TestMethod] public void S15_1_2_1_A2_T2_js() { RunFile(@"S15.1.2.1_A2_T2.js"); }
    [TestMethod] public void S15_1_2_1_A3_1_T1_js() { RunFile(@"S15.1.2.1_A3.1_T1.js"); }
    [TestMethod] public void S15_1_2_1_A3_1_T2_js() { RunFile(@"S15.1.2.1_A3.1_T2.js"); }
    [TestMethod] public void S15_1_2_1_A3_2_T1_js() { RunFile(@"S15.1.2.1_A3.2_T1.js"); }
    [TestMethod] public void S15_1_2_1_A3_2_T2_js() { RunFile(@"S15.1.2.1_A3.2_T2.js"); }
    [TestMethod] public void S15_1_2_1_A3_2_T3_js() { RunFile(@"S15.1.2.1_A3.2_T3.js"); }
    [TestMethod] public void S15_1_2_1_A3_2_T4_js() { RunFile(@"S15.1.2.1_A3.2_T4.js"); }
    [TestMethod] public void S15_1_2_1_A3_2_T5_js() { RunFile(@"S15.1.2.1_A3.2_T5.js"); }
    [TestMethod] public void S15_1_2_1_A3_2_T6_js() { RunFile(@"S15.1.2.1_A3.2_T6.js"); }
    [TestMethod] public void S15_1_2_1_A3_2_T7_js() { RunFile(@"S15.1.2.1_A3.2_T7.js"); }
    [TestMethod] public void S15_1_2_1_A3_2_T8_js() { RunFile(@"S15.1.2.1_A3.2_T8.js"); }
    [TestMethod] public void S15_1_2_1_A3_3_T1_js() { RunFile(@"S15.1.2.1_A3.3_T1.js"); }
    [TestMethod] public void S15_1_2_1_A3_3_T2_js() { RunFile(@"S15.1.2.1_A3.3_T2.js"); }
    [TestMethod] public void S15_1_2_1_A3_3_T3_js() { RunFile(@"S15.1.2.1_A3.3_T3.js"); }
    [TestMethod] public void S15_1_2_1_A3_3_T4_js() { RunFile(@"S15.1.2.1_A3.3_T4.js"); }
    [TestMethod] public void S15_1_2_1_A4_1_js() { RunFile(@"S15.1.2.1_A4.1.js"); }
    [TestMethod] public void S15_1_2_1_A4_2_js() { RunFile(@"S15.1.2.1_A4.2.js"); }
    [TestMethod] public void S15_1_2_1_A4_3_js() { RunFile(@"S15.1.2.1_A4.3.js"); }
    [TestMethod] public void S15_1_2_1_A4_4_js() { RunFile(@"S15.1.2.1_A4.4.js"); }
    [TestMethod] public void S15_1_2_1_A4_5_js() { RunFile(@"S15.1.2.1_A4.5.js"); }
    [TestMethod] public void S15_1_2_1_A4_6_js() { RunFile(@"S15.1.2.1_A4.6.js"); }
    [TestMethod] public void S15_1_2_1_A4_7_js() { RunFile(@"S15.1.2.1_A4.7.js"); }
  }
}
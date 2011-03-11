using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_6_Boolean_Objects_15_6_4_Properties_of_the_Boolean_Prototype_Object : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.6_Boolean_Objects\15.6.4_Properties_of_the_Boolean_Prototype_Object"); }
    [TestMethod] public void S15_6_4_1_A1_js() { RunFile(@"S15.6.4.1_A1.js"); }
    [TestMethod] public void S15_6_4_2_A1_T1_js() { RunFile(@"S15.6.4.2_A1_T1.js"); }
    [TestMethod] public void S15_6_4_2_A1_T2_js() { RunFile(@"S15.6.4.2_A1_T2.js"); }
    [TestMethod] public void S15_6_4_2_A2_T1_js() { RunFile(@"S15.6.4.2_A2_T1.js"); }
    [TestMethod] public void S15_6_4_2_A2_T2_js() { RunFile(@"S15.6.4.2_A2_T2.js"); }
    [TestMethod] public void S15_6_4_2_A2_T3_js() { RunFile(@"S15.6.4.2_A2_T3.js"); }
    [TestMethod] public void S15_6_4_2_A2_T4_js() { RunFile(@"S15.6.4.2_A2_T4.js"); }
    [TestMethod] public void S15_6_4_2_A2_T5_js() { RunFile(@"S15.6.4.2_A2_T5.js"); }
    [TestMethod] public void S15_6_4_3_A1_T1_js() { RunFile(@"S15.6.4.3_A1_T1.js"); }
    [TestMethod] public void S15_6_4_3_A1_T2_js() { RunFile(@"S15.6.4.3_A1_T2.js"); }
    [TestMethod] public void S15_6_4_3_A2_T1_js() { RunFile(@"S15.6.4.3_A2_T1.js"); }
    [TestMethod] public void S15_6_4_3_A2_T2_js() { RunFile(@"S15.6.4.3_A2_T2.js"); }
    [TestMethod] public void S15_6_4_3_A2_T3_js() { RunFile(@"S15.6.4.3_A2_T3.js"); }
    [TestMethod] public void S15_6_4_3_A2_T4_js() { RunFile(@"S15.6.4.3_A2_T4.js"); }
    [TestMethod] public void S15_6_4_3_A2_T5_js() { RunFile(@"S15.6.4.3_A2_T5.js"); }
    [TestMethod] public void S15_6_4_A1_js() { RunFile(@"S15.6.4_A1.js"); }
    [TestMethod] public void S15_6_4_A2_js() { RunFile(@"S15.6.4_A2.js"); }
  }
}
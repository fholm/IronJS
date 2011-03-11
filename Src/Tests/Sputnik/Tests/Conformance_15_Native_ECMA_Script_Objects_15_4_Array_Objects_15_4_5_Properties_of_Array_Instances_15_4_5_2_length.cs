using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_4_Array_Objects_15_4_5_Properties_of_Array_Instances_15_4_5_2_length : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.4_Array_Objects\15.4.5_Properties_of_Array_Instances\15.4.5.2_length"); }
    [TestMethod] public void S15_4_5_2_A1_T1_js() { RunFile(@"S15.4.5.2_A1_T1.js"); }
    [TestMethod] public void S15_4_5_2_A1_T2_js() { RunFile(@"S15.4.5.2_A1_T2.js"); }
    [TestMethod] public void S15_4_5_2_A2_T1_js() { RunFile(@"S15.4.5.2_A2_T1.js"); }
    [TestMethod] public void S15_4_5_2_A3_T1_js() { RunFile(@"S15.4.5.2_A3_T1.js"); }
    [TestMethod] public void S15_4_5_2_A3_T2_js() { RunFile(@"S15.4.5.2_A3_T2.js"); }
    [TestMethod] public void S15_4_5_2_A3_T3_js() { RunFile(@"S15.4.5.2_A3_T3.js"); }
    [TestMethod] public void S15_4_5_2_A3_T4_js() { RunFile(@"S15.4.5.2_A3_T4.js"); }
  }
}
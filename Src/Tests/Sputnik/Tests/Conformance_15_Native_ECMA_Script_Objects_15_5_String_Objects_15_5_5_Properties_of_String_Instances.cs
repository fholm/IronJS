using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_5_String_Objects_15_5_5_Properties_of_String_Instances : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.5_String_Objects\15.5.5_Properties_of_String_Instances"); }
    [TestMethod] public void S15_5_5_1_A1_js() { RunFile(@"S15.5.5.1_A1.js"); }
    [TestMethod] public void S15_5_5_1_A2_js() { RunFile(@"S15.5.5.1_A2.js"); }
    [TestMethod] public void S15_5_5_1_A3_js() { RunFile(@"S15.5.5.1_A3.js"); }
    [TestMethod] public void S15_5_5_1_A4_js() { RunFile(@"S15.5.5.1_A4.js"); }
    [TestMethod] public void S15_5_5_1_A5_js() { RunFile(@"S15.5.5.1_A5.js"); }
    [TestMethod] public void S15_5_5_A1_T1_js() { RunFile(@"S15.5.5_A1_T1.js"); }
    [TestMethod] public void S15_5_5_A1_T2_js() { RunFile(@"S15.5.5_A1_T2.js"); }
    [TestMethod] public void S15_5_5_A2_T1_js() { RunFile(@"S15.5.5_A2_T1.js"); }
    [TestMethod] public void S15_5_5_A2_T2_js() { RunFile(@"S15.5.5_A2_T2.js"); }
  }
}
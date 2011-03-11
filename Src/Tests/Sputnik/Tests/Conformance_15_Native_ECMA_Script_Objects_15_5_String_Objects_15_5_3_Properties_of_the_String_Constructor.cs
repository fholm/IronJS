using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_15_Native_ECMA_Script_Objects_15_5_String_Objects_15_5_3_Properties_of_the_String_Constructor : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\15_Native_ECMA_Script_Objects\15.5_String_Objects\15.5.3_Properties_of_the_String_Constructor"); }
    [TestMethod] public void S15_5_3_1_A1_js() { RunFile(@"S15.5.3.1_A1.js"); }
    [TestMethod] public void S15_5_3_1_A2_js() { RunFile(@"S15.5.3.1_A2.js"); }
    [TestMethod] public void S15_5_3_1_A3_js() { RunFile(@"S15.5.3.1_A3.js"); }
    [TestMethod] public void S15_5_3_1_A4_js() { RunFile(@"S15.5.3.1_A4.js"); }
    [TestMethod] public void S15_5_3_2_A1_js() { RunFile(@"S15.5.3.2_A1.js"); }
    [TestMethod] public void S15_5_3_2_A2_js() { RunFile(@"S15.5.3.2_A2.js"); }
    [TestMethod] public void S15_5_3_2_A3_T1_js() { RunFile(@"S15.5.3.2_A3_T1.js"); }
    [TestMethod] public void S15_5_3_2_A3_T2_js() { RunFile(@"S15.5.3.2_A3_T2.js"); }
    [TestMethod] public void S15_5_3_2_A4_js() { RunFile(@"S15.5.3.2_A4.js"); }
    [TestMethod] public void S15_5_3_A1_js() { RunFile(@"S15.5.3_A1.js"); }
    [TestMethod] public void S15_5_3_A2_T1_js() { RunFile(@"S15.5.3_A2_T1.js"); }
    [TestMethod] public void S15_5_3_A2_T2_js() { RunFile(@"S15.5.3_A2_T2.js"); }
  }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_09_Type_Conversion_9_1_ToPrimitive : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\09_Type_Conversion\9.1_ToPrimitive"); }
    [TestMethod] public void S9_1_A1_T1_js() { RunFile(@"S9.1_A1_T1.js"); }
    [TestMethod] public void S9_1_A1_T2_js() { RunFile(@"S9.1_A1_T2.js"); }
    [TestMethod] public void S9_1_A1_T3_js() { RunFile(@"S9.1_A1_T3.js"); }
    [TestMethod] public void S9_1_A1_T4_js() { RunFile(@"S9.1_A1_T4.js"); }
  }
}
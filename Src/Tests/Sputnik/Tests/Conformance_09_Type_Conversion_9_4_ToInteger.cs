using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_09_Type_Conversion_9_4_ToInteger : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\09_Type_Conversion\9.4_ToInteger"); }
    [TestMethod] public void S9_4_A1_js() { RunFile(@"S9.4_A1.js"); }
    [TestMethod] public void S9_4_A2_js() { RunFile(@"S9.4_A2.js"); }
    [TestMethod] public void S9_4_A3_T1_js() { RunFile(@"S9.4_A3_T1.js"); }
    [TestMethod] public void S9_4_A3_T2_js() { RunFile(@"S9.4_A3_T2.js"); }
  }
}
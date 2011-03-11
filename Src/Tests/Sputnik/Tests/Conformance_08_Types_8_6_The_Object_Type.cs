using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_08_Types_8_6_The_Object_Type : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\08_Types\8.6_The_Object_Type"); }
    [TestMethod] public void S8_6_A2_T1_js() { RunFile(@"S8.6_A2_T1.js"); }
    [TestMethod] public void S8_6_A2_T2_js() { RunFile(@"S8.6_A2_T2.js"); }
    [TestMethod] public void S8_6_A3_T1_js() { RunFile(@"S8.6_A3_T1.js"); }
    [TestMethod] public void S8_6_A3_T2_js() { RunFile(@"S8.6_A3_T2.js"); }
    [TestMethod] public void S8_6_A4_T1_js() { RunFile(@"S8.6_A4_T1.js"); }
  }
}
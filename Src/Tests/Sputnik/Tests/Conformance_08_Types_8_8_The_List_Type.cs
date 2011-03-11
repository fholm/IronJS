using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_08_Types_8_8_The_List_Type : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\08_Types\8.8_The_List_Type"); }
    [TestMethod] public void S8_8_A2_T1_js() { RunFile(@"S8.8_A2_T1.js"); }
    [TestMethod] public void S8_8_A2_T2_js() { RunFile(@"S8.8_A2_T2.js"); }
    [TestMethod] public void S8_8_A2_T3_js() { RunFile(@"S8.8_A2_T3.js"); }
  }
}
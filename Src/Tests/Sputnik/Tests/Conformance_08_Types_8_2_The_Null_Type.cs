using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_08_Types_8_2_The_Null_Type : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\08_Types\8.2_The_Null_Type"); }
    [TestMethod] public void S8_2_A1_T1_js() { RunFile(@"S8.2_A1_T1.js"); }
    [TestMethod] public void S8_2_A1_T2_js() { RunFile(@"S8.2_A1_T2.js"); }
    [TestMethod] public void S8_2_A2_js() { RunFile_ExpectException(@"S8.2_A2.js"); }
    [TestMethod] public void S8_2_A3_js() { RunFile(@"S8.2_A3.js"); }
  }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_07_Lexical_Conventions_7_8_Literals_7_8_2_Boolean_Literals : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\07_Lexical_Conventions\7.8_Literals\7.8.2_Boolean_Literals"); }
    [TestMethod] public void S7_8_2_A1_T1_js() { RunFile(@"S7.8.2_A1_T1.js"); }
    [TestMethod] public void S7_8_2_A1_T2_js() { RunFile(@"S7.8.2_A1_T2.js"); }
  }
}
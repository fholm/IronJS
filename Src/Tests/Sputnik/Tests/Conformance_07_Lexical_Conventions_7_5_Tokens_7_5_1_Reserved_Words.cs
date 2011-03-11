using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_07_Lexical_Conventions_7_5_Tokens_7_5_1_Reserved_Words : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\07_Lexical_Conventions\7.5_Tokens\7.5.1_Reserved_Words"); }
    [TestMethod] public void S7_5_1_A1_1_js() { RunFile(@"S7.5.1_A1.1.js"); }
    [TestMethod] public void S7_5_1_A1_2_js() { RunFile(@"S7.5.1_A1.2.js"); }
    [TestMethod] public void S7_5_1_A1_3_js() { RunFile(@"S7.5.1_A1.3.js"); }
    [TestMethod] public void S7_5_1_A2_js() { RunFile(@"S7.5.1_A2.js"); }
  }
}
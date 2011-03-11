using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_07_Lexical_Conventions_7_9_Automatic_Semicolon_Insertion_7_9_2_Examples_of_Automatic_Semicolon_Insertion : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\07_Lexical_Conventions\7.9_Automatic_Semicolon_Insertion\7.9.2_Examples_of_Automatic_Semicolon_Insertion"); }
    [TestMethod] public void S7_9_2_A1_T1_js() { RunFile(@"S7.9.2_A1_T1.js"); }
    [TestMethod] public void S7_9_2_A1_T2_js() { RunFile(@"S7.9.2_A1_T2.js"); }
    [TestMethod] public void S7_9_2_A1_T3_js() { RunFile(@"S7.9.2_A1_T3.js"); }
    [TestMethod] public void S7_9_2_A1_T4_js() { RunFile(@"S7.9.2_A1_T4.js"); }
    [TestMethod] public void S7_9_2_A1_T5_js() { RunFile(@"S7.9.2_A1_T5.js"); }
    [TestMethod] public void S7_9_2_A1_T6_js() { RunFile(@"S7.9.2_A1_T6.js"); }
    [TestMethod] public void S7_9_2_A1_T7_js() { RunFile(@"S7.9.2_A1_T7.js"); }
  }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_07_Lexical_Conventions_7_5_Tokens_7_5_2_Keywords : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\07_Lexical_Conventions\7.5_Tokens\7.5.2_Keywords"); }
    [TestMethod] public void S7_5_2_A1_1_js() { RunFile_ExpectException<IronJS.Support.CompilerError>(@"S7.5.2_A1.1.js"); }
    [TestMethod] public void S7_5_2_A1_10_js() { RunFile_ExpectException<IronJS.Support.CompilerError>(@"S7.5.2_A1.10.js"); }
    [TestMethod] public void S7_5_2_A1_11_js() { RunFile_ExpectException<IronJS.Support.CompilerError>(@"S7.5.2_A1.11.js"); }
    [TestMethod] public void S7_5_2_A1_12_js() { RunFile_ExpectException<IronJS.Support.CompilerError>(@"S7.5.2_A1.12.js"); }
    [TestMethod] public void S7_5_2_A1_13_js() { RunFile(@"S7.5.2_A1.13.js"); }
    [TestMethod] public void S7_5_2_A1_14_js() { RunFile(@"S7.5.2_A1.14.js"); }
    [TestMethod] public void S7_5_2_A1_15_js() { RunFile(@"S7.5.2_A1.15.js"); }
    [TestMethod] public void S7_5_2_A1_16_js() { RunFile_ExpectException<IronJS.Support.CompilerError>(@"S7.5.2_A1.16.js"); }
    [TestMethod] public void S7_5_2_A1_17_js() { RunFile_ExpectException<IronJS.Support.CompilerError>(@"S7.5.2_A1.17.js"); }
    [TestMethod] public void S7_5_2_A1_18_js() { RunFile_ExpectException<IronJS.Support.CompilerError>(@"S7.5.2_A1.18.js"); }
    [TestMethod] public void S7_5_2_A1_19_js() { RunFile_ExpectException<IronJS.Support.CompilerError>(@"S7.5.2_A1.19.js"); }
    [TestMethod] public void S7_5_2_A1_2_js() { RunFile(@"S7.5.2_A1.2.js"); }
    [TestMethod] public void S7_5_2_A1_20_js() { RunFile_ExpectException<IronJS.Support.CompilerError>(@"S7.5.2_A1.20.js"); }
    [TestMethod] public void S7_5_2_A1_21_js() { RunFile_ExpectException<IronJS.Support.CompilerError>(@"S7.5.2_A1.21.js"); }
    [TestMethod] public void S7_5_2_A1_22_js() { RunFile(@"S7.5.2_A1.22.js"); }
    [TestMethod] public void S7_5_2_A1_23_js() { RunFile_ExpectException<IronJS.Support.CompilerError>(@"S7.5.2_A1.23.js"); }
    [TestMethod] public void S7_5_2_A1_24_js() { RunFile_ExpectException<IronJS.Support.CompilerError>(@"S7.5.2_A1.24.js"); }
    [TestMethod] public void S7_5_2_A1_25_js() { RunFile_ExpectException<IronJS.Support.CompilerError>(@"S7.5.2_A1.25.js"); }
    [TestMethod] public void S7_5_2_A1_3_js() { RunFile(@"S7.5.2_A1.3.js"); }
    [TestMethod] public void S7_5_2_A1_4_js() { RunFile_ExpectException<IronJS.Support.CompilerError>(@"S7.5.2_A1.4.js"); }
    [TestMethod] public void S7_5_2_A1_5_js() { RunFile(@"S7.5.2_A1.5.js"); }
    [TestMethod] public void S7_5_2_A1_6_js() { RunFile_ExpectException<IronJS.Support.CompilerError>(@"S7.5.2_A1.6.js"); }
    [TestMethod] public void S7_5_2_A1_7_js() { RunFile_ExpectException<IronJS.Support.CompilerError>(@"S7.5.2_A1.7.js"); }
    [TestMethod] public void S7_5_2_A1_8_js() { RunFile(@"S7.5.2_A1.8.js"); }
    [TestMethod] public void S7_5_2_A1_9_js() { RunFile(@"S7.5.2_A1.9.js"); }
  }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Unicode_Unicode_500 : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Unicode\Unicode_500"); }
    [TestMethod] public void S15_10_2_12_A1_T6_js() { RunFile(@"S15.10.2.12_A1_T6.js"); }
    [TestMethod] public void S15_10_2_12_A2_T6_js() { RunFile(@"S15.10.2.12_A2_T6.js"); }
    [TestMethod] public void S15_5_4_16_A1_js() { RunFile(@"S15.5.4.16_A1.js"); }
    [TestMethod] public void S15_5_4_16_A2_js() { RunFile(@"S15.5.4.16_A2.js"); }
    [TestMethod] public void S15_5_4_18_A1_js() { RunFile(@"S15.5.4.18_A1.js"); }
    [TestMethod] public void S15_5_4_18_A2_js() { RunFile(@"S15.5.4.18_A2.js"); }
    [TestMethod] public void S7_1_A1_T1_js() { RunFile(@"S7.1_A1_T1.js"); }
    [TestMethod] public void S7_1_A2_1_T1_js() { RunFile(@"S7.1_A2.1_T1.js"); }
    [TestMethod] public void S7_1_A2_1_T2_js() { RunFile(@"S7.1_A2.1_T2.js"); }
    [TestMethod] public void S7_1_A2_2_T1_js() { RunFile(@"S7.1_A2.2_T1.js"); }
    [TestMethod] public void S7_1_A2_2_T2_js() { RunFile(@"S7.1_A2.2_T2.js"); }
    [TestMethod] public void S7_2_A1_6_T1_js() { RunFile(@"S7.2_A1.6_T1.js"); }
    [TestMethod] public void S7_2_A2_6_T1_js() { RunFile(@"S7.2_A2.6_T1.js"); }
    [TestMethod] public void S7_2_A3_6_T1_js() { RunFile(@"S7.2_A3.6_T1.js"); }
    [TestMethod] public void S7_2_A4_6_T1_js() { RunFile(@"S7.2_A4.6_T1.js"); }
    [TestMethod] public void S7_6_A1_1_T1_js() { RunFile(@"S7.6_A1.1_T1.js"); }
    [TestMethod] public void S7_6_A1_1_T2_js() { RunFile(@"S7.6_A1.1_T2.js"); }
    [TestMethod] public void S7_6_A1_1_T3_js() { RunFile(@"S7.6_A1.1_T3.js"); }
    [TestMethod] public void S7_6_A1_1_T4_js() { RunFile(@"S7.6_A1.1_T4.js"); }
    [TestMethod] public void S7_6_A1_1_T5_js() { RunFile(@"S7.6_A1.1_T5.js"); }
    [TestMethod] public void S7_6_A1_1_T6_js() { RunFile(@"S7.6_A1.1_T6.js"); }
    [TestMethod] public void S7_6_A1_4_T1_js() { RunFile(@"S7.6_A1.4_T1.js"); }
    [TestMethod] public void S7_6_A1_4_T2_js() { RunFile(@"S7.6_A1.4_T2.js"); }
    [TestMethod] public void S7_6_A1_4_T3_js() { RunFile(@"S7.6_A1.4_T3.js"); }
    [TestMethod] public void S7_6_A1_4_T4_js() { RunFile(@"S7.6_A1.4_T4.js"); }
    [TestMethod] public void S7_6_A2_2_T1_js() { RunFile(@"S7.6_A2.2_T1.js"); }
    [TestMethod] public void S7_6_A2_2_T2_js() { RunFile(@"S7.6_A2.2_T2.js"); }
    [TestMethod] public void S7_6_A2_3_js() { RunFile(@"S7.6_A2.3.js"); }
    [TestMethod] public void S7_6_A2_4_js() { RunFile(@"S7.6_A2.4.js"); }
    [TestMethod] public void S7_6_A3_1_js() { RunFile(@"S7.6_A3.1.js"); }
    [TestMethod] public void S7_6_A3_2_js() { RunFile(@"S7.6_A3.2.js"); }
    [TestMethod] public void S7_6_A5_2_T1_js() { RunFile(@"S7.6_A5.2_T1.js"); }
    [TestMethod] public void S7_6_A5_2_T10_js() { RunFile(@"S7.6_A5.2_T10.js"); }
    [TestMethod] public void S7_6_A5_2_T2_js() { RunFile(@"S7.6_A5.2_T2.js"); }
    [TestMethod] public void S7_6_A5_2_T3_js() { RunFile(@"S7.6_A5.2_T3.js"); }
    [TestMethod] public void S7_6_A5_2_T4_js() { RunFile(@"S7.6_A5.2_T4.js"); }
    [TestMethod] public void S7_6_A5_2_T5_js() { RunFile(@"S7.6_A5.2_T5.js"); }
    [TestMethod] public void S7_6_A5_2_T6_js() { RunFile(@"S7.6_A5.2_T6.js"); }
    [TestMethod] public void S7_6_A5_2_T7_js() { RunFile(@"S7.6_A5.2_T7.js"); }
    [TestMethod] public void S7_6_A5_2_T8_js() { RunFile(@"S7.6_A5.2_T8.js"); }
    [TestMethod] public void S7_6_A5_2_T9_js() { RunFile(@"S7.6_A5.2_T9.js"); }
    [TestMethod] public void S7_6_A5_3_T1_js() { RunFile(@"S7.6_A5.3_T1.js"); }
    [TestMethod] public void S7_6_A5_3_T2_js() { RunFile(@"S7.6_A5.3_T2.js"); }
  }
}
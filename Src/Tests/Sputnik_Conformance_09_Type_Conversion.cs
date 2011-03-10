using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests {
  [TestClass]
  public class Sputnik_Conformance_09_Type_Conversion : Sputnik {
    [TestMethod]
    public void Conformance_09_Type_Conversion_9_1_ToPrimitive_S9_1_A1_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.1_ToPrimitive", () => { RunFile("S9.1_A1_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_1_ToPrimitive_S9_1_A1_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.1_ToPrimitive", () => { RunFile("S9.1_A1_T2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_1_ToPrimitive_S9_1_A1_T3_js() {
      Test(@"Conformance\09_Type_Conversion\9.1_ToPrimitive", () => { RunFile("S9.1_A1_T3.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_1_ToPrimitive_S9_1_A1_T4_js() {
      Test(@"Conformance\09_Type_Conversion\9.1_ToPrimitive", () => { RunFile("S9.1_A1_T4.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_2_ToBoolean_S9_2_A1_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.2_ToBoolean", () => { RunFile("S9.2_A1_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_2_ToBoolean_S9_2_A1_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.2_ToBoolean", () => { RunFile("S9.2_A1_T2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_2_ToBoolean_S9_2_A2_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.2_ToBoolean", () => { RunFile("S9.2_A2_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_2_ToBoolean_S9_2_A2_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.2_ToBoolean", () => { RunFile("S9.2_A2_T2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_2_ToBoolean_S9_2_A3_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.2_ToBoolean", () => { RunFile("S9.2_A3_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_2_ToBoolean_S9_2_A3_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.2_ToBoolean", () => { RunFile("S9.2_A3_T2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_2_ToBoolean_S9_2_A4_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.2_ToBoolean", () => { RunFile("S9.2_A4_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_2_ToBoolean_S9_2_A4_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.2_ToBoolean", () => { RunFile("S9.2_A4_T2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_2_ToBoolean_S9_2_A4_T3_js() {
      Test(@"Conformance\09_Type_Conversion\9.2_ToBoolean", () => { RunFile("S9.2_A4_T3.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_2_ToBoolean_S9_2_A4_T4_js() {
      Test(@"Conformance\09_Type_Conversion\9.2_ToBoolean", () => { RunFile("S9.2_A4_T4.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_2_ToBoolean_S9_2_A5_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.2_ToBoolean", () => { RunFile("S9.2_A5_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_2_ToBoolean_S9_2_A5_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.2_ToBoolean", () => { RunFile("S9.2_A5_T2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_2_ToBoolean_S9_2_A5_T3_js() {
      Test(@"Conformance\09_Type_Conversion\9.2_ToBoolean", () => { RunFile("S9.2_A5_T3.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_2_ToBoolean_S9_2_A5_T4_js() {
      Test(@"Conformance\09_Type_Conversion\9.2_ToBoolean", () => { RunFile("S9.2_A5_T4.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_2_ToBoolean_S9_2_A6_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.2_ToBoolean", () => { RunFile("S9.2_A6_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_2_ToBoolean_S9_2_A6_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.2_ToBoolean", () => { RunFile("S9.2_A6_T2.js"); });
    }
    [TestMethod]
    public void Conformance_09_Type_Conversion_9_3_ToNumber_S9_3_A1_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.3_ToNumber", () => { RunFile("S9.3_A1_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_3_ToNumber_S9_3_A1_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.3_ToNumber", () => { RunFile("S9.3_A1_T2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_3_ToNumber_S9_3_A2_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.3_ToNumber", () => { RunFile("S9.3_A2_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_3_ToNumber_S9_3_A2_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.3_ToNumber", () => { RunFile("S9.3_A2_T2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_3_ToNumber_S9_3_A3_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.3_ToNumber", () => { RunFile("S9.3_A3_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_3_ToNumber_S9_3_A3_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.3_ToNumber", () => { RunFile("S9.3_A3_T2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_3_ToNumber_S9_3_A4_1_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.3_ToNumber", () => { RunFile("S9.3_A4.1_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_3_ToNumber_S9_3_A4_1_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.3_ToNumber", () => { RunFile("S9.3_A4.1_T2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_3_ToNumber_S9_3_A4_2_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.3_ToNumber", () => { RunFile("S9.3_A4.2_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_3_ToNumber_S9_3_A4_2_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.3_ToNumber", () => { RunFile("S9.3_A4.2_T2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_3_ToNumber_S9_3_A5_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.3_ToNumber", () => { RunFile("S9.3_A5_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_3_ToNumber_S9_3_A5_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.3_ToNumber", () => { RunFile("S9.3_A5_T2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_4_ToInteger_S9_4_A1_js() {
      Test(@"Conformance\09_Type_Conversion\9.4_ToInteger", () => { RunFile("S9.4_A1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_4_ToInteger_S9_4_A2_js() {
      Test(@"Conformance\09_Type_Conversion\9.4_ToInteger", () => { RunFile("S9.4_A2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_4_ToInteger_S9_4_A3_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.4_ToInteger", () => { RunFile("S9.4_A3_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_4_ToInteger_S9_4_A3_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.4_ToInteger", () => { RunFile("S9.4_A3_T2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_5_ToInt32_S9_5_A1_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.5_ToInt32", () => { RunFile("S9.5_A1_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_5_ToInt32_S9_5_A2_1_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.5_ToInt32", () => { RunFile("S9.5_A2.1_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_5_ToInt32_S9_5_A2_1_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.5_ToInt32", () => { RunFile("S9.5_A2.1_T2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_5_ToInt32_S9_5_A2_2_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.5_ToInt32", () => { RunFile("S9.5_A2.2_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_5_ToInt32_S9_5_A2_2_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.5_ToInt32", () => { RunFile("S9.5_A2.2_T2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_5_ToInt32_S9_5_A2_3_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.5_ToInt32", () => { RunFile("S9.5_A2.3_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_5_ToInt32_S9_5_A2_3_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.5_ToInt32", () => { RunFile("S9.5_A2.3_T2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_5_ToInt32_S9_5_A3_1_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.5_ToInt32", () => { RunFile("S9.5_A3.1_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_5_ToInt32_S9_5_A3_1_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.5_ToInt32", () => { RunFile("S9.5_A3.1_T2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_5_ToInt32_S9_5_A3_1_T3_js() {
      Test(@"Conformance\09_Type_Conversion\9.5_ToInt32", () => { RunFile("S9.5_A3.1_T3.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_5_ToInt32_S9_5_A3_1_T4_js() {
      Test(@"Conformance\09_Type_Conversion\9.5_ToInt32", () => { RunFile("S9.5_A3.1_T4.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_5_ToInt32_S9_5_A3_2_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.5_ToInt32", () => { RunFile("S9.5_A3.2_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_5_ToInt32_S9_5_A3_2_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.5_ToInt32", () => { RunFile("S9.5_A3.2_T2.js"); });
    }
    [TestMethod]
    public void Conformance_09_Type_Conversion_9_6_ToUint32_S9_6_A1_js() {
      Test(@"Conformance\09_Type_Conversion\9.6_ToUint32", () => { RunFile("S9.6_A1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_6_ToUint32_S9_6_A2_1_js() {
      Test(@"Conformance\09_Type_Conversion\9.6_ToUint32", () => { RunFile("S9.6_A2.1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_6_ToUint32_S9_6_A2_2_js() {
      Test(@"Conformance\09_Type_Conversion\9.6_ToUint32", () => { RunFile("S9.6_A2.2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_6_ToUint32_S9_6_A3_1_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.6_ToUint32", () => { RunFile("S9.6_A3.1_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_6_ToUint32_S9_6_A3_1_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.6_ToUint32", () => { RunFile("S9.6_A3.1_T2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_6_ToUint32_S9_6_A3_1_T3_js() {
      Test(@"Conformance\09_Type_Conversion\9.6_ToUint32", () => { RunFile("S9.6_A3.1_T3.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_6_ToUint32_S9_6_A3_1_T4_js() {
      Test(@"Conformance\09_Type_Conversion\9.6_ToUint32", () => { RunFile("S9.6_A3.1_T4.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_6_ToUint32_S9_6_A3_2_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.6_ToUint32", () => { RunFile("S9.6_A3.2_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_7_ToUint16_S9_7_A1_js() {
      Test(@"Conformance\09_Type_Conversion\9.7_ToUint16", () => { RunFile("S9.7_A1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_7_ToUint16_S9_7_A2_1_js() {
      Test(@"Conformance\09_Type_Conversion\9.7_ToUint16", () => { RunFile("S9.7_A2.1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_7_ToUint16_S9_7_A2_2_js() {
      Test(@"Conformance\09_Type_Conversion\9.7_ToUint16", () => { RunFile("S9.7_A2.2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_7_ToUint16_S9_7_A3_1_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.7_ToUint16", () => { RunFile("S9.7_A3.1_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_7_ToUint16_S9_7_A3_1_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.7_ToUint16", () => { RunFile("S9.7_A3.1_T2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_7_ToUint16_S9_7_A3_1_T3_js() {
      Test(@"Conformance\09_Type_Conversion\9.7_ToUint16", () => { RunFile("S9.7_A3.1_T3.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_7_ToUint16_S9_7_A3_1_T4_js() {
      Test(@"Conformance\09_Type_Conversion\9.7_ToUint16", () => { RunFile("S9.7_A3.1_T4.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_7_ToUint16_S9_7_A3_2_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.7_ToUint16", () => { RunFile("S9.7_A3.2_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_8_ToString_S9_8_A1_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.8_ToString", () => { RunFile("S9.8_A1_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_8_ToString_S9_8_A1_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.8_ToString", () => { RunFile("S9.8_A1_T2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_8_ToString_S9_8_A2_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.8_ToString", () => { RunFile("S9.8_A2_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_8_ToString_S9_8_A2_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.8_ToString", () => { RunFile("S9.8_A2_T2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_8_ToString_S9_8_A3_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.8_ToString", () => { RunFile("S9.8_A3_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_8_ToString_S9_8_A3_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.8_ToString", () => { RunFile("S9.8_A3_T2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_8_ToString_S9_8_A4_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.8_ToString", () => { RunFile("S9.8_A4_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_8_ToString_S9_8_A4_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.8_ToString", () => { RunFile("S9.8_A4_T2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_8_ToString_S9_8_A5_T1_js() {
      Test(@"Conformance\09_Type_Conversion\9.8_ToString", () => { RunFile("S9.8_A5_T1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_8_ToString_S9_8_A5_T2_js() {
      Test(@"Conformance\09_Type_Conversion\9.8_ToString", () => { RunFile("S9.8_A5_T2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_9_ToObject_S9_9_A1_js() {
      Test(@"Conformance\09_Type_Conversion\9.9_ToObject", () => { RunFile("S9.9_A1.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_9_ToObject_S9_9_A2_js() {
      Test(@"Conformance\09_Type_Conversion\9.9_ToObject", () => { RunFile("S9.9_A2.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_9_ToObject_S9_9_A3_js() {
      Test(@"Conformance\09_Type_Conversion\9.9_ToObject", () => { RunFile("S9.9_A3.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_9_ToObject_S9_9_A4_js() {
      Test(@"Conformance\09_Type_Conversion\9.9_ToObject", () => { RunFile("S9.9_A4.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_9_ToObject_S9_9_A5_js() {
      Test(@"Conformance\09_Type_Conversion\9.9_ToObject", () => { RunFile("S9.9_A5.js"); });
    }

    [TestMethod]
    public void Conformance_09_Type_Conversion_9_9_ToObject_S9_9_A6_js() {
      Test(@"Conformance\09_Type_Conversion\9.9_ToObject", () => { RunFile("S9.9_A6.js"); });
    }

  }
}

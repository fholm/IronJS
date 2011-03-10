using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests {
  [TestClass]
  public class MozillaECMA3 {
    IronJS.Hosting.Context ctx;

    [TestInitialize]
    public void Init() {
      System.IO.Directory.SetCurrentDirectory(@"..\..\..\Tests");
      ctx = IronJS.Hosting.Context.Create();
      ctx.ExecuteFile("MozillaECMA3-shell.js");
      return;
    }

    [TestMethod]
    public void TestMethod1() {

    }

    [TestMethod]
    public void TestMethod2() {

    }
  }
}

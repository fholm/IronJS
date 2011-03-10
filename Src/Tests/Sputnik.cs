using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests {
  public class Sputnik {
    static string baseDir;

    static Sputnik() {
      System.IO.Directory.SetCurrentDirectory(@"..\..\..\Tests");
      baseDir = System.IO.Directory.GetCurrentDirectory();
    }

    void Error(string error) {
      Assert.Fail(error);
    }

    protected IronJS.Hosting.Context Ctx {
      get {
        var ctx = IronJS.Hosting.Context.Create();
        var error = new Action<string>(Error);
        var errorFunc = IronJS.Native.Utils.createHostFunction(ctx.Environment, error);
        ctx.PutGlobal("ERROR", errorFunc);
        ctx.PutGlobal("$ERROR", errorFunc);
        return ctx;
      }
    }

    protected void Test(string testDirectory, Action test) {
      System.IO.Directory.SetCurrentDirectory(@"sputnik\" + testDirectory);

      try {
        test();
      } finally {
        System.IO.Directory.SetCurrentDirectory(baseDir);
      }
    }

    protected void RunFile(string file) {
      var _ = Ctx.ExecuteFile(file);
    }

    protected void RunFile_ExpectException(string file) {
      try {
        var _ = Ctx.ExecuteFile(file);
        Assert.Fail("Expected exception from file " + file);
      } catch {

      }
    }
  }
}


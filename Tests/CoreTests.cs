namespace Craiel.UnityEssentials.Tests
{
    using System;
    using NUnit.Framework;
    using Runtime;

    public static class CoreTests
    {
        [Test]
        public static void RuntimeInfo()
        {
            Assert.NotNull(Runtime.RuntimeInfo.Assembly);
            Assert.AreNotEqual(0, Runtime.RuntimeInfo.ProcessId);
            Assert.NotNull(Runtime.RuntimeInfo.ProcessName);
            
            TestContext.WriteLine(string.Format("{0} | {1}@{2}", Runtime.RuntimeInfo.Assembly, Runtime.RuntimeInfo.ProcessId, Runtime.RuntimeInfo.ProcessName));
            
            Assert.NotNull(Runtime.RuntimeInfo.Path);
            TestContext.WriteLine(string.Format("Path: {0}", Runtime.RuntimeInfo.Path));
            
            Assert.NotNull(Runtime.RuntimeInfo.WorkingDirectory);
            TestContext.WriteLine(string.Format("WorkingDir: {0}", Runtime.RuntimeInfo.WorkingDirectory));
            
            Assert.NotNull(Runtime.RuntimeInfo.SystemDirectory);
            TestContext.WriteLine(string.Format("SystemDir: {0}", Runtime.RuntimeInfo.SystemDirectory));
        }

        [Test]
        public static void UnitTestSupport()
        {
            Assert.IsTrue(UnitTest.IsRunningFromNunit);
            
            TestContext.WriteLine("Unit Test Environment Confirmed");
        }
    }
}
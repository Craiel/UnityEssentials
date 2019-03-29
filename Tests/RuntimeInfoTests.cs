namespace Craiel.UnityEssentials.Tests
{
    using NUnit.Framework;

    public class RuntimeInfoTests
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [Test]
        public static void Run()
        {
            Assert.NotNull(Runtime.RuntimeInfo.Assembly);
            Assert.AreNotEqual(0, Runtime.RuntimeInfo.ProcessId);
            Assert.NotNull(Runtime.RuntimeInfo.ProcessName);
            
            UnitTestUtils.Log("{0} | {1}@{2}", Runtime.RuntimeInfo.Assembly, Runtime.RuntimeInfo.ProcessId, Runtime.RuntimeInfo.ProcessName);
            
            Assert.NotNull(Runtime.RuntimeInfo.Path);
            UnitTestUtils.Log("Path: {0}", Runtime.RuntimeInfo.Path);
            
            Assert.NotNull(Runtime.RuntimeInfo.WorkingDirectory);
            UnitTestUtils.Log("WorkingDir: {0}", Runtime.RuntimeInfo.WorkingDirectory);
            
            Assert.NotNull(Runtime.RuntimeInfo.SystemDirectory);
            UnitTestUtils.Log("SystemDir: {0}", Runtime.RuntimeInfo.SystemDirectory);
            
            Assert.IsTrue(Runtime.RuntimeInfo.RunningFromNUnit);
        }
    }
}
namespace Craiel.UnityEssentials.Runtime
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using Extensions;
    using IO;

    public static class RuntimeInfo
    {
        static RuntimeInfo()
        {
            UpdateRuntimeInfo();
        }

        public static Assembly Assembly { get; private set; }

        public static int ProcessId { get; private set; }

        public static string ProcessName { get; private set; }

        public static string AssemblyName { get; private set; }

        public static ManagedDirectory Path { get; private set; }

        public static ManagedDirectory WorkingDirectory { get; private set; }

        public static ManagedDirectory SystemDirectory { get; private set; }
        
        public static bool RunningFromNUnit { get; private set; }

        private static void UpdateRuntimeInfo()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.ToLowerInvariant().StartsWith("nunit.framework"))
                {
                    RunningFromNUnit = true;
                    break;
                }
            }
            
            WorkingDirectory = new ManagedDirectory(System.IO.Directory.GetCurrentDirectory());

            if (ProcessName == null)
            {
                Process process = Process.GetCurrentProcess();
                ProcessId = process.Id;
                ProcessName = process.ProcessName;
            }

            if (AssemblyName == null)
            {
                Assembly = RunningFromNUnit ? Assembly.GetExecutingAssembly() : Assembly.GetEntryAssembly();
                if (Assembly != null)
                {
                    AssemblyName = System.IO.Path.GetFileName(Assembly.Location);
                    Path = Assembly.GetDirectory();
                }
            }

            SystemDirectory = new ManagedDirectory(Environment.GetFolderPath(Environment.SpecialFolder.System));
            
            
        }
    }
}

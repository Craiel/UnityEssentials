namespace Craiel.UnityEssentials.Runtime.Logging
{
    using IO;
    using NLog;
    using NLog.Config;
    using NLog.Targets;

    public static class NLogUtils
    {
        private const string ArchiveFolderName = "Archive";
        private const string LogsFolderName = "Logs";
        private const string DefaultLogFormat = @"${date:format=HH\:mm\:ss.fff} [${logger}:${threadid}] ${level} ${message}";
        private const string ExceptionLogFormat = @"
    ${exception:format=tostring} 
    ${stacktrace:format=DetailedFlat:topFrames=5}
";

        public static void ConfigureNLog(string dataPath, params TargetWithLayout[] customTargets)
        {
            var target = new ManagedDirectory(dataPath).ToDirectory(LogsFolderName);
            target.Create();

            // Step 1. Create configuration object 
            var config = new LoggingConfiguration();

            foreach (TargetWithLayout customTarget in customTargets)
            {
                config.AddTarget("interceptor", customTarget);
            }

            ManagedFile allFile = target.ToFile("All.log");
            FileTarget allTarget = CreateDefaultFileTarget(allFile, DefaultLogFormat);
            config.AddTarget("all_file", allTarget);

            ManagedFile warnFile = target.ToFile("Warning.log");
            FileTarget warnTarget = CreateDefaultFileTarget(warnFile, DefaultLogFormat);
            config.AddTarget("warn_file", warnTarget);

            ManagedFile errorFile = target.ToFile("Error.log");
            FileTarget errorTarget = CreateDefaultFileTarget(errorFile, DefaultLogFormat + ExceptionLogFormat);
            config.AddTarget("err_file", errorTarget);

            // Configure the targets
            var rule = new LoggingRule("*", LogLevel.Debug, allTarget);
            config.LoggingRules.Add(rule);

            rule = new LoggingRule("*", warnTarget);
            rule.EnableLoggingForLevel(LogLevel.Warn);
            config.LoggingRules.Add(rule);

            rule = new LoggingRule("*", LogLevel.Error, errorTarget);
            config.LoggingRules.Add(rule);

            foreach (TargetWithLayout customTarget in customTargets)
            {
                rule = new LoggingRule("*", LogLevel.Debug, customTarget);
                config.LoggingRules.Add(rule);
            }

            // Step 5. Activate the configuration
            LogManager.Configuration = config;

            EssentialsCore.Logger.Info("NLOG DATAPATH: " + target);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static FileTarget CreateDefaultFileTarget(ManagedFile file, string layout)
        {
            return new FileTarget
            {
                ArchiveAboveSize = EssentialsConstants.LogFileArchiveSize,
                MaxArchiveFiles = EssentialsConstants.LogFileArchiveCount,
                KeepFileOpen = true,
                ConcurrentWrites = true,
                ArchiveOldFileOnStartup = true,
                ArchiveFileName = file.GetDirectory().ToDirectory(ArchiveFolderName).ToFile(file.FileNameWithoutExtension + ".{#######}.log").GetUnityPath(),
                FileName = file.GetUnityPath(),
                Layout = layout
            };
        }
    }
}
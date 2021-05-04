namespace Craiel.UnityEssentials.Runtime.Logging
{
    using Singletons;
    using UnityEngine;

    public class UnityNLogRelay : UnitySingleton<UnityNLogRelay>
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        static UnityNLogRelay()
        {
            Application.logMessageReceivedThreaded += OnUnityLogMessage;

            UnityEngine.Debug.Log("Unity NLog Relay Initialized");
        }

        private static void OnUnityLogMessage(string message, string stacktrace, LogType type)
        {
            switch (type)
            {
                case LogType.Assert:
                case LogType.Error:
                    {
                        if (string.IsNullOrEmpty(stacktrace))
                        {
                            EssentialsCore.Logger.Error(message);
                        }
                        else
                        {
                            EssentialsCore.Logger.Error(new UnityStackTraceException(stacktrace), message);
                        }

                        break;
                    }

                case LogType.Warning:
                    {
                        if (string.IsNullOrEmpty(stacktrace))
                        {
                            EssentialsCore.Logger.Warn(message);
                        }
                        else
                        {
                            EssentialsCore.Logger.Warn(new UnityStackTraceException(stacktrace), message);
                        }

                        break;
                    }

                case LogType.Log:
                    {
                        if (string.IsNullOrEmpty(stacktrace))
                        {
                            EssentialsCore.Logger.Info(message);
                        }
                        else
                        {
                            EssentialsCore.Logger.Info(new UnityStackTraceException(stacktrace), message);
                        }

                        break;
                    }
            }
        }
    }
}

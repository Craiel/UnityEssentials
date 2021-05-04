namespace Craiel.UnityEssentialsUI.Runtime
{
    using NLog;

    public static class EssentialCoreUI
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static readonly NLog.Logger Logger = LogManager.GetLogger("CRAIEL_ESSENTIALS_UI");

        public const string ComponentMenuFolder = "UI/Craiel";

        public const string DefaultAnimatorControllerName = "New Animator Controller";
        
        public const int ModalBoxOrder = 99996;
        public const int SelectFieldBlockerOrder = 99997;
        public const int SelectFieldOrder = 99998;
        public const int TooltipOrder = 99999;
    }
}
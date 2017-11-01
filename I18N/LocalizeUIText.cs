namespace Assets.Scripts.Craiel.Essentials.I18N
{
    using NLog;
    using UnityEngine;
    using UnityEngine.UI;

    public class LocalizeUIText : MonoBehaviour
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Awake()
        {
            Text target = this.GetComponent<Text>();
            if (target == null)
            {
                Logger.Warn("LocalizeUIText without valid target on {0}", this.gameObject.name);
                return;
            }

            target.text = Localization.Get(target.text);
        }
    }
}

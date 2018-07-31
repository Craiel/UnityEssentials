namespace Craiel.UnityEssentials.Runtime.I18N
{
    using UnityEngine;
    using UnityEngine.UI;

    public class LocalizeUIText : MonoBehaviour
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Awake()
        {
            Text target = this.GetComponent<Text>();
            if (target == null)
            {
                EssentialsCore.Logger.Warn("LocalizeUIText without valid target on {0}", this.gameObject.name);
                return;
            }

            target.text = Localization.Get(target.text);
        }
    }
}

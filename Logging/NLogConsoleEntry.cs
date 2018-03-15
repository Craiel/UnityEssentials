namespace Craiel.UnityEssentials.Logging
{
    using NLog;
    using UnityEngine;
    using UnityEngine.UI;

    public class NLogConsoleEntry : MonoBehaviour
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public Text Text;

        [SerializeField]
        public Color WarningColor = Color.yellow;

        [SerializeField]
        public Color ErrorColor = Color.red;

        [SerializeField]
        public Color DefaultColor = Color.white;

        public void Set(NLogInterceptorEvent eventData)
        {
            this.Text.text = eventData.Message;

            if (eventData.Level == LogLevel.Warn)
            {
                this.Text.color = this.WarningColor;
            } else if (eventData.Level == LogLevel.Error)
            {
                this.Text.color = this.ErrorColor;
            }
            else
            {
                this.Text.color = this.DefaultColor;
            }
        }
    }
}

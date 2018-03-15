namespace Craiel.UnityEssentials.Logging
{
    using System.Collections.Generic;
    using UnityEngine;

    public class NLogConsole : MonoBehaviour
    {
        private readonly IList<NLogConsoleEntry> entries;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public NLogConsole()
        {
            this.entries = new List<NLogConsoleEntry>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public NLogConsoleEntry LineTemplate;

        [SerializeField]
        public GameObject LineRoot;

        [SerializeField]
        public GameObject Content;

        [SerializeField]
        public GameObject ShowButton;

        [SerializeField]
        public int MaxLines = 30;

        public void Awake()
        {
            NLogInterceptor.Instance.OnLogChanged += this.OnLogChanged;
        }

        public void OnDestroy()
        {
            NLogInterceptor.Instance.OnLogChanged -= this.OnLogChanged;
        }

        public void Close()
        {
            this.Content.gameObject.SetActive(false);
            this.ShowButton.gameObject.SetActive(true);
        }

        public void Show()
        {
            this.Content.gameObject.SetActive(true);
            this.ShowButton.gameObject.SetActive(false);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnLogChanged(NLogInterceptorEvent eventData)
        {
            var instance = Instantiate(this.LineTemplate);
            instance.Set(eventData);

            instance.transform.SetParent(this.LineRoot.transform);

            instance.gameObject.SetActive(true);

            this.entries.Add(instance);

            while (this.entries.Count > this.MaxLines)
            {
                Destroy(this.entries[0].gameObject);
                this.entries.RemoveAt(0);
            }
        }
    }
}

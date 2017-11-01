namespace Assets.Scripts.Craiel.Essentials.Formatting
{
    using System;

    public class FormatHandler
    {
        private string cachedValue;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FormatHandler()
        {
            this.AllowCaching = false;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool AllowCaching { get; set; }
        public string DefaultParameter { get; set; }
        public Func<string, string> HandlerFunction { get; set; }

        public void ClearCache()
        {
            this.cachedValue = null;
        }

        public string Evaluate(string parameter)
        {
            if (this.cachedValue != null && this.AllowCaching)
            {
                return this.cachedValue;
            }

            if (this.HandlerFunction != null)
            {
                this.cachedValue = this.HandlerFunction.Invoke(parameter ?? this.DefaultParameter);
            }

            return this.cachedValue;
        }
    }
}

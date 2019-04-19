namespace Craiel.UnityEssentials.Runtime.Grammar.Parsers
{
    using System.Collections.Generic;

    public class CommandLineSwitch
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Switch { get; set; }

        public IList<string> Arguments { get; set; }
        
        public override int GetHashCode()
        {
            return this.Switch.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var typed = obj as CommandLineSwitch;
            if (typed == null)
            {
                return false;
            }

            return this.Switch.Equals(typed.Switch);
        }
    }
}
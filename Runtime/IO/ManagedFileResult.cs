namespace Craiel.UnityEssentials.Runtime.IO
{
    public class ManagedFileResult
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ManagedDirectory Root { get; set; }

        public ManagedFile Relative { get; set; }
        public ManagedFile Absolute { get; set; }

        public override int GetHashCode()
        {
            return this.Absolute.GetHashCode();
        }

        public override string ToString()
        {
            return this.Absolute.ToString();
        }

        public override bool Equals(object obj)
        {
            var typed = obj as ManagedFileResult;
            if (typed == null)
            {
                return false;
            }

            return this.Absolute.Equals(typed.Absolute);
        }
    }
}

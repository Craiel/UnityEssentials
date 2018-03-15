namespace Craiel.UnityEssentials.IO
{
    public class ManagedDirectoryResult
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ManagedDirectory Relative { get; set; }
        public ManagedDirectory Absolute { get; set; }

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
            var typed = obj as ManagedDirectoryResult;
            if (typed == null)
            {
                return false;
            }

            return this.Absolute.Equals(typed.Absolute);
        }
    }
}

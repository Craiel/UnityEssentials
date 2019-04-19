namespace Craiel.UnityEssentials.Runtime.Grammar.Tokenize
{
    using System.Diagnostics;

    using Craiel.UnityEssentials.Runtime.Grammar.Contracts;

    [DebuggerDisplay("Term = {Term}, Content = {Contents}")]
    public class Token : IToken
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public BaseTerm Term { get; set; }

        public int Line { get; set; }
        public int Position { get; set; }

        public string Contents { get; set; }

        public object Tag { get; set; }

        public static Token NewToken(BaseTerm term, int line, int position, string contents)
        {
            return new Token { Term = term, Line = line, Position = position, Contents = contents };
        }

        public override string ToString()
        {
            return this.Contents;
        }
    }
}

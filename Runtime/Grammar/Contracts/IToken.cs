namespace Craiel.UnityEssentials.Runtime.Grammar.Contracts
{
    public interface IToken
    {
        BaseTerm Term { get; set; }

        int Line { get; set; }
        int Position { get; set; }

        string Contents { get; set; }
    }
}

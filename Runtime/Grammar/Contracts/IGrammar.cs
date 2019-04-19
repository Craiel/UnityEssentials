namespace Craiel.UnityEssentials.Runtime.Grammar.Contracts
{
    using System.Collections.Generic;

    using Craiel.UnityEssentials.Runtime.Grammar.Terms;

    public interface IGrammar
    {
        TermIdentifier Identifier { get; }
        TermPunctuation Punctuation { get; }
        TermNumber Numbers { get; }

        IList<TermKey> KeyTerms { get; }
        IList<TermIdentifierKey> IdentifierKeyTerms { get; }
        IList<TermComment> CommentTerms { get; }
        IList<TermString> StringTerms { get; }
    }
}

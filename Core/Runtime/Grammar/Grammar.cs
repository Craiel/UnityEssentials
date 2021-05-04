﻿namespace Craiel.UnityEssentials.Runtime.Grammar
{
    using System;
    using System.Collections.Generic;

    using Craiel.UnityEssentials.Runtime.Grammar.Contracts;
    using Craiel.UnityEssentials.Runtime.Grammar.Terms;

    public abstract class Grammar : IGrammar
    {
        private readonly IList<TermKey> keyTerms;
        private readonly IList<TermIdentifierKey> identifierKeyTerms;
        private readonly IList<TermComment> commentTerms;
        private readonly IList<TermString> stringTerms;

        private readonly IDictionary<string, BaseTerm> termStringLookup;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected Grammar()
        {
            this.keyTerms = new List<TermKey>();
            this.identifierKeyTerms = new List<TermIdentifierKey>();
            this.commentTerms = new List<TermComment>();
            this.stringTerms = new List<TermString>();
            this.termStringLookup = new Dictionary<string, BaseTerm>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public TermIdentifier Identifier { get; protected set; }
        public TermPunctuation Punctuation { get; protected set; }
        public TermNumber Numbers { get; protected set; }

        public IList<TermKey> KeyTerms
        {
            get
            {
                return this.keyTerms;
            }
        }

        public IList<TermIdentifierKey> IdentifierKeyTerms
        {
            get
            {
                return this.identifierKeyTerms;
            }
        }

        public IList<TermComment> CommentTerms
        {
            get
            {
                return this.commentTerms;
            }
        }

        public IList<TermString> StringTerms
        {
            get
            {
                return this.stringTerms;
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected static HashSet<char> InitializeCharacterSet(IEnumerable<int[]> ranges)
        {
            var result = new HashSet<char>();
            foreach (var range in ranges)
            {
                for (int i = range[0]; i <= range[1]; ++i)
                {
                    result.Add((char)i);
                }
            }

            return result;
        }
        
        protected void AddTerm<T>(T term)
            where T : BaseTerm
        {
            switch (term.Type)
            {
                case TermType.Key:
                    {
                        this.keyTerms.Add(term as TermKey);
                        break;
                    }

                case TermType.IdentifierKey:
                    {
                        this.identifierKeyTerms.Add(term as TermIdentifierKey);
                        break;
                    }

                case TermType.Comment:
                    {
                        this.commentTerms.Add(term as TermComment);
                        break;
                    }

                case TermType.String:
                    {
                        this.stringTerms.Add(term as TermString);
                        break;
                    }

                default:
                    {
                        throw new NotImplementedException(term.Type.ToString());
                    }
            }
        }

        protected void ToTerm<T>(string text, T key)
        {
            var term = new TermKey(key.ToString(), text, key);
            this.AddTerm(term);
            this.termStringLookup.Add(text, term);
        }

        protected void ToIdentifierTerm<T>(string text, T key)
        {
            var term = new TermIdentifierKey(key.ToString(), text, key);
            this.AddTerm(term);
            this.termStringLookup.Add(text, term);
        }
    }
}

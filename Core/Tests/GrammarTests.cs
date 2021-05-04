namespace Craiel.UnityEssentials.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Runtime.Grammar.Grammars;
    using Runtime.Grammar.Parsers;
    using Runtime.Grammar.Tokenize;

    public static class GrammarTests
    {
        private const string TestCommandLine = " -v -b -mno -s \"hello world\" --param withValue -k=value";
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        
        [Test]
        public static void CommandLineTest()
        {
            var grammar = new CommandLineGrammar();
            
            var tokenizer = new Tokenizer();
            IList<Token> tokens = tokenizer.Tokenize(grammar, TestCommandLine);
            Assert.AreEqual(16, tokens.Count);

            IDictionary<string, CommandLineSwitch> switches;
            Assert.IsTrue(CommandLineTokenParser.Parse(tokens, out switches));
            
            Assert.AreEqual(6, switches.Count);
            Assert.AreEqual("hello world", switches["s"].Arguments[0]);
            Assert.AreEqual("withValue",switches["param"].Arguments[0]);
            Assert.AreEqual("value", switches["k"].Arguments[0]);
        }
    }
}
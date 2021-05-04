namespace Craiel.UnityEssentials.Runtime.Grammar.Parsers
{
    using System.Collections.Generic;
    using Grammars;
    using Tokenize;

    public static class CommandLineTokenParser
    {
        private enum TranslationMode
        {
            None,
            GotSwitch,
            ExpectSwitch,
            ExpectArgument
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static bool Parse(IList<Token> tokens, out IDictionary<string, CommandLineSwitch> switches)
        {
            int currentToken = 0;
            var mode = TranslationMode.None;
            CommandLineSwitch currentSwitch = new CommandLineSwitch();
            switches = new Dictionary<string, CommandLineSwitch>();
            
            while (currentToken < tokens.Count)
            {
                Token token = tokens[currentToken++];
                if (token.Term?.Tag == null)
                {
                    EssentialsCore.Logger.Error("Argument parsing failed for token {0}", token);
                    continue;
                }

                var key = (CommandLineTermKey) token.Term.Tag;
                switch (key)
                {
                    case CommandLineTermKey.Slash:
                    case CommandLineTermKey.Dash:
                    case CommandLineTermKey.Dash2:
                    {
                        if (mode != TranslationMode.None && mode != TranslationMode.GotSwitch)
                        {
                            EssentialsCore.Logger.Error("Unexpected switch term: {0}", token);
                            return false;
                        }

                        mode = TranslationMode.ExpectSwitch;
                        break;
                    }

                    case CommandLineTermKey.Equals:
                    case CommandLineTermKey.Colon:
                    {
                        if (mode != TranslationMode.GotSwitch)
                        {
                            EssentialsCore.Logger.Error("Unexpected argument assignment: {0}", token);
                            return false;
                        }

                        mode = TranslationMode.ExpectArgument;
                        break;
                    }

                    case CommandLineTermKey.String:
                    {
                        if (mode != TranslationMode.ExpectArgument && mode != TranslationMode.GotSwitch)
                        {
                            EssentialsCore.Logger.Error("Unexpected string: {0}", token);
                            return false;
                        }

                        UpdateSwitchArguments(currentSwitch, token.Contents);
                        mode = TranslationMode.None;
                        break;
                    }

                    case CommandLineTermKey.Identifier:
                    {
                        if (mode == TranslationMode.ExpectArgument || mode == TranslationMode.GotSwitch)
                        {
                            UpdateSwitchArguments(currentSwitch, token.Contents);
                            mode = TranslationMode.None;
                            continue;
                        }

                        if (mode == TranslationMode.ExpectSwitch)
                        {
                            currentSwitch = RegisterSwitch(token.Contents, switches);
                            mode = TranslationMode.GotSwitch;
                            continue;
                        }

                        EssentialsCore.Logger.Error("Unexpected identifier: {0}", token);
                        return false;
                    }

                    case CommandLineTermKey.Pipe:
                    {
                        // Abort parsing, this is probably no longer for us
                        return true;
                    }
                }
            }

            return true;
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void UpdateSwitchArguments(CommandLineSwitch target, string argument)
        {
            argument = argument.Trim('"');
            if (string.IsNullOrWhiteSpace(argument))
            {
                return;
            }

            if (target.Arguments == null)
            {
                target.Arguments = new List<string>();
            }

            target.Arguments.Add(argument);
        }
        
        private static CommandLineSwitch RegisterSwitch(string contents, IDictionary<string, CommandLineSwitch> target)
        {
            if (target.TryGetValue(contents, out CommandLineSwitch commandSwitch))
            {
                return commandSwitch;
            }

            commandSwitch = new CommandLineSwitch { Switch = contents };
            target.Add(contents, commandSwitch);
            return commandSwitch;
        }
    }
}
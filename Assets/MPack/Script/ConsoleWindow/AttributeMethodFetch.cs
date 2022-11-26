using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;


namespace MPack
{
    public class AttributeMethodFetch
    {
        private CommandMethod[] m_commands;

        public CommandMethod[] commandSuggestionsCache;
        
        public void ReindexCommandsInAllAssemblies()
        {
            List<CommandMethod> commands = new List<CommandMethod>();

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in a.GetTypes())
                {
                    commands.AddRange(FindMethodWithAttrInType(t));
                }
            }

            m_commands = commands.ToArray();
        }

        private List<CommandMethod> FindMethodWithAttrInType(Type type)
        {
            List<CommandMethod> commands = new List<CommandMethod>();

            MethodInfo[] infos = type.GetMethods(
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var method in infos)
            {
                ConsoleCommandAttribute commandAttr = (ConsoleCommandAttribute)method.GetCustomAttribute(typeof(ConsoleCommandAttribute), true);
                if (commandAttr != null)
                {
                    CommandMethod commandMethod = new CommandMethod(type, method, commandAttr);
                    AbstractCommandPart[] commandParts;

                    if (ParseCommandPart(commandAttr.command, out commandParts))
                    {
                        commandMethod.commandParts = commandParts;
                        commands.Add(commandMethod);
                    }
                    else
                        Debug.LogWarningFormat("Command parsing failed: '{0}'", commandAttr.command);
                }
            }

            return commands;
        }

        private bool ParseCommandPart(string command, out AbstractCommandPart[] commandParts)
        {
            string[] stringParts = command.Split();
            commandParts = new AbstractCommandPart[stringParts.Length];

            for (int i = 0; i < stringParts.Length; i++)
            {
                if (!stringParts[i].StartsWith(":"))
                    commandParts[i] = new CommandPureText(stringParts[i]);
                else
                {
                    switch (stringParts[i].Substring(1).ToLower())
                    {
                        case "int":
                        case "integer":
                            commandParts[i] = new IntgerCommandArgument();
                            break;
                        case "str":
                        case "string":
                            commandParts[i] = new StringCommandArgument();
                            break;
                        case "float":
                            commandParts[i] = new FloatCommandArgument();
                            break;
                        default:
                            return false;
                    }
                }
            }

            return true;
        }

        private string[] SplitCommandInput(string command)
        {
            List<string> commands = new List<string>();
            int searchedIndex = 0;
            // int count = 0;

            while (true)
            {
                int spaceIndex = command.IndexOf(' ', searchedIndex);
                int quoteIndex = command.IndexOf('\'', searchedIndex);

                if (spaceIndex == -1) spaceIndex = command.Length;
                if (quoteIndex == -1) quoteIndex = command.Length;

                if (spaceIndex <= quoteIndex)
                {
                    string part = command.Substring(searchedIndex, spaceIndex - searchedIndex);
                    if (!string.IsNullOrEmpty(part))
                        commands.Add(part);
                    searchedIndex = spaceIndex + 1;
                }
                else
                {
                    quoteIndex += 1;

                    int secondQuote = command.IndexOf('\'', quoteIndex);
                    if (secondQuote == -1) secondQuote = command.Length;

                    commands.Add(command.Substring(quoteIndex, secondQuote - quoteIndex));

                    searchedIndex = secondQuote + 1;
                }

                if (searchedIndex >= command.Length - 1 || searchedIndex == -1)
                    break;
            }

            return commands.ToArray();
        }



        public bool FindCommandMatch(string command, out CommandMethod commandMethod, out object[] arguments)
        {
            string[] commandPrefixes = SplitCommandInput(command);

            for (int i = 0; i < m_commands.Length; i++)
            {
                if (m_commands[i].IsFullyMatched(commandPrefixes, out arguments))
                {
                    commandMethod = m_commands[i];
                    return true;
                }
            }

            commandMethod = null;
            arguments = null;
            return false;
        }

        public CommandMethod[] GetCommandSuggestions(int length)
        {
            CommandMethod[] commands = new CommandMethod[length];
            int index = 0;

            for (int i = 0; i < m_commands.Length; i++)
            {
                if (!m_commands[i].commandAttribute.hideFromSuggestion)
                {
                    commands[index++] = m_commands[i];
                    if (index >= length) break;
                }
            }

            commandSuggestionsCache = commands;
            return commands;
        }

        public CommandMethod[] GetCommandSuggestions(string commandPrefix, int length)
        {
            CommandMethod[] commands = new CommandMethod[length];
            string[] commandPrefixes = SplitCommandInput(commandPrefix);
            int index = 0;

            for (int i = 0; i < m_commands.Length; i++)
            {
                if (!m_commands[i].commandAttribute.hideFromSuggestion)
                {
                    if (m_commands[i].IsMatched(commandPrefixes))
                    {
                        commands[index++] = m_commands[i];
                        if (index >= length) break;
                    }
                }
            }

            commandSuggestionsCache = commands;
            return commands;
        }
    }
}
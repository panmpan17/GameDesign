using System;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace MPack
{
    public class CommandMethod
    {
        public Type type;
        public MethodInfo method;
        public ConsoleCommandAttribute commandAttribute;
        public AbstractCommandPart[] commandParts;

        public CommandMethod(Type type, MethodInfo method, ConsoleCommandAttribute commandAttribute)
        {
            this.type = type;
            this.method = method;
            this.commandAttribute = commandAttribute;
            this.commandParts = null;
        }

        public string FormmatedSuggestion()
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < commandParts.Length; i++)
            {
                stringBuilder.Append(commandParts[i].FormattedString());
                if (i < commandParts.Length - 1)
                    stringBuilder.Append(" ");
            }

            return stringBuilder.ToString();
        }

        public string PureTextSuggestion()
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < commandParts.Length; i++)
            {
                stringBuilder.Append(commandParts[i].PureString());
                if (i < commandParts.Length - 1)
                    stringBuilder.Append(" ");
            }

            return stringBuilder.ToString();
        }

        public override string ToString()
        {
            return method.Name;
        }

        public bool IsMatched(string[] commandPrefixes)
        {
            if (commandPrefixes.Length > commandParts.Length) return false;

            for (int i = 0; i < commandParts.Length && i < commandPrefixes.Length; i++)
            {
                if (!commandParts[i].IsMatched(commandPrefixes[i], i == commandPrefixes.Length - 1))
                    return false;
            }
            return true;
        }

        public bool IsFullyMatched(string[] commandPrefixes, out object[] arguments)
        {
            if (commandPrefixes.Length != commandParts.Length)
            {
                arguments = null;
                return false;
            }

            List<object> argumentsList = new List<object>();
            for (int i = 0; i < commandParts.Length; i++)
            {
                object argument;
                if (!commandParts[i].IsFullyMatched(commandPrefixes[i], i == commandPrefixes.Length - 1, out argument))
                {
                    arguments = null;
                    return false;
                }
                else if (argument != null)
                {
                    argumentsList.Add(argument);
                }
            }

            arguments = argumentsList.ToArray();
            return true;
        }
    }


    public abstract class AbstractCommandPart
    {
        public abstract string FormattedString();
        public abstract string PureString();
        public abstract bool IsMatched(string prefix, bool isLastPart);
        public abstract bool IsFullyMatched(string prefix, bool isLastPart, out object argument);
    }

    public class CommandPureText: AbstractCommandPart
    {
        public string value;

        public CommandPureText(string value)
        {
            this.value = value;
        }

        public override string FormattedString()
        {
            return value;
        }

        public override string PureString()
        {
            return value;
        }

        public override bool IsMatched(string prefix, bool isLastPart)
        {
            if (isLastPart) return value.StartsWith(prefix);
            return value == prefix;
        }

        public override bool IsFullyMatched(string prefix, bool isLastPart, out object argument)
        {
            argument = null;
            if (isLastPart) return value.StartsWith(prefix);
            return value == prefix;
        }
    }


    public class StringCommandArgument : AbstractCommandPart
    {
        public override string FormattedString()
        {
            return "<color=#0df>:string</color>";
        }

        public override string PureString()
        {
            return "string";
        }

        public override bool IsMatched(string prefix, bool isLastPart)
        {
            return true;
        }

        public override bool IsFullyMatched(string prefix, bool isLastPart, out object argument)
        {
            argument = (object)prefix;
            return true;
        }
    }


    public class IntgerCommandArgument : AbstractCommandPart
    {
        public override string FormattedString()
        {
            return "<color=#0df>:int</color>";
        }

        public override string PureString()
        {
            return ":int";
        }

        public override bool IsMatched(string prefix, bool isLastPart)
        {
            if (":int".StartsWith(prefix)) return true;

            Char[] chars = prefix.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                switch (chars[i])
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        break;
                    default:
                        return false;
                }
            }

            return true;
        }

        public override bool IsFullyMatched(string prefix, bool isLastPart, out object argument)
        {
            int value;
            if (int.TryParse(prefix, out value))
            {
                argument = (object)value;
                return true;
            }
            else
            {
                argument = null;
                return false;
            }
        }
    }


    public class FloatCommandArgument : AbstractCommandPart
    {
        public override string FormattedString()
        {
            return "<color=#0df>:float</color>";
        }

        public override string PureString()
        {
            return ":float";
        }

        public override bool IsMatched(string prefix, bool isLastPart)
        {
            if (":float".StartsWith(prefix)) return true;

            Char[] chars = prefix.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                switch (chars[i])
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '.':
                        break;
                    default:
                        return false;
                }
            }

            return true;
        }

        public override bool IsFullyMatched(string prefix, bool isLastPart, out object argument)
        {
            float value;
            if (float.TryParse(prefix, out value))
            {
                argument = (object)value;
                return true;
            }
            else
            {
                argument = null;
                return false;
            }
        }
    }
}
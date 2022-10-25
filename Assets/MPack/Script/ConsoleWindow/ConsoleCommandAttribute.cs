using System;


namespace MPack
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ConsoleCommandAttribute : Attribute
    {
        public string command;
        public bool hideFromSuggestion;

        public ConsoleCommandAttribute(string command, bool hideFromSuggestion=false)
        {
            this.command = command;
            this.hideFromSuggestion = hideFromSuggestion;
        }
    }
}
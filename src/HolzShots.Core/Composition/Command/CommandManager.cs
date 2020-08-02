using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using HolzShots.Input;

namespace HolzShots.Composition.Command
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CommandAttribute : Attribute
    {
        public string Name { get; }
        public CommandAttribute(string name) => Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public class CommandManager
    {
        private Dictionary<string, ICommand> Actions { get; } = new Dictionary<string, ICommand>();

        public void RegisterCommand(ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var name = GetCommandNameForType(command.GetType());
            Debug.Assert(name != null);
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name of command is null or white space");

            name = name.ToLowerInvariant();

            if (Actions.ContainsKey(name))
            {
                Debug.Assert(false);
                return;
            }
            Actions[name] = command;
        }

        public IHotkeyAction GetHotkeyActionFromKeyBinding(KeyBinding binding)
        {
            // We assume that everything is already checked (validation step should have validated that all hotkeys are != null
            // We also assume that every key is only assigned once

            Debug.Assert(binding != null);
            Debug.Assert(binding.Keys != null);
            Debug.Assert(binding.Command != null);

            return new HotkeyCommand(this, binding);
        }

        private ICommand GetCommand(string name)
        {
            Debug.Assert(IsRegisteredCommand(name));
            return Actions.TryGetValue(name.ToLowerInvariant(), out var command)
                ? command
                : null;
        }

        private string GetCommandNameForType(Type t) => t.GetCustomAttribute<CommandAttribute>(false)?.Name;
        private string GetCommandNameForType<T>() where T : ICommand => GetCommandNameForType(typeof(T));

        public bool IsRegisteredCommand(string name) => !string.IsNullOrWhiteSpace(name) && Actions.ContainsKey(name.ToLowerInvariant());

        public Task Dispatch<T>() where T : ICommand
        {
            var name = GetCommandNameForType<T>();
            return Dispatch(name);
        }

        public Task Dispatch(string name)
        {
            Debug.Assert(IsRegisteredCommand(name));

            var cmd = GetCommand(name);
            Debug.Assert(cmd != null);

            return cmd == null
                ? Task.CompletedTask
                : cmd.Invoke();
        }
    }
}

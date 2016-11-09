using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public delegate void CommandHandler(string[] args);

public class ConsoleController {

    public static ConsoleController Instance;

	public bool IsVisble = false;

    public delegate void LogChangedHandler(string[] log);
    public event LogChangedHandler logChanged;

    public delegate void VisibilityChangedHandler(bool visible);
    public event VisibilityChangedHandler visibilityChanged;

    class CommandRegistration {

        public string Command { get; protected set; }
        public string Args { get; protected set; }
        public CommandHandler Handler { get; protected set; }
        public string Help { get; protected set; }

        public CommandRegistration(string command, string args, CommandHandler handler, string help) {
            this.Command = command;
            this.Args = args;
            this.Handler = handler;
            this.Help = help;
        }

    }

    //How many log lines should be retained?
    //Note that strings submitted to AppendLogLine with embeded newlines will be counted as a single line
    private const int scrollbackSize = 20;

    Queue<string> scrollback = new Queue<string>(scrollbackSize);
    List<string> commandHistory = new List<string>();
    Dictionary<string, CommandRegistration> Commands = new Dictionary<string, CommandRegistration>();

    Dictionary<string, string> Variables = new Dictionary<string, string>();

    public string[] Log { get; private set; }

    public ConsoleController() {
        Instance = this;
	    visibilityChanged += visibilityChanged;

        //Declare Commands
        RegisterCommand("echo", "<optional>string | var", echo, "Prints the passed argument or Variable");
        RegisterCommand("help", "<optional>cmd", help, "Prints all commands(and arguments) to the Console Log if no argument is given. If a command is passed as a argument, its prints that commands help");
        RegisterCommand("set", "name, val", set, "Set a Global Variable");
    }

    private void RegisterCommand(string command, string args,  CommandHandler handler, string help) {
        Commands.Add(command, new CommandRegistration(command, args, handler, help));
    }

    public void RegisterSystemVariable(string name, object defaultVal) {
        if(Variables.ContainsKey(name)) {
            Debug.LogError("The Variable '" + name + "' has already been registered!");
            return;
        }

        Variables.Add(name, defaultVal.ToString());
    }

    public string GetSystemVariable(string name) {
        if(!Variables.ContainsKey(name)) {
            Debug.LogError("The Variable '" + name + "' does not exist!");
            return null;
        }

        return Variables[name];
    }

    public void AppendLogLine(string line) {
        if(scrollback.Count >= ConsoleController.scrollbackSize) {
            scrollback.Dequeue();
        }
        scrollback.Enqueue(line);

        Log = scrollback.ToArray();
        if(logChanged != null)
            logChanged(Log);
    }

    public void RunCommandString(string commandString) {
        AppendLogLine("> " + commandString);

	    int firstSpaceIndex = commandString.IndexOf(" ", StringComparison.Ordinal);

        string commandHandler = firstSpaceIndex >= 0 ? commandString.Substring(0, firstSpaceIndex) : commandString;
        string[] args = PullArgsFromCmd(commandString.Substring(firstSpaceIndex >= 0 ? firstSpaceIndex + 1 : commandString.Length));

        if(!Commands.ContainsKey(commandHandler)) {
            AppendLogLine("Invalid Command: " + commandHandler);
            return;
        }

        CommandRegistration cmd = Commands[commandHandler];
        cmd.Handler(args);
    }

    private string[] PullArgsFromCmd(string argsString) {
        List<string> args = Regex
                            .Matches(argsString, @"(?<match>\w+)|\""(?<match>[\w\s]*)""")
                            .Cast<Match>()
                            .Select(m => m.Groups["match"].Value)
                            .ToList();


        return args.ToArray();
    }

    #region Command Handlers

    private void echo(string[] args) {
        // prints argument or variable with the argument as a name
        if(args.Length > 1) {
            AppendLogLine("Error: Expected 1 argument");
            return;
        }

        if(Variables.ContainsKey(args[0])) {
            AppendLogLine(Variables[args[0]].ToString());
        } else {
            AppendLogLine(args[0]);
        }
    }

    private void help(string[] args) {
        // prints all registered commands or help for a single command if specified
        if(args.Length > 1) {
            AppendLogLine("Error: Expected 1 argument");
            return;
        }

        if(args.Length == 1) {
            if(Commands.ContainsKey(args[0])) {
                CommandRegistration cmd = Commands[args[0]];
                AppendLogLine(cmd.Command + " [" + cmd.Args + "] | " + cmd.Help);
            } else {
                AppendLogLine("Error: Command is non-existent");
            }
        } else {
            foreach(CommandRegistration cmd in Commands.Values) {
                AppendLogLine(cmd.Command + " [" + cmd.Args + "] | " + cmd.Help);
            }
        }

    }

    private void set(string[] args) {
		// set a system-wide variable
		if(args.Length > 2) {
			AppendLogLine("Error: Expected 2 arguments");
			return;
		}

		if(Variables.ContainsKey(args[0])) {
			Variables[args[0]] = args[1];
			AppendLogLine("Variable Set: " + args[0] + " = " + args[1]);
		} else {
			Variables.Add(args[0], args[1]);
			AppendLogLine("Variable Stored: " + args[0] + " = " + args[1]);
		}
	}

	#endregion

}

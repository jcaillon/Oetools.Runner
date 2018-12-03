﻿using System;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Oetools.Sakoe.Command.Oe;
using Oetools.Sakoe.Utilities;

namespace Oetools.Sakoe.Command {
    
    /// <summary>
    /// The main command of the application, called when the user passes no arguments/commands
    /// </summary>
    [Command(
        FullName = "SAKOE - a Swiss Army Knife for OpenEdge",
        Description = "SAKOE is a collection of tools aimed to simplify your work in Openedge environments.",
        ExtendedHelpText = @"GETTING STARTED 
  The '" + ManCommand.Name + @"' command is a good place to start using this tool.
  This tool has a lot of utility commands but the bread and butter command is '" + BuildCommand.Name + @"'. It allows you to do Openedge build automation.

NOTES
  - You can escape white spaces in argument and option values by using double quotes (i.e. "")
  - In the 'USAGE' help, arguments between brackets (i.e. []) are optionals

GET RAW OUTPUT
  If you want a raw output for each commands (without display the log lines), you can set the verbosity to ""None"" and use the no progress bars option.
    sakoe [command] -vb None -po

EXIT CODE
  The convention followed by this tool is the following.
    0 : used when a command completed successfully, without errors nor warnings.
    1-8 : used when a command completed but with warnings, the level can be used to pinpoint different kind of warnings.
    9 : used when a command does not complete and ends up in error.

RESPONSE FILE PARSING
  Instead of using a long command line, you can use a response file that contains each argument/option that should be used.
  Everything that is usually separated by a space in the command line should be separated by a new line in the file.
  In response files, you do not have to double quote arguments containing spaces, they will be considered as a whole as long as they are on a separated line.
    sakoe @responsefile.txt

WEBSITE 
  https://jcaillon.github.io/Oetools.Sakoe/
",
        OptionsComparison = StringComparison.CurrentCultureIgnoreCase,
        ResponseFileHandling = ResponseFileHandling.ParseArgsAsLineSeparated
    )]
    [HelpOption("-?|-h|" + HelpLongName, Description = "Show help information.", Inherited = true)]
#if DEBUG
    [Subcommand(typeof(SelfTestCommand))]
#endif
    [Subcommand(typeof(ManCommand))]
    [Subcommand(typeof(DatabaseCommand))]
    [Subcommand(typeof(LintCommand))]
    [Subcommand(typeof(ProjectCommand))]
    [Subcommand(typeof(BuildCommand))]
    [Subcommand(typeof(ShowVersionCommand))]
    [Subcommand(typeof(XcodeCommand))]
    [Subcommand(typeof(HashCommand))]
    [Subcommand(typeof(ProHelpCommand))]
    [Subcommand(typeof(UtilitiesCommand))]
    [Subcommand(typeof(ProlibCommand))]
    internal class MainCommand : BaseCommand {

        public const string HelpLongName = "--help";
        
        public static int ExecuteMainCommand(string[] args) {
            try {
                using (var app = new CommandLineApplicationCustomHint<MainCommand>(HelpGenerator.Singleton, PhysicalConsole.Singleton, Directory.GetCurrentDirectory(), true)) {
                    app.Conventions.UseDefaultConventions();
                    app.ParserSettings.MakeSuggestionsInErrorMessage = true;
                    return app.Execute(args);
                }
            } catch (Exception ex) {
                var log = ConsoleIo.Singleton;
                log.LogLevel = ConsoleIo.LogLvl.Debug;
                
                if (ex is CommandParsingException) {
                    var bb = ex.GetType()?.GetProperty("NearestMatch")?.GetValue(ex);
                    
                    //if (ex is UnrecognizedCommandParsingException unrecognizedCommandParsingException) {
                    //    log.Info($"Did you mean {unrecognizedCommandParsingException.NearestMatch}?");
                    //}
                    log.Error(ex.Message);
                    log.Info($"Specify {HelpLongName} for a list of available options and commands. {bb}");
                } else {
                    log.Error(ex.Message, ex);
                }

                log.Fatal($"Exit code {FatalExitCode}");
                log.WriteOnNewLine(null);
                return FatalExitCode;
            } finally {
                ConsoleIo.Singleton.Dispose();
            }
        }
    }
}
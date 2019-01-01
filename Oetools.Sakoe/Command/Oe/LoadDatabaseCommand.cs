﻿using McMaster.Extensions.CommandLineUtils;
using Oetools.Utilities.Openedge.Database;

namespace Oetools.Sakoe.Command.Oe {
    [Command(
        "load", "lo",
        Description = "Load data or schema definition to a database."
    )]
    [Subcommand(typeof(LoadSeqDatabaseCommand))]
    [Subcommand(typeof(LoadDataDatabaseCommand))]
    [Subcommand(typeof(LoadDfDatabaseCommand))]
    internal class LoadDatabaseCommand : AExpectSubCommand {
    }

    [Command(
        "seq", "se",
        Description = "Load the database sequence values from a plain text file (.d)."
    )]
    internal class LoadSeqDatabaseCommand : ADatabaseCommand {

        [Option("-d|--data-file", "File path that contains the sequence data to load.", CommandOptionType.SingleValue)]
        public string LoadFilePath { get; set; }

        protected override int ExecuteCommand(CommandLineApplication app, IConsole console) {
            SetTargetDatabasePath();
            using (var dbAdministrator = new UoeDatabaseAdministrator(GetDlcPath())) {
                dbAdministrator.Log = Log;
                dbAdministrator.LoadSequenceData(TargetDatabasePath, LoadFilePath);
            }
            return 0;
        }
    }

    [Command(
        "data", "da",
        Description = "Load the database data from plain text files (.d)."
    )]
    internal class LoadDataDatabaseCommand : ADatabaseCommand {

        [Option("-d|--data-directory", "Directory path that contain the data to load. Each table of the database should be stored as an individual .d file named like the table.", CommandOptionType.SingleValue)]
        public string LoadFilePath { get; set; }

        [Option("-t|--table", "TA list of comma separated table names to load. Defaults to `ALL` which loads every table.", CommandOptionType.SingleValue)]
        public string TableName { get; set; } = "ALL";

        protected override int ExecuteCommand(CommandLineApplication app, IConsole console) {
            SetTargetDatabasePath();
            using (var dbAdministrator = new UoeDatabaseAdministrator(GetDlcPath())) {
                dbAdministrator.Log = Log;
                dbAdministrator.LoadData(TargetDatabasePath, LoadFilePath, TableName);
            }
            return 0;
        }
    }

    [Command(
        "schema", "df",
        Description = "Load the schema definition (.df) to a database."
    )]
    internal class LoadDfDatabaseCommand : ADatabaseCommand {

        [Option("-df|--df", "Path to the .df file that contains the schema definition (or partial schema) of the database.", CommandOptionType.SingleValue)]
        public string LoadFilePath { get; set; }

        protected override int ExecuteCommand(CommandLineApplication app, IConsole console) {
            SetTargetDatabasePath();
            using (var dbAdministrator = new UoeDatabaseAdministrator(GetDlcPath())) {
                dbAdministrator.Log = Log;
                dbAdministrator.LoadSchemaDefinition(TargetDatabasePath, LoadFilePath);
            }
            return 0;
        }
    }

}

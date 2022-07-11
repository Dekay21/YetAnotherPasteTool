using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using System.IO;
using Microsoft.Xna.Framework;

namespace YetAnotherPasteTool
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        public override string Author => "Dekay";
        public override string Description => "Tool for creating and pasting schematics";
        public override string Name => "Test Plugin";
        public override Version Version => new Version(1, 0);
        public readonly string SCHEMATICS_PATH = "YetAnotherPasteTool";

        public Plugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
        }

        private void OnInitialize(EventArgs args)
        {
            if (!Directory.Exists(Path.Combine(TShock.SavePath, SCHEMATICS_PATH)))
            {
                Directory.CreateDirectory(Path.Combine(TShock.SavePath, SCHEMATICS_PATH));
            }

            Commands.ChatCommands.Add(new Command("", SchematicCommand, "schematic", "sc"));
        }

        private void SchematicCommand(CommandArgs args)
        {
            if (args.Parameters.Count != 2)
            {
                args.Player.SendErrorMessage("Invalid syntax!");
                args.Player.SendErrorMessage("Use /sc <save/paste/delete> name");
            }

            string subcommand = args.Parameters[0].ToLower();
            string name = args.Parameters[1].ToLower();

            switch (subcommand)
            {
                case "save":
                    {
                        // Creates a new schematic
                        if (!args.Player.TempPoints.All(x => x.X + x.Y != 0))
                        {
                            args.Player.SendWarningMessage("You need to set a region to Save as schematic (/region set <1/2>)");
                            return;
                        }

                        Point p1 = args.Player.TempPoints[0];
                        Point p2 = args.Player.TempPoints[1];
                        Schematic sc = new Schematic(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Abs(p1.X - p2.X) + 1, Math.Abs(p1.Y - p2.Y) + 1);
                        string filePath = Path.Combine(TShock.SavePath, SCHEMATICS_PATH, name);
                        bool result = sc.Save(filePath);
                        if (result)
                        {
                        args.Player.SendInfoMessage("Saved selected region as Schematic '" + name + "'");
                        } else
                        {
                            args.Player.SendErrorMessage("Error while saving schematic '" + name + "'");
                        }
                        break;
                    }
                case "paste":
                    {
                        // Pastes an existing schematic
                        Point p1 = args.Player.TempPoints[0];
                        if (!(p1.X + p1.Y != 0))
                        {
                            args.Player.SendWarningMessage("You need to set a upper left corner to paste the schematic (/region set 1)");
                            return;
                        }


                        Schematic sc = Schematic.Load(Path.Combine(TShock.SavePath, SCHEMATICS_PATH, name));
                        sc.Paste(p1.X, p1.Y);
                        args.Player.SendInfoMessage("Schematic '" + name + "' successfully pasted");
                        TSPlayer.All.SendTileRect((short)p1.X, (short)p1.Y, (byte)sc.Width, (byte)sc.Height);
                        break;
                    }
                case "delete":
                    {
                        // Deletes an existing schematic
                        string filePath = Path.Combine(TShock.SavePath, SCHEMATICS_PATH, name + Schematic.SCHEMATIC_FILE_ENDING);
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                            args.Player.SendInfoMessage("Schematic '" + name + "' successfully deleted");
                        }
                        else
                        {
                            args.Player.SendErrorMessage("No schematic with that name exists");
                        }
                        break;
                    }
                default:
                    args.Player.SendErrorMessage("Unknown subcommand specified: " + subcommand + "\nAvailable subcommands are: save, paste, delete");
                    break;
            }
        }
    }
}

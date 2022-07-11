using System;
using Terraria;
using System.IO;
using OTAPI.Tile;
using System.Runtime.Serialization.Formatters.Binary;

namespace YetAnotherPasteTool
{
    [Serializable]
    public class Schematic
    {
        public readonly static string SCHEMATIC_FILE_ENDING = ".sc";

        private readonly TileType[,] tileTypes;

        public int Width
        {
            get
            {
                return tileTypes.GetLength(0);
            }
        }

        public int Height
        {
            get
            {
                return tileTypes.GetLength(1);
            }
        }

        public Schematic(int x, int y, int width, int height)
        {
            tileTypes = new TileType[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    TileType copy = new TileType();
                    ITile tile = Main.tile[x + i, y + j];
                    copy.Active = tile.active();
                    copy.InActive = tile.inActive();
                    copy.Type = tile.type;
                    copy.Color = tile.color();
                    copy.Slope = tile.slope();
                    copy.Wall = tile.wall;
                    copy.WallColor = tile.wallColor();
                    copy.FrameX = tile.frameX;
                    copy.FrameY = tile.frameY;
                    copy.Liquid = tile.liquid;
                    copy.LiquidType = tile.liquidType();
                    copy.Actuator = tile.actuator();
                    copy.Wire1 = tile.wire();
                    copy.Wire2 = tile.wire2();
                    copy.Wire3 = tile.wire3();
                    copy.Wire4 = tile.wire4();
                    tileTypes[i, j] = copy;
                }
            }
        }

        public void Paste(int x, int y)
        {
            for (int i = 0; i < tileTypes.GetLength(0); i++)
            {
                for (int j = 0; j < tileTypes.GetLength(1); j++)
                {
                    TileType scTile = tileTypes[i, j];
                    Main.tile[x + i, y + j].active(scTile.Active);
                    Main.tile[x + i, y + j].inActive(scTile.InActive);
                    Main.tile[x + i, y + j].type = scTile.Type;
                    Main.tile[x + i, y + j].color(scTile.Color);
                    Main.tile[x + i, y + j].slope(scTile.Slope);
                    Main.tile[x + i, y + j].wall = scTile.Wall;
                    Main.tile[x + i, y + j].wallColor(scTile.WallColor);
                    Main.tile[x + i, y + j].frameX = scTile.FrameX;
                    Main.tile[x + i, y + j].frameY = scTile.FrameY;
                    Main.tile[x + i, y + j].liquid = scTile.Liquid;
                    Main.tile[x + i, y + j].liquidType(scTile.LiquidType);
                    Main.tile[x + i, y + j].actuator(scTile.Actuator);
                    Main.tile[x + i, y + j].wire(scTile.Wire1);
                    Main.tile[x + i, y + j].wire2(scTile.Wire2);
                    Main.tile[x + i, y + j].wire3(scTile.Wire3);
                    Main.tile[x + i, y + j].wire4(scTile.Wire4);
                }
            }
        }

        public bool Save(string path)
        {
            string filename = path + SCHEMATIC_FILE_ENDING;
            if (File.Exists(filename))
            {
                return false;
            }

            FileStream stream = File.Create(filename);
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            stream.Close();

            return true;
        }

        public static Schematic Load(string path)
        {
            string filename = path + SCHEMATIC_FILE_ENDING;
            if (!File.Exists(filename))
            {
                return null;
            }

            FileStream stream = File.OpenRead(filename);
            var formatter = new BinaryFormatter();
            var v = (Schematic)formatter.Deserialize(stream);
            stream.Close();

            return v;
        }
    }

    [Serializable]
    internal struct TileType
    {
        internal bool Active { get; set; }
        internal bool InActive { get; set; }
        internal byte Liquid { get; set; }
        internal bool Honey { get; set; }
        internal bool Lava { get; set; }
        internal bool Actuator { get; set; }
        internal byte LiquidType { get; set; }
        internal byte Slope { get; set; }
        internal byte Color { get; set; }
        internal ushort Type { get; set; }
        internal bool Wire1 { get; set; }
        internal bool Wire2 { get; set; }
        internal bool Wire3 { get; set; }
        internal bool Wire4 { get; set; }
        internal ushort Wall { get; set; }
        internal byte WallColor { get; set; }
        internal short FrameX { get; set; }
        internal short FrameY { get; set; }
    }
}

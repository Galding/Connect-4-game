using System;
using System.Linq;

namespace puissance4.DesktopClient
{
    internal class Board
    {
        private byte[,] map;

        public Board()
        {
            map = new byte[Game1.VX, Game1.VY]{ // 0 = empty (white), 1 = yellow, 2 = red
             {0, 0, 0, 0, 0, 0, 0},
             {0, 0, 0, 0, 0, 0, 0},
             {0, 0, 0, 0, 0, 0, 0},
             {0, 0, 0, 0, 0, 0, 0},
             {0, 0, 0, 0, 0, 0, 0},
             {0, 0, 0, 0, 0, 0, 0}
             };
        }

        public int getValue(int x, int y)
        {
            return map[x, y];
        }


        public void setValueAt(int x, int y, byte value)
        {
            map[x, y] = value;
        }

        public bool isMapFull()
        {
            for (int y = 0; y < Game1.VY; y++)
            {
                for (int x = 0; x < Game1.VX; x++)
                {
                    if (map[x, y] == 0) return false;
                }
            }
            return true;
        }

        public bool dropCoin(int column, int player)
        {
            if (this.map[0, column] != 0) // if column already full
                return false;

            int x = Game1.VX - 1;
            while (this.map[x, column] != 0) // found first empty from the bottom
                x--;

            this.map[x, column] = Convert.ToByte(player);
            return true;
        }

        public int getWinner()
        {
            var winner = 0;
            for (int i = 0; i < Game1.VX; i++)
            {
                winner = getPlayerThatHasFourInArray(getColumn(i));
                if (winner != 0) return winner;
            }
            for (int i = 0; i < Game1.VY; i++)
            {
                winner = getPlayerThatHasFourInArray(getRow(i));
                if (winner != 0) return winner;
            }

            byte[][] diagonals = getDiagonals();
            foreach (var diagonal in diagonals)
            {
                winner = getPlayerThatHasFourInArray(diagonal);
                if (winner != 0) return winner;
            }

            if (isMapFull()) return -1;

            return 0;
        }

        public int getPlayerThatHasFourInArray(byte[] array)
        {
            for (int p = 1; p <= 2; p++)
            {
                byte connected = 0;
                var previous = 0;
                foreach (byte current in array)
                {
                    if (current != p)
                    {
                        connected = 0;
                        continue;
                    }
                    if (current != previous)
                    {
                        connected = 1;
                        previous = current;
                        continue;
                    }
                    connected++;
                    if (connected == 4)
                    {
                        return p;
                    }
                }
            }
            return 0;
        }

        public byte[] getColumn(int row)
        {
            var result = new byte[Game1.VY - 1];
            for (int i = 0; i < Game1.VY; i++)
            {
                result = result.Append(map[row, i]).ToArray();
            }
            return result;
        }

        public byte[] getRow(int column)
        {
            var result = new byte[Game1.VX - 1];
            for (int i = 0; i < Game1.VX; i++)
            {
                result = result.Append(map[i, column]).ToArray();
            }
            return result;
        }

        public byte[][] getDiagonals()
        {
            byte[][] diagonals = new byte[][] { };
            int x;
            for (int way = 0; way < 2; way++)
            {
                for (int offset = -Game1.VX + 1; offset < Game1.VX; offset++)
                {
                    byte[] diagonal = new byte[] { };
                    for (int y = 0; y < Game1.VY; y++)
                    {
                        x = way == 1 ? y + offset : Game1.VX - 1 - y + offset;
                        if (x < 0 || x > Game1.VX - 1 || y < 0 || y > Game1.VY) continue;
                        Array.Resize(ref diagonal, diagonal.Length + 1);
                        diagonal[diagonal.Length - 1] = map[x, y];
                    }
                    if (diagonal.Length >= 4)
                    {
                        Array.Resize(ref diagonals, diagonals.Length + 1);
                        diagonals[diagonals.Length - 1] = diagonal;
                    }
                }
            }
            return diagonals;
        }

    }
}

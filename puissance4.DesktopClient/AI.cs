using System;
namespace puissance4.DesktopClient
{
    internal class AI
    {
        public int Play()
        {
            Random rnd = new Random();
            return rnd.Next(0, 7);
        }

        public int test(Board board, int ToMove)
        {
            int best = -1;
            double best_ratio = 0;
            int games_per_move = 10000;
            for (int move = 0; move < 7; move++)
            {
                if (board.getColumn(move)[0] != 0) continue; // full column
                int won = 0, lost = 0;
                for (int i = 0; i < games_per_move; i++)
                {
                    Board copy = board.copy();
                    copy.dropCoin(move, ToMove);
                    if (copy.getWinner() == ToMove) return move;
                    int next = (ToMove == 1) ? 2 : 1;
                    int winner = randomGame(copy, next); // ?
                    if (winner == ToMove) won++;
                    else lost++;
                }
                double ratio = (double)won / (lost+1);
                if (ratio > best_ratio || best == -1)
                {
                    best = move;
                    best_ratio = ratio;
                }
            }
            return best;
        }

        private int randomGame(Board board, int ToMove)
        {
            Random rdm = new Random();
            while (true)
            {
                if(board.dropCoin(rdm.Next(0, 7), ToMove)) 
                    ToMove = (ToMove == 1) ? 2 : 1;
                int winner = board.getWinner();
                if (winner != 0) return winner;
            }
        }
    }
}
﻿using Chess.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Pieces
{
    public class Knight : Piece
    {
        #region constructor

        public Knight(char player) : base(player) { }

        #endregion

        private readonly static int[][] MoveTemplates = new int[][]
        {
            new [] { 1, 2 }, // right 1 up 2
            new [] { 2, 1 }, // right 2 up 1
            new [] { -1, 2 }, // left 1 up 2
            new [] { -2, 1 }, // left 2 up 1
            new [] { -2, -1 }, // left 2 down 1
            new [] { -1, -2 }, // left 1 down 2
            new [] { 1, -2 }, // right 1 down 2
            new [] { 2, -1 }, // right 2 down 1
        };

        public override IList<Tile> GetValidMoves(Board board)
        {
            return Movement.GetMoves(board, this, 1, MoveTemplates);
        }
    }
}

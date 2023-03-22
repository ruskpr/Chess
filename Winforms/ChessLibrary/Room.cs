﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary
{
    public class Room
    {
        public User PlayerOne { get; set; }
        public User PlayerTwo { get; set; }
        public Room(User pOne, User pTwo)
        {
            PlayerOne = pOne;
            PlayerTwo = pTwo;
        }
        public Room()
        {

        }
    }
}
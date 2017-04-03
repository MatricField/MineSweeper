﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace MineSweep.Model
{
    [DataContract]
    public sealed class Mine:
        Cell, IEquatable<Mine>
    {
        public Mine(int x, int y) :
            base(x, y)
        {
        }

        public override bool Equals(object obj)
        {
            switch(obj)
            {
            case Mine rhs:
                return Equals(rhs);
            default:
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(Mine other)
        {
            return Equals((Cell)other);
        }
    }
}

using System;

[Flags]
public enum MapSides
{
    Nothing = 0,
   Front = 1,
   Left = 2,
   Right = 4,
   LeanLeft = 8,
   LeanRight = 16,
   Cringe = 32,
}

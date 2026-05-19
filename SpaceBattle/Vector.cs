using System;
using System.Linq;
using System.Numerics;


namespace SpaceBattle;

public class NVector
{
    public int[] Coords { get; }

    public NVector(params int[] coords) => Coords = coords;

    public static NVector operator +(NVector a, NVector b)
    {
        if (a.Coords.Length != b.Coords.Length)
            throw new ArgumentException("Разные размерности");

        int[] result = a.Coords.Zip(b.Coords, (x, y) => x + y).ToArray();

        return new NVector(result);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;
        return obj is NVector other && Coords.SequenceEqual(other.Coords);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            foreach (var coord in Coords)
            {
                hash = hash * 31 + coord;
            }
            return hash;
        }
    }

    public static bool operator ==(NVector? a, NVector? b) => Equals(a, b);
    public static bool operator !=(NVector? a, NVector? b) => !Equals(a, b);
}
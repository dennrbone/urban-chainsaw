using System;

namespace SpaceBattle
{
    public class Angle
    {
        public static int Denominator { get; private set; } = 8;

        public int Numerator { get; private set; }

        public Angle(int numerator)
        {
            Numerator = Mod(numerator, Denominator);
        }

        public Angle(int numerator, int denominator)
        {
            if (Denominator != denominator)
                throw new ArgumentException($"Знаменатель должен быть {Denominator}");

            Numerator = Mod(numerator, Denominator);
        }

        public static Angle operator +(Angle a, Angle b)
        {
            return new Angle(a.Numerator + b.Numerator);
        }

        public static bool operator ==(Angle a, Angle b)
        {
            if (ReferenceEquals(a, null))
                return ReferenceEquals(b, null);

            return a.Equals(b);
        }

        public static bool operator !=(Angle a, Angle b)
        {
            return !(a == b);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            return Equals((Angle)obj);
        }

        public bool Equals(Angle other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return Numerator == other.Numerator;
        }

        public override int GetHashCode()
        {
            return Numerator.GetHashCode();
        }

        private static int Mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }

        public double Sin()
        {
            double radians = (Numerator * 2.0 * Math.PI) / Denominator;
            return Math.Sin(radians);
        }

        public double Cos()
        {
            double radians = (Numerator * 2.0 * Math.PI) / Denominator;
            return Math.Cos(radians);
        }

        public static implicit operator double(Angle angle)
        {
            return (angle.Numerator * 2.0 * Math.PI) / Denominator;
        }
    }
}
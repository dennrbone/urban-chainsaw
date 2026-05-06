using System;
using Xunit;

namespace SpaceBattle.Tests
{
    public class AngleTests
    {
        [Fact]
        public void Sum_Angle()
        {
            var angle1 = new Angle(5, 8);
            var angle2 = new Angle(7, 8);

            var result = angle1 + angle2;

            Assert.Equal(4, result.Numerator);
            Assert.Equal(8, Angle.Denominator);
        }

        [Fact]
        public void Equals_Angle()
        {
            var angle1 = new Angle(15, 8);
            var angle2 = new Angle(23, 8);

            Assert.True(angle1.Equals(angle2));
        }

        [Fact]
        public void OperatorEquals_Angle()
        {
            var angle1 = new Angle(15, 8);
            var angle2 = new Angle(23, 8);

            Assert.True(angle1 == angle2);
        }

        [Fact]
        public void NotEquals_Angle()
        {
            var angle1 = new Angle(1, 8);
            var angle2 = new Angle(2, 8);

            Assert.False(angle1.Equals(angle2));
        }

        [Fact]
        public void OperatorNotEquals_Angle()
        {
            var angle1 = new Angle(1, 8);
            var angle2 = new Angle(2, 8);

            Assert.True(angle1 != angle2);
        }

        [Fact]
        public void GetHashCode_Angle()
        {
            var angle = new Angle(5, 8);

            var hashCode = angle.GetHashCode();

            Assert.IsType<int>(hashCode);
        }

        [Fact]
        public void MathCos_Angle()
        {
            var angle = new Angle(2, 8);

            var cos = Math.Cos(angle);

            Assert.Equal(0, cos, 10);
        }

        [Fact]
        public void MathSin_Angle()
        {
            var angle = new Angle(2, 8);

            var sin = Math.Sin(angle);

            Assert.Equal(1, sin, 10);
        }

        [Fact]
        public void Normalization_Angle()
        {
            var angle1 = new Angle(15);
            var angle2 = new Angle(7);

            Assert.Equal(angle1, angle2);
            Assert.Equal(7, angle1.Numerator);
        }
    }
}
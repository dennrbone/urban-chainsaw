using Xunit;
using SpaceBattle;
using System;

namespace SpaceBattle.Tests;

public class NVectorTests
{
    [Fact]
    public void Add_OppositeVectors_ReturnsZeroVector()
    {
        var v1 = new NVector(1, -1, 2);
        var v2 = new NVector(-1, 1, -2);
        var expected = new NVector(0, 0, 0);

        Assert.Equal(expected, v1 + v2);
    }

    [Fact]
    public void Add_LongerAndShorter_ThrowsArgumentException()
    {
        var v1 = new NVector(1, 2, 3);
        var v2 = new NVector(1, 2);

        Assert.Throws<ArgumentException>(() => v1 + v2);
    }

    [Fact]
    public void Add_ShorterAndLonger_ThrowsArgumentException()
    {
        var v1 = new NVector(1, 2);
        var v2 = new NVector(1, 2, 3);

        Assert.Throws<ArgumentException>(() => v1 + v2);
    }

    [Fact]
    public void Equals_SameCoords_ReturnsTrue()
    {
        var v1 = new NVector(1, 2, 3);
        var v2 = new NVector(1, 2, 3);

        Assert.True(v1.Equals(v2));
    }

    [Fact]
    public void OperatorEquals_SameCoords_ReturnsTrue()
    {
        var v1 = new NVector(1, 2, 3);
        var v2 = new NVector(1, 2, 3);

        Assert.True(v1 == v2);
    }

    [Fact]
    public void Equals_DifferentCoords_ReturnsFalse()
    {
        var v1 = new NVector(1, 2);
        var v2 = new NVector(2, 3);

        Assert.False(v1.Equals(v2));
    }

    [Fact]
    public void OperatorNotEquals_DifferentCoords_ReturnsTrue()
    {
        var v1 = new NVector(1, 2);
        var v2 = new NVector(2, 3);

        Assert.True(v1 != v2);
    }

    [Fact]
    public void GetHashCode_ReturnsNotNullValue()
    {
        var v = new NVector(1, 2, 3);

        var hash = v.GetHashCode();
        Assert.IsType<int>(hash);
    }
}

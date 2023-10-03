using System.Numerics;
using System.Reflection;
using Xunit.Sdk;

namespace InlineArrays.Tests;

public class InlineArrayTests
{

    class LengthData : TheoryData<int>
    {
        public LengthData()
        {
            for (int i = 1; i <= InlineArrayGenerator.MaxSize; i++) 
            {
                Add(i);
            }
        }
    }

    IInlineArray<T> CreateInlineArray<T>(int length)
    {
        var assembly = Assembly.GetAssembly(typeof(IInlineArray<>)) ?? throw new InvalidOperationException("Assembly not found");

        var types = assembly.GetTypes();

        var type = assembly.GetType($"InlineArrays.InlineArray{length}`1")?.MakeGenericType(typeof(T)) ?? throw new InvalidOperationException("Type not found");

        return (IInlineArray<T>)Activator.CreateInstance(type)!;
    }

    [Theory]
    [ClassData(typeof(LengthData))]
    public void Length_CorrectValue(int length)
    {
        IInlineArray<int> target = CreateInlineArray<int>(length);

        Assert.Equal(length, target.Length);
    }

    [Theory]
    [ClassData(typeof(LengthData))]
    public void IInlineArrayIndex_Works(int length)
    {
        IInlineArray<int> target = CreateInlineArray<int>(length);

        for (int i = 0; i < length; i++)
        {
            target[i] = i;
        }

        for (int i = 0; i < length; i++)
        {
            Assert.Equal(i, target[i]);
        }
    }

    [Theory]
    [ClassData(typeof(LengthData))]
    public void GetSpan_SpanCorrectlyCreated(int length)
    {
        var target = CreateInlineArray<int>(length);

        for (int i = 0; i < length; i++)
        {
            target[i] = i;
        }

        var span = target.GetSpan();

        Assert.Equal(length, span.Length);

        for (int i = 0; i < length; i++)
        {
            Assert.Equal(i, span[i]);
        }
    }
}
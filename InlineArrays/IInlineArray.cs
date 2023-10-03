
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace InlineArrays;

public interface IInlineArray<T>
{
    int Length { get; }

    Span<T> GetSpan();

    T this[int index] { get; set; }
}
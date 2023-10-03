using Microsoft.CodeAnalysis;
using System.Text;

[Generator]
public class InlineArrayGenerator : ISourceGenerator
{
    public const int MaxSize = 2048;

    const string StructTemplate = @"
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace InlineArrays;

[InlineArray({0})]
public struct InlineArray{0}<T> : IInlineArray<T>
{{
    private T _element0;

    public int Length => {0};

    public Span<T> GetSpan() => MemoryMarshal.CreateSpan(ref Unsafe.As<InlineArray{0}<T>, T>(ref this), {0});

    T IInlineArray<T>.this[int index] {{ get => this[index]; set => this[index] = value; }}
}}";

    const string StaticClassTemplate = @"
using System.Diagnostics;

namespace InlineArrays;

public static class InlineArray
{{
    public static object WithSize<T>(int size)
    {{
        if (size == 0 || size > {1}) throw new ArgumentOutOfRangeException(nameof(size), ""size must be between 1 and {1}"");
        return size switch
        {{
{0}
            _ => throw new UnreachableException(""Invalid Size {{size}}""),
        }};
    }}
}}";

    const string SwitchLineTemplate = @"            {0} => new InlineArray{0}<T>(),";

    public void Execute(GeneratorExecutionContext context)
    {
        var switchLines = new StringBuilder();

        for (int length = 1; length <= MaxSize; length++)
        {
            context.AddSource($"InlineArray{length}.g.cs", string.Format(StructTemplate, length));

            switchLines.AppendFormat(SwitchLineTemplate, length);
            switchLines.AppendLine();
        }

        context.AddSource("InlineArray.g.cs", string.Format(StaticClassTemplate, switchLines.ToString(), MaxSize));
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required for this one
    }
}

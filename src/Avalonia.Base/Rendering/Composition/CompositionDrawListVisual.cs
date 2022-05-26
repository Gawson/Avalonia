using System.Numerics;
using Avalonia.Rendering.Composition.Drawing;
using Avalonia.Rendering.Composition.Server;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Rendering.Composition;

internal class CompositionDrawListVisual : CompositionContainerVisual
{
    public Visual Visual { get; }

    private bool _drawListChanged;
    private CompositionDrawList? _drawList;
    public CompositionDrawList? DrawList
    {
        get => _drawList;
        set
        {
            _drawList?.Dispose();
            _drawList = value;
            _drawListChanged = true;
            RegisterForSerialization();
        }
    }

    private protected override void SerializeChangesCore(BatchStreamWriter writer)
    {
        writer.Write((byte)(_drawListChanged ? 1 : 0));
        if (_drawListChanged)
        {
            writer.WriteObject(DrawList?.Clone());
            _drawListChanged = false;
        }
        base.SerializeChangesCore(writer);
    }

    internal CompositionDrawListVisual(Compositor compositor, ServerCompositionDrawListVisual server, Visual visual) : base(compositor, server)
    {
        Visual = visual;
    }

    internal override bool HitTest(Point pt)
    {
        if (DrawList == null)
            return false;
        if (Visual is ICustomHitTest custom)
            return custom.HitTest(pt);
        foreach (var op in DrawList)
            if (op.Item.HitTest(pt))
                return true;
        return false;
    }
}
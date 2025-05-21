using Jc.OpenNov.Buffers;

namespace Jc.OpenNov.Data;

public class FullSpecification : Encodable
{
    public Specification Specification { get; }
    public long RelativeTime { get; }
    public List<string> Model { get; }

    public FullSpecification(Specification specification, long relativeTime, List<string> model)
    {
        Specification = specification;
        RelativeTime = relativeTime;
        Model = model;
    }

    public static FullSpecification FromAttributes(List<Attribute> attributes)
    {
        var specification = new Specification();
        var relativeTime = 0L;
        var model = new List<string>();

        foreach (var attr in attributes)
        {
            switch (attr.Type)
            {
                case Attribute.AttrIdProdSpecn:
                    using (var buffer = attr.Wrap())
                    {
                        specification = Specification.FromByteBuffer(buffer);
                    }

                    break;

                case Attribute.AttrTimeRel:
                    using (var buffer = attr.Wrap())
                    {
                        relativeTime = buffer.GetUnsignedInt();
                    }

                    break;

                case Attribute.AttrIdModel:
                    using (var buffer = attr.Wrap())
                    {
                        while (buffer.BaseStream.Position < buffer.BaseStream.Length)
                        {
                            model.Add(buffer.GetIndexedString());
                        }
                    }

                    break;
            }
        }

        return new FullSpecification(specification, relativeTime, model);
    }
}
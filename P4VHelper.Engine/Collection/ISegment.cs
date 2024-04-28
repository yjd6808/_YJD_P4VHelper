// jdyun 24/04/27(토)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Engine.Search;

namespace P4VHelper.Engine.Collection
{
    public interface ISegmentElement
    {
        int Key { get; }
        void ReadFrom(BinaryReader _reader);
        void WriteTo(BinaryWriter _writer);
        int CalculateSize();
    }
}

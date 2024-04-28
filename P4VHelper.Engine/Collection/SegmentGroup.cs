using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Engine.Collection
{
    public class SegmentGroup : List<Segment>
    {
        public SegmentType Type => Io.Type;
        public SegmentIo Io { get; }

        public SegmentGroup(int _capacity, SegmentIo _io) : base(_capacity)
        {
            Io = _io;
        }

        public void Ready(int _count)
        {
            int readyCount = _count - Count;    // 추가해줘야하는 세그먼트 수
            int segCount = Count;               // 기존 세그먼트 수

            for (int i = 0; i < readyCount; ++i)
            {
                int segId = i + segCount;
                var seg = new Segment(segId, this);
                Add(seg);
            }
        }

    }
}

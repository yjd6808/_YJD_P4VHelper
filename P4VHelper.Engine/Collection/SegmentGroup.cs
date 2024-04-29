using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Engine.Model;

namespace P4VHelper.Engine.Collection
{
    public class SegmentGroup : List<Segment>
    {
        public SegmentType Type => Io.Type;
        public P4VConfig.SegmentGroup Config { get; }
        public string DirPath => $"segments/{Type.ToString().ToLower()}/{Config.Alias}";
        public int Id { get; }
        public SegmentIo Io { get; }

        public SegmentGroup(P4VConfig.SegmentGroup _config) : base(_config.CachedSegmentCount * 10)
        {
            Config = _config;
            Id = _config.Id;
            Io = SegmentIo.Create(_config.Type, this);
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

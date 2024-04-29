using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Engine.Model;

namespace P4VHelper.Engine.Collection
{
    public class SegmentGroup
    {
        private readonly List<Segment> elements_;

        public SegmentType Type => Config.Type;
        public P4VConfig.SegmentGroup Config { get; }
        public string DirPath => $"segments/{Type.ToString().ToLower()}/{Config.Alias}";
        public int Id { get; }
        public SegmentIo Io { get; }

        public SegmentGroup(P4VConfig.SegmentGroup _config)
        {
            Config = _config;
            Id = _config.Id;
            Io = SegmentIo.Create(_config.Type, this);

            elements_ = new List<Segment>(64);
        }

        public void Ready(int _count)
        {
            lock (this)
            {
                int readyCount = _count - elements_.Count;    // 추가해줘야하는 세그먼트 수
                int segCount = elements_.Count;               // 기존 세그먼트 수

                for (int i = 0; i < readyCount; ++i)
                {
                    int segId = i + segCount;
                    var seg = new Segment(segId, this);
                    elements_.Add(seg);
                }
            }
        }

        public void Clear()
        {
            lock (this)
            {
                foreach (Segment element in elements_)
                {
                    element.Clear();
                }
            }
        }

        public Segment At(int _id)
        {
            lock (this)
            {
                return elements_[_id];
            }
        }
    }
}

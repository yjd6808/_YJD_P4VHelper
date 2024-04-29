// jdyun 24/04/27(토)
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Base.Notifier;
using P4VHelper.Engine.Model;
using P4VHelper.Engine.Search;
using Perforce.P4;

namespace P4VHelper.Engine.Collection
{
    // 어디의 데이터를 읽어서 메모리에 저장할지
    public enum SegmentLoadFrom
    {
        Server,
        File,
    }

    // 어디의 데이터를 읽어서 저장할지
    public enum SegmentSaveFrom
    {
        Server,
        Memory,
    }

    public enum SegmentType
    {
        Changelist,
        Max,
    }

    public enum SegmentState
    {
        Disk,       // 파일에 저장되거나 파일로도 저장되지 않은 상태
        Memory,     // 메모리에 저장되어 있는 상태
    }

    public class Segment
    {
        protected int id_;
        protected List<ISegmentElement>? list_;
        protected SegmentState state_;

        public int Id => id_;
        public int Count => list_?.Count ?? 0;
        public int Capacity => list_?.Capacity ?? 0;
        public SegmentState State => state_;
        public List<ISegmentElement>? List => list_;
        public SegmentGroup Parent { get; }
        public SegmentType Type => Parent.Type;
        public int StartId => id_ * Capacity + 1;
        public int EndId => (id_ + 1) * Capacity;
        public uint Checksum { get; set; }

        public Segment(int _id, SegmentGroup _group)
        {
            id_ = _id;
            state_ = SegmentState.Disk;
            Parent = _group;
        }

        public void Ready(int _capacity)
        {
            lock (this)
            {
                if (list_ == null)
                {
                    list_ = new List<ISegmentElement>(_capacity);
                }

                if (list_.Capacity != _capacity)
                {
                    list_ = new List<ISegmentElement>(_capacity);
                }
            }
        }

        public void Clear()
        {
            lock (this)
            {
                if (list_ != null)
                {
                    list_.Clear();
                    state_ = SegmentState.Disk;
                }
            }
        }

        public void Add(ISegmentElement _item, bool _sort = false)
        {
            lock (this)
            {
                ThrowIfNotReady();

                list_.Add(_item);

                if (_sort)
                {
                    Sort();
                }
            }
        }
        
        public void Load(LoadArgs? _args)
        {
            lock (this)
            {
                ThrowIfNotReady();
                Parent.Io.Load(id_, _args);
                state_ = SegmentState.Memory;
            }
        }

        public void Save(SaveArgs? _args)
        {
            lock (this)
            {
                ThrowIfNotReady();
                ThrowIfNotLoaded();
                Parent.Io.Save(id_, _args);
            }
        }

        public int CalculateSize()
        {
            lock (this)
            {
                ThrowIfNotLoaded();

                int size = sizeof(uint) + sizeof(int); // checksum + count
                for (int i = 0; i < Count; ++i)
                {
                    size += list_[i].CalculateSize();
                }

                return size;
            }
        }

        public void Sort()
        {
            lock (this)
            {
                ThrowIfNotReady();

                list_.Sort((_x, _y) => _y.Key.CompareTo(_x.Key));
            }
        }

        public ISegmentElement? Find(int _key)
        {
            return default;
        }

        protected void ThrowIfNotReady()
        {
            if (list_ == null)
                throw new Exception("리스트가 준비되지 않았습니다");
        }

        protected void ThrowIfNotLoaded()
        {
            if (state_ == SegmentState.Memory && (list_ == null || list_.Count == 0))
                throw new Exception("로드되지 않은 세그먼트입니다.");
        }

        public class Comparer : IComparer<Segment>
        {
            public int Compare(Segment? x, Segment? y)
            {
                return x.id_.CompareTo(y.id_);
            }
        }
    }

    public abstract class LoadArgs
    {
        public class Changelist : LoadArgs
        {
            public bool ForceServer { get; set; } = false;
            public static LoadArgs Create(bool _forceServer) => new Changelist() { ForceServer = _forceServer };
        }
    }

    public abstract class SaveArgs
    {
        public class Changelist : SaveArgs
        {
            public bool ForceServer { get; set; } = false;
            public static SaveArgs Create(bool _forceServer) => new Changelist() { ForceServer = _forceServer };
        }
    }
}

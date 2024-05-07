// jdyun 24/04/27(토)
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Base.Extension;
using P4VHelper.Base.Notifier;
using P4VHelper.Engine.Model;
using P4VHelper.Engine.Param;
using P4VHelper.Engine.Search;

namespace P4VHelper.Engine.Collection
{
    public enum SegmentType
    {
        Changelist,
        ChangelistByUser,
        Max,
    }

    public enum SegmentState
    {
        None,       // 아무것도 없는 상태
        Disk,       // 해당 세그먼트 파일이 존재하는 상태
        Memory,     // 메모리에 저장되어 있는 상태
    }

    public class Segment
    {
        protected int id_;
        protected List<ISegmentElement>? elements_;
        protected SegmentState state_;

        public int Id => id_;
        public int Count => elements_?.Count ?? 0;
        public int Capacity => elements_?.Capacity ?? 0;
        public SegmentState State => state_;
        public List<ISegmentElement>? Elements => elements_;
        public SegmentGroup Parent { get; }
        public SegmentType Type => Parent.Type;
        public int StartId => id_ * Capacity + 1;
        public int EndId => (id_ + 1) * Capacity;
        public uint Checksum { get; set; }
        public string FilePath => Parent.Io.GetFilePath(this);

        public Segment(int _id, SegmentGroup _group)
        {
            id_ = _id;
            Parent = _group;

            if (File.Exists(FilePath))
            {
                byte[] checksumBytes = FileEx.ReadBytes(FilePath, 4);

                state_ = SegmentState.Disk;
                Checksum = BitConverter.ToUInt32(checksumBytes);
            }
            else
            {
                state_ = SegmentState.None;
            }
        }

        public void Ready(int _capacity)
        {
            lock (this)
            {
                if (elements_ == null)
                {
                    elements_ = new List<ISegmentElement>(_capacity);
                }

                if (elements_.Capacity != _capacity)
                {
                    elements_ = new List<ISegmentElement>(_capacity);
                }
            }
        }

        public void Clear()
        {
            lock (this)
            {
                if (elements_ != null)
                {
                    elements_.Clear();
                }
            }
        }

        public void Add(ISegmentElement _item, bool _sort = false)
        {
            lock (this)
            {
                ThrowIfNotReady();

                elements_.Add(_item);

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

                if (Parent.Io.Load(this, _args))
                {
                    state_ = SegmentState.Memory;
                }
            }
        }

        public void Save(SaveArgs? _args)
        {
            lock (this)
            {
                ThrowIfNotReady();
                ThrowIfNotLoaded();
                Parent.Io.Save(this, _args);
            }
        }

        public bool Search(SearchParam _param)
        {
            int enteredSlot = _param.Result.Enter();
            _param.Notifier?.Progress(0);
            bool finished = false;

            lock (this)
            {
                ThrowIfNotReady();
                ThrowIfNotLoaded();

                if (state_ == SegmentState.Disk)
                {
                    Load(null);
                }

                foreach (ISegmentElement element in elements_)
                {
                    if (element.IsMatch(_param))
                    {
                        ProgressState state = _param.Notifier.Progress(1);
                        _param.Result.Slot[enteredSlot].Add(element);

                        if (state.HasFlag(ProgressState.Finished))
                        {
                            finished = true;
                            break;
                        }
                    }
                }

                Clear();
            }

            _param.NotifySlot(enteredSlot);
            _param.Result.Leave(enteredSlot);
            return finished;
        }

        public int CalculateSize()
        {
            lock (this)
            {
                ThrowIfNotLoaded();

                int size = sizeof(uint) + sizeof(int); // checksum + count
                for (int i = 0; i < Count; ++i)
                {
                    size += elements_[i].CalculateSize();
                }

                return size;
            }
        }

        public void Sort()
        {
            lock (this)
            {
                ThrowIfNotReady();

                elements_.Sort((_x, _y) => _y.Key.CompareTo(_x.Key));
            }
        }

        public ISegmentElement? Find(int _key)
        {
            return default;
        }

        protected void ThrowIfNotReady()
        {
            if (elements_ == null)
                throw new Exception("리스트가 준비되지 않았습니다");
        }

        protected void ThrowIfNotLoaded()
        {
            if (state_ == SegmentState.None || elements_ == null)
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

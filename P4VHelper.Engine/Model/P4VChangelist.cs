// jdyun 24/04/10(수)
using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Base;
using P4VHelper.Base.Extension;
using P4VHelper.Engine.Collection;
using P4VHelper.Engine.Search;

using NativeChangelist = Perforce.P4.Changelist;

namespace P4VHelper.Engine.Model
{
    // 인스턴스 메모리 크기: 160 byte (언젠가 최적화 필요... FieldHolder 안쓰면 50byte 나옴)
    public class P4VChangelist : 
        Bindable,
        ISegmentElement,
        IComparable<P4VChangelist>
    {
        public enum Member
        {
            //[Description("리비전")]
            Revision,
            //[Description("날짜")]
            Date,
            //[Description("이름")]
            UserName,
            //[Description("설명")]
            Description,

            [Browsable(false)]
            Max
        }

        public static readonly IComparer<P4VChangelist> s_DefaultComparer = new DescendingComparer<P4VChangelist>();

        public P4VChangelist()
        {
            revision_ = new FieldHolder<int>(0);
            date_ = new FieldHolder<DateTime>(new DateTime());
            userName_ = new FieldHolder<string>(string.Empty);
            description_ = new FieldHolder<string>(string.Empty);
        }

        public P4VChangelist(NativeChangelist _changelist)
        {
            revision_ = new FieldHolder<int>(_changelist.Id);
            date_ = new FieldHolder<DateTime>(_changelist.ModifiedDate);
            userName_ = new FieldHolder<string>(_changelist.OwnerName);
            description_ = new FieldHolder<string>(_changelist.Description);
        }

        private readonly FieldHolder<int> revision_;
        public FieldHolder<int> RevisionHolder => revision_;
        public int Revision
        {
            get => revision_.Value;
            set
            {
                revision_.Value = value;
                OnPropertyChanged();
            }
        }

        private readonly FieldHolder<DateTime> date_;
        public FieldHolder<DateTime> DateHolder => date_;
        public DateTime Date
        {
            get => date_.Value;
            set
            {
                date_.Value = value;
                OnPropertyChanged();
            }
        }

        private readonly FieldHolder<string> userName_;
        public FieldHolder<string> UserNameHolder => userName_;
        public string UserName
        {
            get => userName_.Value;
            set
            {
                userName_.Value = value; 
                OnPropertyChanged();
            }
        }

        private readonly FieldHolder<string> description_;
        public FieldHolder<string> DescriptionHolder => description_;
        public string Description
        {
            get => description_.Value;
            set
            {
                description_.Value = value;
                OnPropertyChanged();
            }
        }

        public int Key => revision_.Value;
        public SegmentType Type => SegmentType.Changelist;

        public void WriteTo(BinaryWriter _writer)
        {
            _writer.Write(Revision);
            _writer.Write(Date.Ticks);
            _writer.Write(UserName);
            _writer.Write(Description);
        }

        public void ReadFrom(BinaryReader _reader)
        {
            revision_.Value = _reader.ReadInt32();
            date_.Value = new DateTime(_reader.ReadInt64());
            userName_.Value = _reader.ReadString();
            description_.Value = _reader.ReadString();
        }

        public int CalculateSize()
        {
            int size = 0;
            size += sizeof(int);
            size += sizeof(long);
            size += Encoding.UTF8.GetByteCount(userName_.Value);
            size += Encoding.UTF8.GetByteCount(description_.Value);

            // Write7BitEncodedInt() 함수에서 길이를 7bit단위로 끊어서 저장하는데
            // 길이가 127보다 짧으면 1바이트만 저장하고 더 크면 8bit를 1로 set하고 8bit 이후의 다시 7bit를 읽어서 뭐 그런식으로 처리함
            // 무튼 길이 저장용도의 공간을 사용하고 있으므로 널널하게 4바이트를 저장하도록 한다.
            // 문자열이 userName과 description 2개이므로 8바이트를 더함
            size += 8;  
            return size;
        }

        public int CompareTo(P4VChangelist? _other)
        {
            if (_other is null)
                throw new ArgumentException($"other must be of type {nameof(P4VChangelist)}");

            return revision_.Value.CompareTo(_other.revision_.Value);
        }

        public class Reflection : Reflection<P4VChangelist>
        {
            static Reflection()
            {
                setters_ = new Action<P4VChangelist, IFieldHolder>[(int)Member.Max];
                setters_[(int)Member.Revision]      = ((_changelist, _holder) => _holder.Set(_changelist.Revision));
                setters_[(int)Member.Date]          = ((_changelist, _holder) => _holder.Set(_changelist.Date));
                setters_[(int)Member.UserName]      = ((_changelist, _holder) => _holder.Set(_changelist.UserName));
                setters_[(int)Member.Description]   = ((_changelist, _holder) => _holder.Set(_changelist.Description));

                getters_ = new Func<P4VChangelist, IFieldHolder>[(int)Member.Max];
                getters_[(int)Member.Revision]      = ((_changelist) => _changelist.RevisionHolder);
                getters_[(int)Member.Date]          = ((_changelist) => _changelist.DateHolder);
                getters_[(int)Member.UserName]      = ((_changelist) => _changelist.UserNameHolder);
                getters_[(int)Member.Description]   = ((_changelist) => _changelist.DescriptionHolder);

                segTypes_ = new SegmentType[(int)Member.Max];
                segTypes_[(int)Member.Revision]      = SegmentType.Changelist;
                segTypes_[(int)Member.Date]          = SegmentType.Changelist;
                segTypes_[(int)Member.UserName]      = SegmentType.Changelist;
                segTypes_[(int)Member.Description]   = SegmentType.ChangelistByUser;
            }

            public Reflection() : base((int)Member.Max)
            {

            }
        }
        
    }
}

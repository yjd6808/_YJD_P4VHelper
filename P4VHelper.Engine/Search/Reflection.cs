// jdyun 24/04/21(일)
using P4VHelper.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Engine.Collection;

namespace P4VHelper.Engine.Search
{
    public interface IReflection
    {
        public SegmentType GetSegType(int _member);
    }

    public abstract class Reflection<T> : IReflection where T : ISegmentElement
    {
        public Action<T, IFieldHolder>[] setters_;
        public Func<T, IFieldHolder>[] getters_;
        public SegmentType[] segTypes_;

        public int maxKey_;

        public Reflection(int _maxKey)
        {
            maxKey_ = _maxKey;
        }

        /// <summary>
        /// T 객체의 _member에 해당하는 멤버변수를 가져온다
        /// </summary>
        public IFieldHolder GetField(T _segElem, int _member)
        {
            if (_member < 0 || _member >= maxKey_ )
                throw new ArgumentOutOfRangeException();

            return getters_[_member](_segElem);
        }

        /// <summary>
        /// T 객체의 _member에 해당하는 멤버변수는 어떤 타입의 세그먼트 정보를 가지고 있는지 반환한다.
        /// UserName의 경우 Segmenttype.Changelist에서도 검색할 수 있지만 더 효율적으로 이름만 인덱싱하여 검색하는 경우가 있어서 추가하였다.
        /// UserName은 SegmentType.ChangelistByUser 타입의 세그먼트에서 먼저 검색한다.
        /// </summary>
        public SegmentType GetSegType(int _member)
        {
            if (_member < 0 || _member >= maxKey_)
                throw new ArgumentOutOfRangeException();

            return segTypes_[_member];
        }
    }

    public class ReflectionMgr
    {
        public static IReflection[] reflections_;

        static ReflectionMgr()
        {
            reflections_ = new IReflection[(int)SegmentType.Max];
            reflections_[0] = new P4VChangelist.Reflection();
        }

        public static Reflection<T> Get<T>(T _elem) where T : ISegmentElement
        {
            return (Reflection<T>)reflections_[(int)_elem.Type];
        }

        public static IReflection Get(SegmentType _type)
        {
            return reflections_[(int)_type];
        }
    }
}

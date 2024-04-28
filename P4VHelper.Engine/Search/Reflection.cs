// jdyun 24/04/21(일)
using P4VHelper.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Engine.Search
{
    public abstract class Reflection<T> where T : ISearchable
    {
        public static ThreadLocal<IFieldHolder>[] holders_;
        public static Action<T, IFieldHolder>[] setters_;
        public static Func<T, IFieldHolder>[] getters_;

        public int maxKey_;

        public Reflection(int _maxKey)
        {
            maxKey_ = _maxKey;
        }

        public IFieldHolder GetField(T _searchable, int _member)
        {
            if (_member < 0 || _member >= maxKey_ )
                throw new ArgumentOutOfRangeException();

            return getters_[_member](_searchable);
        }
    }

    public class ReflectionMgr
    {
        public static object[] reflections_;

        static ReflectionMgr()
        {
            reflections_ = new object[(int)SearchableType.Max];
            reflections_[0] = new P4VChangelist.Reflection();
        }

        public static Reflection<T> Get<T>(T _searchable) where T : ISearchable
        {
            return (Reflection<T>)reflections_[(int)_searchable.SearchableType];
        }
    }
}

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
        public static ThreadLocal<IFieldHolder>[] Holders;
        public static Action<T, IFieldHolder>[] Setters;
        public static Func<T, IFieldHolder>[] Getters;

        public int MaxKey;

        public Reflection(int maxKey)
        {
            MaxKey = maxKey;
        }

        public IFieldHolder GetField(T searchable, int member) => Getters[member](searchable);
    }

    public class ReflectionMgr
    {
        public static object[] Reflections;

        static ReflectionMgr()
        {
            Reflections = new object[(int)SearchableType.Max];
            Reflections[0] = new Changelist._Reflection();
        }

        public static Reflection<T> Get<T>(T searchable) where T : ISearchable
        {
            return (Reflection<T>)Reflections[(int)searchable.SearchableType];
        }
    }
}

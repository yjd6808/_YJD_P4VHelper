using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using P4VHelper.Base.Extension;
using P4VHelper.Engine.Search;

namespace P4VHelper.Engine.Cache
{

    public class TimeCache
    {
        private Dictionary<int, Value> map_;
        public object this[int _key] => Get<object>(_key);

        public TimeCache()
        {
            map_ = new();
        }

        public void Add(int _key, TimeSpan _delay)
        {
            lock (this)
            {
                if (!map_.TryAdd(_key, new Value() { Delay = _delay }))
                    throw new Exception($"동일한 타임 캐시 키가 존재합니다.");
            }
        }

        public bool Elapsed(int _key)
        {
            return Elapsed(_key, TimeSpan.Zero);
        }

        public bool Elapsed(int _key, TimeSpan _delay)
        {
            DateTime now = DateTime.Now;

            lock (this)
            {
                if (map_.TryGetValue(_key, out Value value))
                {
                    // 전달받은 딜레이를 더 우선시
                    if (_delay.Ticks != 0 && (now - value.Check) > _delay)
                        return true;

                    if ((now - value.Check) > value.Delay)
                        return true;

                    return false;
                }
                else
                {
                    Value newValue = new Value();
                    newValue.Check = now;
                    map_.Add(_key, newValue);
                    return true;
                }
            }
        }

        public void Set(int _key, object _data)
        {
            lock (this)
            {
                if (map_.TryGetValue(_key, out Value value))
                {
                    value.Data = _data;
                    value.Check = DateTime.Now;
                    return;
                }

                throw new Exception("존재하지 않는 캐시 데이터에 접근했습니다.");
            }
        }

        public T Get<T>(int _key)
        {
            lock (this)
            {
                return (T)map_[_key].Data;
            }
        }

        public class Value
        {
            public TimeSpan Delay = TimeSpan.FromSeconds(120.0);    // 캐시 딜레이
            public DateTime Check;                                  // 체크한 시점
            public object Data;                                     // 체크한 시점의 데이터
        }
    }
}

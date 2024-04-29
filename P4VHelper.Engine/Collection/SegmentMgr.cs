// jdyun 24/04/27(토)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using P4VHelper.Base.Extension;
using P4VHelper.Base.Notifier;
using P4VHelper.Engine.Cache;
using P4VHelper.Engine.Model;
using P4VHelper.Engine.Search;

using NativeChangelist = Perforce.P4.Changelist;

namespace P4VHelper.Engine.Collection
{
   
    public class SegmentMgr
    {
        private readonly List<SegmentGroup> groups_;
        private readonly P4VEngine engine_;
        private readonly TimeCache timeCache_;

        public SegmentMgr(P4VEngine _engine)
        {
            engine_ = _engine;
            timeCache_ = new TimeCache();
            groups_ = ArrayEx.Create<SegmentGroup>(_engine.Config.Count, () => null).ToList();

            for (int i = 0; i < _engine.Config.Count; ++i)
            {
                P4VConfig.SegmentGroup groupConfig = engine_.Config.GetSegmentGroupById(i);
                groups_[i] = new SegmentGroup(groupConfig);
            }
        }

        public SegmentGroup GetGroup(SegmentType _type, string _alias)
        {
            for (int i = 0; i < groups_.Count; ++i)
            {
                SegmentGroup group = groups_[i];
                if (group.Config.Type == _type && group.Config.Alias == _alias)
                    return group;
            }

            throw new Exception($"{_type} {_alias}에 해당하는 세그먼트 그룹을 찾지 못함");
        }

        /// <summary>
        /// alias 그룹중 member를 가장 효율적으로 다루는 그룹을 찾는다.
        /// </summary>
        public SegmentGroup GetGroup(string _alias, int _member)
        {
            for (int i = 0; i < groups_.Count; ++i)
            {
                SegmentGroup group = groups_[i];
                if (group.Config.Alias == _alias)
                {
                    SegmentType memberSegType = ReflectionMgr.Get(group.Type).GetSegType(_member);
                    if (group.Type == memberSegType)
                        return group;

                    // 차선책으로 조금 비효율적이겠지만 동일한 Alias이므로 그냥반환함
                    // 예를들어 UserName은 SegmentType이 ChangelistByUser 세그먼트에서 검색하는게 가장 효율적인데
                    // 이 세그먼트 그룹을 configuration.xml에 정의하지 않은 경우가 있을 수 있는데 이때는 동일한 alias인 세그먼트 그룹을 그냥 반환하도록 한다.
                    return group;
                }
            }

            throw new Exception($"{_alias} {_member}에 해당하는 세그먼트 그룹을 찾지 못함");
        }

        public SegmentGroup GetGroupById(int _id)
        {
            return groups_[_id];
        }

        /// <summary>
        /// 메모리에 로드된 데이터를 모두 초기화 한다.
        /// </summary>
        public void ClearAll()
        {
            for (int i = 0; i < groups_.Count; ++i)
            {
                groups_[i].Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_segType">로딩할 세그먼트</param>
        /// <param name="_alias"></param>
        /// <param name="_notifier"></param>
        /// <param name="_loadCount"></param>
        /// <param name="_save"></param>
        /// <param name="_loadArgs"></param>
        /// <param name="_saveArgs"></param>
        /// <exception cref="Exception"></exception>
        public void Load(SegmentType _segType, string _alias, ProgressNotifer _notifier, int _loadCount = Int32.MaxValue, bool _save = false, LoadArgs? _loadArgs = null, SaveArgs? _saveArgs = null)
        {
            SegmentGroup group = GetGroup(_segType, _alias);

            API.P4.SetPath(group.Config.Path);

            NativeChangelist changelist;

            if (timeCache_.Elapsed(group.Id))
            {
                changelist = API.P4.GetLastChangelist();
                timeCache_.Set(group.Id, changelist);
            }
            else
            {
                changelist = timeCache_.Get<NativeChangelist>(group.Id);
            }

            int totalRevCount = changelist.Id;
            int totalSegmentCount = (totalRevCount - 1) / engine_.Config.SegmentSize + 1;

            group.Ready(totalSegmentCount);

            if (totalRevCount <= 0)
            {
                _notifier.Progress();
                return;
            }

            int availableLoadCount = _loadCount > totalSegmentCount ? totalSegmentCount : totalSegmentCount - _loadCount;

            int startSegmentId = totalSegmentCount - 1;
            int endSegmentid = _loadCount > totalSegmentCount ? 0 : availableLoadCount;

            int leftSaveCount = _save ? availableLoadCount : 0;
            int leftCacheCount = _loadCount < group.Config.CachedSegmentCount ? _loadCount : group.Config.CachedSegmentCount;

            int curLoadedCount = 0;
            int curCachedSegmentCount = 0;

            _notifier.Start(leftSaveCount + leftCacheCount);

            for (int segId = startSegmentId; segId >= endSegmentid; --segId)
            {
                Segment? seg = group.At(segId);

                if (seg == null)
                    throw new Exception($"{segId} 세그먼트가 존재하지 않습니다");

                seg.Ready(engine_.Config.SegmentSize);

                if (curCachedSegmentCount < group.Config.CachedSegmentCount)
                {
                    seg.Load(_loadArgs);
                    _notifier.Progress();
                    curCachedSegmentCount++;
                }

                if (_save)
                {
                    seg.Save(_saveArgs);
                    _notifier.Progress();
                }

                curLoadedCount++;
            }
        }

    }
}

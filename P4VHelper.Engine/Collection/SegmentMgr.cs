// jdyun 24/04/27(토)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using P4VHelper.Base.Notifier;
using P4VHelper.Engine.Model;
using P4VHelper.Engine.Search;

using NativeChangelist = Perforce.P4.Changelist;

namespace P4VHelper.Engine.Collection
{
   
    public class SegmentMgr
    {
        private readonly SegmentGroup[] groups_;
        private readonly P4VEngine engine_;
        private int count_;

        public SegmentMgr(P4VEngine _engine)
        {
            engine_ = _engine;
            groups_ = new SegmentGroup[P4VConfig.SegmentGroup.s_Count];

            for (int i = 0; i < P4VConfig.SegmentGroup.s_Count; ++i)
            {
                P4VConfig.SegmentGroup groupConfig = engine_.Config.GetSegmentGorupById(i);
                groups_[i] = new SegmentGroup(groupConfig);
            }
            count_ = P4VConfig.SegmentGroup.s_Count;
        }

        public SegmentGroup GetGroup(SegmentType _type, string _alias)
        {
            for (int i = 0; i < count_; ++i)
            {
                SegmentGroup group = groups_[i];
                if (group.Config.Type == _type && group.Config.Alias == _alias)
                    return group;
            }

            throw new Exception($"{_type} {_alias}에 해당하는 세그먼트 그룹을 찾지 못함");
        }

        public SegmentGroup GetGroupById(int _id)
        {
            return groups_[_id];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_segmentType">로딩할 세그먼트</param>
        /// <param name="_alias"></param>
        /// <param name="_notifier"></param>
        /// <param name="_loadCount"></param>
        /// <param name="_save"></param>
        /// <param name="_loadArgs"></param>
        /// <param name="_saveArgs"></param>
        /// <exception cref="Exception"></exception>
        public void Load(SegmentType _segmentType, string _alias, ProgressNotifer _notifier, int _loadCount = Int32.MaxValue, bool _save = false, LoadArgs? _loadArgs = null, SaveArgs? _saveArgs = null)
        {
            SegmentGroup group = GetGroup(_segmentType, _alias);

            API.P4.SetPath(group.Config.Path);
            NativeChangelist changelist = API.P4.GetLastChangelist();

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
                Segment? seg = group[segId];

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

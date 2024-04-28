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

        public SegmentMgr(P4VEngine _engine)
        {
            engine_ = _engine;
            groups_ = new SegmentGroup[10];
            groups_[(int)SegmentType.Changelist] = new SegmentGroup(800, new SegmentIo.Changelist());
        }

        public SegmentGroup GetGroup(SegmentType _type)
        {
            return groups_[(int)_type];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_segmentType"></param>
        /// <param name="_searcherAlias"></param>
        /// <param name="_notifier"></param>
        /// <param name="_loadCount"></param>
        /// <param name="_save"></param>
        /// <param name="_loadArgs"></param>
        /// <param name="_saveArgs"></param>
        /// <exception cref="Exception"></exception>
        public void Load(SegmentType _segmentType, string _searcherAlias, ProgressNotifer _notifier, int _loadCount = Int32.MaxValue, bool _save = false, LoadArgs? _loadArgs = null, SaveArgs? _saveArgs = null)
        {
            P4VConfig.Searcher searcher = engine_.Config.GetSearcherByAlias(_searcherAlias);

            API.P4.SetPath(searcher.Path);
            NativeChangelist changelist = API.P4.GetLastChangelist();

            int totalRevCount = changelist.Id;
            int totalSegmentCount = (totalRevCount - 1) / engine_.Config.SegmentSize + 1;

            SegmentGroup group = GetGroup(_segmentType);
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
            int leftCacheCount = _loadCount < searcher.CachedSegmentCount ? _loadCount : searcher.CachedSegmentCount;

            int curLoadedCount = 0;
            int curCachedSegmentCount = 0;

            _notifier.Start(leftSaveCount + leftCacheCount);

            for (int segId = startSegmentId; segId >= endSegmentid; --segId)
            {
                Segment? seg = group[segId];

                if (seg == null)
                    throw new Exception($"{segId} 세그먼트가 존재하지 않습니다");

                seg.Ready(engine_.Config.SegmentSize);

                if (curCachedSegmentCount < searcher.CachedSegmentCount)
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

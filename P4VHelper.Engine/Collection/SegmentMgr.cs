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
using P4VHelper.Engine.Param;
using P4VHelper.Engine.Search;
using NativeChangelist = Perforce.P4.Changelist;

namespace P4VHelper.Engine.Collection
{

    public class SegmentMgr
    {
        private readonly List<SegmentGroup> groups_;
        private readonly P4VEngine engine_;

        public SegmentMgr(P4VEngine _engine)
        {
            engine_ = _engine;
            groups_ = ArrayEx.Create<SegmentGroup>(_engine.Config.Count, () => null).ToList();

            for (int i = 0; i < _engine.Config.Count; ++i)
            {
                P4VConfig.SegmentGroup groupConfig = engine_.Config.GetGroupById(i);
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

        public List<SegmentGroup> GetAliasGroup(string _alias)
        {
            List<SegmentGroup> groups = new List<SegmentGroup>(groups_.Count);

            for (int i = 0; i < groups_.Count; ++i)
            {
                SegmentGroup group = groups_[i];
                if (group.Config.Alias == _alias)
                    groups.Add(group);
                    
            }
            return groups;
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

        public void Load(LoadParam _param)
        {
            _param.Validate();
            SegmentGroup group = GetGroup(_param.Type, _param.Alias);
            group.Load(_param);
        }

        public void Search(SearchParam _param)
        {
            _param.Validate();
            SegmentGroup group = GetGroup(_param.Alias, _param.Member);
            group.Search(_param);
        }
    }
}

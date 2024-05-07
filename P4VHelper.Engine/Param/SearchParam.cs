// jdyun 24/04/21(일)
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Base.Extension;
using P4VHelper.Base.Notifier;
using P4VHelper.Engine.Collection;
using P4VHelper.Engine.Model;
using P4VHelper.Engine.Notification;
using P4VHelper.Engine.Search;

namespace P4VHelper.Engine.Param
{
    public struct SearchResult
    {
        public List<ISegmentElement>[] Slot;
        public int[] IsSlotUse;
        public int SlotCount;

        public SearchResult(int _slotCount)
        {
            // hash collsion을 줄일려면 쓰레드수는 슬롯수보다 작아야한다.
            Debug.Assert(SearchParam.ThreadCount < SearchParam.SlotCount);
            Slot = ArrayEx.Create(_slotCount, () => new List<ISegmentElement>());
            IsSlotUse = new int[_slotCount];
            SlotCount = _slotCount;
        }

        public int Enter()
        {
            int slotIndex = Random.Shared.Next(0, SlotCount);
            while (InterlockedEx.Bool.Cas(ref IsSlotUse[slotIndex], false, true) == false)
            {
                slotIndex += 1; // linear probing ㄱㄱ
                slotIndex %= SlotCount;
            }
            return slotIndex;
        }

        public void Leave(int _slotIndex)
        {
            Debug.Assert(IsSlotUse[_slotIndex] == 1);
            InterlockedEx.Bool.Set(ref IsSlotUse[_slotIndex], true);
        }
    }

    public class SearchParam
    {
        // 이 변수들은 처음 프로그램 로딩시에만 변경 가능하도록 한다.
        // 옵션에서 해당 수치를 변경하고자 설정한 경우 프로그램을 껏다가 켠 경우 반영되도록 해야함
        // P4VHelper.Model.SearchResult<T>에서 참조하여 사용한다.
        public static int SlotCount = 8;
        public static int ThreadCount = 4;

        public SearchParam()
        {
            Result = new SearchResult(SlotCount);
            Alias = string.Empty;
            Rule = Rule.Max;
            Notifier = null;
            Limit = int.MaxValue;
        }

        public string Input;                    // 검색시 입력값 (abcd의 문자열에서 ab를 검색할 때, 이때의 ab를 입력값이라 칭한다.)
        public SearchResult Result;
        public string Alias;                    // 검색 대상인 세그먼트 그룹의 이름
        public int Member;                      // SegmentElement를 상속받은 클래스의 멤버를 가리키는 키값
        public Rule Rule;                       // 검색방식: 일치, 포함, 정규식
        public ProgressNotifer? Notifier;       // 알림기

        // 확장할지 미정
        public int Limit;                       // 최대 몇개의 검색정보만 찾을 것인지 ex) select * from table limit 1000, 할때 그 limit이라고 생각함댐.
        // public int notifirSlot_;             // 알림기의 어떤 유닛에 진행상황을 보고 할지
        // public Unit notificationUnit_;       // 알림 단위

        public void Validate()
        {
            if (string.IsNullOrEmpty(Alias))
                throw new Exception("SeachParam.Alias 설정안됨");

            if (Rule == Rule.Max)
                throw new Exception("SeachParam.Rule 설정안됨");
        }

        public void Start(int _elementCount)
        {
            Notifier.IsThreadSafe = true;

            if (Limit != int.MaxValue)
                Notifier?.Start(_elementCount, Limit);
            else
                Notifier?.Start(_elementCount);
        }

        public void NotifySlot(int _slot)
        {
            // 무조건 크리티컬 섹션이어야함
            ref List<ISegmentElement> elements = ref Result.Slot[_slot];

            if (elements.Count > 0)
            {
                Handler?.Invoke(ref elements, this);
                elements = new List<ISegmentElement>(64);
            }
        }

        public delegate void SlotNotifyHandler(ref List<ISegmentElement> _1, SearchParam _2);
        public event SlotNotifyHandler Handler;
    }
}
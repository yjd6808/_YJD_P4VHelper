// jdyun 24/04/21(일)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Base.Notifier;
using P4VHelper.Engine.Notification;

namespace P4VHelper.Engine.Search
{
    public class SearchParam
    {
        public IFieldHolder input_;          // 검색시 입력값 (abcd의 문자열에서 ab를 검색할 때, 이때의 ab를 입력값이라 칭한다.)
        public int member_;                  // Searchable 인터페이스를 상속받은 클래스의 멤버를 가리키는 값
        public SearchableType searchable_;   // 어떤 Searchable 클래스인지
        public Rule rule_;                   // 검색방식: 일치, 포함, 정규식
        public int limit_;                   // 최대 몇개의 검색정보만 찾을 것인지
        public ProgressNotifer notifier_;    // 알림기
        public int notifirSlot_;             // 알림기의 어떤 유닛에 진행상황을 보고 할지
        public Unit notificationUnit_;       // 알림 단위()
    }
}
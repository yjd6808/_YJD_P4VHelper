// jdyun 24/04/27(토)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Engine.Search;

namespace P4VHelper.Engine.Collection
{
    public interface ISegmentElement
    {
        int Key { get; }
        SegmentType Type { get; }

        /// <summary>
        /// 메모리로부터 데이터를 읽어와서 엘리먼트 필드를 초기화한다.
        /// </summary>
        void ReadFrom(BinaryReader _reader);

        /// <summary>
        /// 엘리먼트 필드를 바이너리 데이터로 변환하여 메모리에 저장한다.
        /// </summary>
        void WriteTo(BinaryWriter _writer);

        /// <summary>
        /// 바이너리크기를 산출한다.
        /// </summary>
        int CalculateSize();

        /// <summary>
        /// 파라미터 정보와 일치하는 대상인지 검사한다.
        /// </summary>
        bool Match(SearchParam _param) => Matchers.IsMatch(this, _param);
    }
}

// jdyun 24/05/01(수) - 근로자의 날
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Engine.Collection;
using P4VHelper.Engine.Search;

namespace P4VHelper.Model
{
    public class SearchState
    {
        public string Alias { get; set; } = string.Empty;
        public Rule Rule { get; set; } = Rule.Max;
        public int Member { get; set; } = int.MaxValue;
        public string InputText { get; set; } = null;

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Alias))
                return false;

            if (Rule == Rule.Max)
                return false;

            if (Member == int.MaxValue)
                return false;

            if (InputText == null)
                return false;

            return true;
        }
    }
}

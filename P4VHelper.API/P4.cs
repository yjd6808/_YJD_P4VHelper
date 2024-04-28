// jdyun 24/04/26(금)
// @참고: https://www.perforce.com/manuals/p4api.net/p4api.net_reference/html/N_Perforce_P4.htm
// @참고: https://github.com/perforce/p4api.net/tree/master/p4api.net
using P4VHelper.API.Internal;
using Perforce.P4;

namespace P4VHelper.API
{
    public class P4
    {
        public const string CMD_CHANGES = "changes";

        private static readonly P4Instance s_P4 = new ();
        private static readonly ThreadLocal<string> s_Path = new ();

        public static Task ConnectAsync(string _uri, string _userName, string _workspace)
            => Task.Run(() => s_P4.Connect(_uri, _userName, _workspace));

        public static void SetPath(string _path)
            => s_Path.Value = _path;

        public static Changelist GetLastChangelist()
        {
            ThrowIfNotConnected();
            ThrowIfNotPathSet();

            // p4 changes -s submitted -l -m 1 <path>
            Options options = new Options();
            options["-s"] = "submitted";        // status (submitted, pending, ...)
            options["-l"] = string.Empty;       // show description
            options["-m"] = "1";                // count
            P4CommandResult cmdRet = s_P4.Run(CMD_CHANGES, options, s_Path.Value);
            Changelist changelist = P4Instance.CreateChangelistFromTaggedObject(s_P4, cmdRet.TaggedOutput[0]);
            return changelist;
        }

        public static Task<Changelist> GetLastChangelistAsync()
            => Task.Run(GetLastChangelist);

        public static List<Changelist> GetChangelists(Range _rangeRev)
        {
            ThrowIfNotConnected();
            ThrowIfNotPathSet();

            int startRev = _rangeRev.Start.Value;
            int endRev = _rangeRev.End.Value;

            // p4 changes -s submitted -l <path>@<start>,@<start + count>
            Options options = new Options();
            List<Changelist> changelists = new(endRev - startRev);

            options["-s"] = "submitted";    // status (submitted, pending, ...)
            options["-l"] = string.Empty;   // show description

            P4CommandResult cmdRet = s_P4.Run(CMD_CHANGES, options, $"{s_Path.Value}@{startRev},@{endRev}");
            foreach (var taggedObject in cmdRet.TaggedOutput)
            {
                Changelist changelist = P4Instance.CreateChangelistFromTaggedObject(s_P4, taggedObject);
                changelists.Add(changelist);
            }
            return changelists;
        }

        public static Task<List<Changelist>> GetChangelistsAsync(Range _rangeRev)
            => Task.Run(() => GetChangelists(_rangeRev));

        public static List<Changelist> GetChangelists(int _startRev, int _count)
        {
            ThrowIfNotConnected();
            ThrowIfNotPathSet();

            // p4 changes -s submitted -l -m <count> <path>@<start>
            Options options = new Options();
            List<Changelist> changelists = new(_count);

            options["-s"] = "submitted";    // status (submitted, pending, ...)
            options["-l"] = string.Empty;   // show description
            options["-m"] = $"{_count}";    // count

            P4CommandResult cmdRet = s_P4.Run(CMD_CHANGES, options, $"{s_Path}@{_startRev}");
            foreach (var taggedObject in cmdRet.TaggedOutput)
            {
                Changelist changelist = P4Instance.CreateChangelistFromTaggedObject(s_P4, taggedObject);
                changelists.Add(changelist);
            }

            return changelists;
        }

        public static Task<List<Changelist>> GetChangelistsAsync(int _startRev, int _count)
            => Task.Run(() => GetChangelists(_startRev, _count));

        private static void ThrowIfNotConnected()
        {
            if (!s_P4.IsConnected)
                throw new Exception("연결 안됨");
        }

        private static void ThrowIfNotPathSet()
        {
            if (string.IsNullOrEmpty(s_Path.Value))
                throw new Exception("히스토리 경로가 설정안됨");
        }
    }
}

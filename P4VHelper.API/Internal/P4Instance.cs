// jdyun 24/04/26(금)
using Perforce.P4;

namespace P4VHelper.API.Internal
{
    internal class P4Instance
    {
        private Server server_;
        private Repository repo_;
        private Connection conn_;
        private bool isDstMismatched_;
        private string utc_;

        public Server Server => server_;
        public Connection Connection => conn_;
        public Repository Repository => repo_;

        public bool IsConnected => conn_.Status == ConnectionStatus.Connected;

        public P4Instance()
        {
            server_ = new Server(new ServerAddress(string.Empty));
            repo_ = new Repository(server_);
            conn_ = repo_.Connection;
            utc_ = string.Empty;
        }

        // @참고: https://www.perforce.com/manuals/p4api.net/Content/P4API_NET/initialize-connection.html#Initialize_a_connection
        public void Connect(string _uri, string _userName, string _workspace)
        {
            server_ = new Server(new ServerAddress(_uri));
            repo_ = new Repository(server_);
            conn_ = repo_.Connection;
            conn_.UserName = _userName;
            conn_.Client = new Client();
            conn_.Client.Name = _workspace;
            conn_.Connect(null);

            isDstMismatched_ = FormBase.DSTMismatch(server_.Metadata);  // 일광 절약 시간제 불일치 여부
            utc_ = server_.Metadata.DateTimeOffset;                     // UTC 표준시(한국이면 무조건 +9:00)
        }

        public P4CommandResult Run(string _command, Options _options, params string[] _args)
        {
            P4Command p4Cmd = new P4Command(repo_, _command, true, _args);
            return p4Cmd.Run(_options);
        }

        public static Changelist CreateChangelistFromTaggedObject(P4Instance _p4, TaggedObject _obj)
        {
            Changelist changelist = new Changelist();
            changelist.FromChangeCmdTaggedOutput(_obj, _p4.utc_, _p4.isDstMismatched_);
            return changelist;
        }
    }
}

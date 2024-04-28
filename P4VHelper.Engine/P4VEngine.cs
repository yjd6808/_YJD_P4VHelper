using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using P4VHelper.Base.Notifier;
using P4VHelper.Engine.Collection;
using P4VHelper.Engine.Model;
using Perforce.P4;

using NativeChangelist = Perforce.P4.Changelist;

namespace P4VHelper.Engine
{
    public class P4VEngine
    {
        public static P4VEngine Instance = new();

        public P4VConfig Config { get; }
        public SegmentMgr Mgr { get; }

        private P4VEngine()
        {
            Config = new ();
            Mgr = new SegmentMgr(this);
        }

        public Task ConnectAsync(P4VConfig _config)
        {
            Config.CopyFrom(_config);

            return API.P4.ConnectAsync(
                _config.Uri,
                _config.UserName,
                _config.Workspace);
        }

        
    }
}


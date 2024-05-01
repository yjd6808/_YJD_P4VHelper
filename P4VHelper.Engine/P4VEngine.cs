using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using P4VHelper.API;
using P4VHelper.Base.Notifier;
using P4VHelper.Engine.Collection;
using P4VHelper.Engine.Model;
using Perforce.P4;

using NativeChangelist = Perforce.P4.Changelist;

namespace P4VHelper.Engine
{
    public class P4VEngine
    {
        public P4VConfig Config { get; }
        public SegmentMgr SegmentMgr { get; }
        public bool IsConnected => P4.IsConnected();

        public P4VEngine(P4VConfig _config)
        {
            Config = _config;
            SegmentMgr = new SegmentMgr(this);
        }

        public Task ConnectAsync()
        {
            return API.P4.ConnectAsync(
                Config.Uri,
                Config.UserName,
                Config.Workspace);
        }

        
    }
}


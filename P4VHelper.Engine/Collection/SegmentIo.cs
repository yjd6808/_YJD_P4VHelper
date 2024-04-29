// jdyun 24/04/27(토)
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Base;
using P4VHelper.Base.Extension;
using P4VHelper.Engine.Model;

using NativeChangelist = Perforce.P4.Changelist;

namespace P4VHelper.Engine.Collection
{
    public abstract class SegmentIo
    {
        public abstract string FileExtensionName { get; }
        public abstract SegmentType Type { get; }
        public SegmentGroup Group { get; private set; }

        public abstract void Load(int _segId, LoadArgs? _args);
        public abstract void Save(int _segId, SaveArgs? _args);

        public void SetGroup(SegmentGroup _group) => Group = _group;

        public static SegmentIo Create(SegmentType _type, SegmentGroup _group)
        {
            SegmentIo io = null;
            if (_type == SegmentType.Changelist)
                io = new Changelist();
            if (io == null) 
                throw new Exception("알 수 없는 세그먼트 타입입니다.");

            io.SetGroup(_group);
            return io;
        }

        public class Changelist : SegmentIo
        {
            public override string FileExtensionName => ".bin";
            public override SegmentType Type => SegmentType.Changelist;
            public string GetFilePath(Segment _seg) => $"{_seg.Parent.DirPath}/{_seg.Id}{FileExtensionName}";

            public override void Load(int _segId, LoadArgs? _args)
            {
                Segment seg = Group[_segId];
                LoadArgs.Changelist? args = _args as LoadArgs.Changelist;
                bool forceServer = args?.ForceServer ?? false;

                if (forceServer)
                {
                    LoadFromServer(seg);
                }
                else
                {
                    if (seg.State == SegmentState.Disk)
                    {
                        if (!LoadFromFile(seg))
                        {
                            LoadFromServer(seg);
                        }
                    }
                    else
                    {
                        // 이미 메모리에 올라와있는 경우에는 서버에서 로딩
                        LoadFromServer(seg);
                    }
                }
            }

            public override void Save(int _segId, SaveArgs? _args)
            {
                Segment seg = Group[_segId];
                string path = GetFilePath(seg);
                string dir = seg.Parent.DirPath;
                SaveArgs.Changelist? args = _args as SaveArgs.Changelist;
                bool forceServer = args?.ForceServer ?? false;

                if (seg.State == SegmentState.Disk)
                {
                    Load(_segId, new LoadArgs.Changelist() { ForceServer = forceServer });
                }

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                int byteSize = seg.CalculateSize();
                byte[] bytes = ArrayPool<byte>.Shared.Rent(byteSize);
                MemoryStream stream = new MemoryStream(bytes);
                BinaryWriter writer = new BinaryWriter(stream);

                writer.Write(0);
                writer.Write(seg.List.Count);
                for (int i = 0; i < seg.List.Count; ++i)
                {
                    P4VChangelist elem = seg.List[i] as P4VChangelist;
                    elem.WriteTo(writer);
                }

                int writePos = (int)stream.Position;
                uint checksum = Checksum.ChecksumAvx2(bytes, 4, writePos - 4); // 시작 4바이트는 체크섬 공간이므로 제외
                writer.Seek(0, SeekOrigin.Begin);
                writer.Write(checksum);
                FileEx.WriteAllBytes(path, bytes, 0, writePos);
                ArrayPool<byte>.Shared.Return(bytes);
                seg.Checksum = checksum;

                // 메모리에 캐싱되지 않는 세그먼트는 반환토록 함
                if (seg.State == SegmentState.Disk)
                {
                    seg.Clear();
                }
            }

            public void LoadFromServer(Segment _seg)
            {
                List<NativeChangelist> list = API.P4.GetChangelists(new Range(_seg.StartId, _seg.EndId));
                _seg.Clear();
                for (int i = 0; i < list.Count; ++i)
                {
                    NativeChangelist native = list[i];
                    if (Group.Config.IsMatch(native.Description))
                        continue;

                    P4VChangelist changelistMain = new P4VChangelist(native);
                    _seg.Add(changelistMain);
                }
            }

            public bool LoadFromFile(Segment _seg)
            {
                string path = GetFilePath(_seg);
                if (!File.Exists(path))
                    return false;

                int fileSize = (int)new FileInfo(path).Length;
                byte[] bytes = ArrayPool<byte>.Shared.Rent(fileSize);
                int readBytes = FileEx.ReadAllBytes(path, bytes);
                MemoryStream stream = new MemoryStream(bytes);
                BinaryReader reader = new BinaryReader(stream);
                _seg.Checksum = reader.ReadUInt32();
                int count = reader.ReadInt32();

                for (int i = 0; i < count; ++i)
                {
                    P4VChangelist changelist = new P4VChangelist();
                    changelist.ReadFrom(reader);
                    _seg.Add(changelist);
                }
                ArrayPool<byte>.Shared.Return(bytes);
                return true;
            }
        }
    }
}

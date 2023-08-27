rem protoc.exe --csharp_out=..\FrameUpdate\LocalClient\Assets\Proto .\ProtoBuffs.proto
protoc.exe --csharp_out=../LocalClient/Assets/Proto --proto_path=./ ProtoBuffs.proto
protoc.exe --csharp_out=../LocalServer/ConsoleApplicatLocalServer/Proto --proto_path=./ ProtoBuffs.proto
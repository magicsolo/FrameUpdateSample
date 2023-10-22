// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: ProtoInterface.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace C2SProtoInterface {

  /// <summary>Holder for reflection information generated from ProtoInterface.proto</summary>
  public static partial class ProtoInterfaceReflection {

    #region Descriptor
    /// <summary>File descriptor for ProtoInterface.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ProtoInterfaceReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChRQcm90b0ludGVyZmFjZS5wcm90bxIRQzJTUHJvdG9JbnRlcmZhY2UiJQoI",
            "QzJTTG9naW4SCwoDZ0lkGAEgASgFEgwKBG5hbWUYAiABKAkiOgoIUzJDTG9n",
            "aW4SCwoDZ0lkGAEgASgFEg4KBnVkcFBvdBgCIAEoBRIRCglwbGF5ZXJOdW0Y",
            "AyABKAUiDgoMQzJTU3RhcnRHYW1lIiEKDFMyQ1N0YXJ0R2FtZRIRCglwbGF5",
            "ZXJOdW0YASABKAUiWQoOQzJTRnJhbWVVcGRhdGUSDQoFaW5kZXgYASABKAUS",
            "DQoFc3RhcnQYAiABKAUSCwoDZW5kGAMgASgFEg0KBWFuZ2xlGAQgASgDEg0K",
            "BWlucHV0GAUgASgFIkcKDFMyQ0ZyYW1lRGF0YRISCgpmcmFtZUluZGV4GAEg",
            "ASgFEg4KBmlucHV0cxgCIAMoBRITCgtpbnB1dEFuZ2xlcxgDIAMoAyJdCg5T",
            "MkNGcmFtZVVwZGF0ZRIWCg5jdXJTZXJ2ZXJGcmFtZRgBIAEoBRIzCgpmcmFt",
            "ZURhdGFzGAIgAygLMh8uQzJTUHJvdG9JbnRlcmZhY2UuUzJDRnJhbWVEYXRh",
            "Ki4KCEVNZXNzYWdlEggKBE5vbmUQABIJCgVMb2dpbhABEg0KCVN0YXJ0R2Ft",
            "ZRACYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::C2SProtoInterface.EMessage), }, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::C2SProtoInterface.C2SLogin), global::C2SProtoInterface.C2SLogin.Parser, new[]{ "GId", "Name" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::C2SProtoInterface.S2CLogin), global::C2SProtoInterface.S2CLogin.Parser, new[]{ "GId", "UdpPot", "PlayerNum" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::C2SProtoInterface.C2SStartGame), global::C2SProtoInterface.C2SStartGame.Parser, null, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::C2SProtoInterface.S2CStartGame), global::C2SProtoInterface.S2CStartGame.Parser, new[]{ "PlayerNum" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::C2SProtoInterface.C2SFrameUpdate), global::C2SProtoInterface.C2SFrameUpdate.Parser, new[]{ "Index", "Start", "End", "Angle", "Input" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::C2SProtoInterface.S2CFrameData), global::C2SProtoInterface.S2CFrameData.Parser, new[]{ "FrameIndex", "Inputs", "InputAngles" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::C2SProtoInterface.S2CFrameUpdate), global::C2SProtoInterface.S2CFrameUpdate.Parser, new[]{ "CurServerFrame", "FrameDatas" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Enums
  public enum EMessage {
    [pbr::OriginalName("None")] None = 0,
    [pbr::OriginalName("Login")] Login = 1,
    [pbr::OriginalName("StartGame")] StartGame = 2,
  }

  #endregion

  #region Messages
  public sealed partial class C2SLogin : pb::IMessage<C2SLogin> {
    private static readonly pb::MessageParser<C2SLogin> _parser = new pb::MessageParser<C2SLogin>(() => new C2SLogin());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<C2SLogin> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::C2SProtoInterface.ProtoInterfaceReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public C2SLogin() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public C2SLogin(C2SLogin other) : this() {
      gId_ = other.gId_;
      name_ = other.name_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public C2SLogin Clone() {
      return new C2SLogin(this);
    }

    /// <summary>Field number for the "gId" field.</summary>
    public const int GIdFieldNumber = 1;
    private int gId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int GId {
      get { return gId_; }
      set {
        gId_ = value;
      }
    }

    /// <summary>Field number for the "name" field.</summary>
    public const int NameFieldNumber = 2;
    private string name_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Name {
      get { return name_; }
      set {
        name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as C2SLogin);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(C2SLogin other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (GId != other.GId) return false;
      if (Name != other.Name) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (GId != 0) hash ^= GId.GetHashCode();
      if (Name.Length != 0) hash ^= Name.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (GId != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(GId);
      }
      if (Name.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Name);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (GId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(GId);
      }
      if (Name.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(C2SLogin other) {
      if (other == null) {
        return;
      }
      if (other.GId != 0) {
        GId = other.GId;
      }
      if (other.Name.Length != 0) {
        Name = other.Name;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            GId = input.ReadInt32();
            break;
          }
          case 18: {
            Name = input.ReadString();
            break;
          }
        }
      }
    }

  }

  public sealed partial class S2CLogin : pb::IMessage<S2CLogin> {
    private static readonly pb::MessageParser<S2CLogin> _parser = new pb::MessageParser<S2CLogin>(() => new S2CLogin());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<S2CLogin> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::C2SProtoInterface.ProtoInterfaceReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S2CLogin() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S2CLogin(S2CLogin other) : this() {
      gId_ = other.gId_;
      udpPot_ = other.udpPot_;
      playerNum_ = other.playerNum_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S2CLogin Clone() {
      return new S2CLogin(this);
    }

    /// <summary>Field number for the "gId" field.</summary>
    public const int GIdFieldNumber = 1;
    private int gId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int GId {
      get { return gId_; }
      set {
        gId_ = value;
      }
    }

    /// <summary>Field number for the "udpPot" field.</summary>
    public const int UdpPotFieldNumber = 2;
    private int udpPot_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int UdpPot {
      get { return udpPot_; }
      set {
        udpPot_ = value;
      }
    }

    /// <summary>Field number for the "playerNum" field.</summary>
    public const int PlayerNumFieldNumber = 3;
    private int playerNum_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int PlayerNum {
      get { return playerNum_; }
      set {
        playerNum_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as S2CLogin);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(S2CLogin other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (GId != other.GId) return false;
      if (UdpPot != other.UdpPot) return false;
      if (PlayerNum != other.PlayerNum) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (GId != 0) hash ^= GId.GetHashCode();
      if (UdpPot != 0) hash ^= UdpPot.GetHashCode();
      if (PlayerNum != 0) hash ^= PlayerNum.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (GId != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(GId);
      }
      if (UdpPot != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(UdpPot);
      }
      if (PlayerNum != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(PlayerNum);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (GId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(GId);
      }
      if (UdpPot != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(UdpPot);
      }
      if (PlayerNum != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(PlayerNum);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(S2CLogin other) {
      if (other == null) {
        return;
      }
      if (other.GId != 0) {
        GId = other.GId;
      }
      if (other.UdpPot != 0) {
        UdpPot = other.UdpPot;
      }
      if (other.PlayerNum != 0) {
        PlayerNum = other.PlayerNum;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            GId = input.ReadInt32();
            break;
          }
          case 16: {
            UdpPot = input.ReadInt32();
            break;
          }
          case 24: {
            PlayerNum = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  public sealed partial class C2SStartGame : pb::IMessage<C2SStartGame> {
    private static readonly pb::MessageParser<C2SStartGame> _parser = new pb::MessageParser<C2SStartGame>(() => new C2SStartGame());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<C2SStartGame> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::C2SProtoInterface.ProtoInterfaceReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public C2SStartGame() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public C2SStartGame(C2SStartGame other) : this() {
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public C2SStartGame Clone() {
      return new C2SStartGame(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as C2SStartGame);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(C2SStartGame other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(C2SStartGame other) {
      if (other == null) {
        return;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
        }
      }
    }

  }

  public sealed partial class S2CStartGame : pb::IMessage<S2CStartGame> {
    private static readonly pb::MessageParser<S2CStartGame> _parser = new pb::MessageParser<S2CStartGame>(() => new S2CStartGame());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<S2CStartGame> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::C2SProtoInterface.ProtoInterfaceReflection.Descriptor.MessageTypes[3]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S2CStartGame() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S2CStartGame(S2CStartGame other) : this() {
      playerNum_ = other.playerNum_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S2CStartGame Clone() {
      return new S2CStartGame(this);
    }

    /// <summary>Field number for the "playerNum" field.</summary>
    public const int PlayerNumFieldNumber = 1;
    private int playerNum_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int PlayerNum {
      get { return playerNum_; }
      set {
        playerNum_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as S2CStartGame);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(S2CStartGame other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (PlayerNum != other.PlayerNum) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (PlayerNum != 0) hash ^= PlayerNum.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (PlayerNum != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(PlayerNum);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (PlayerNum != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(PlayerNum);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(S2CStartGame other) {
      if (other == null) {
        return;
      }
      if (other.PlayerNum != 0) {
        PlayerNum = other.PlayerNum;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            PlayerNum = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  public sealed partial class C2SFrameUpdate : pb::IMessage<C2SFrameUpdate> {
    private static readonly pb::MessageParser<C2SFrameUpdate> _parser = new pb::MessageParser<C2SFrameUpdate>(() => new C2SFrameUpdate());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<C2SFrameUpdate> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::C2SProtoInterface.ProtoInterfaceReflection.Descriptor.MessageTypes[4]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public C2SFrameUpdate() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public C2SFrameUpdate(C2SFrameUpdate other) : this() {
      index_ = other.index_;
      start_ = other.start_;
      end_ = other.end_;
      angle_ = other.angle_;
      input_ = other.input_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public C2SFrameUpdate Clone() {
      return new C2SFrameUpdate(this);
    }

    /// <summary>Field number for the "index" field.</summary>
    public const int IndexFieldNumber = 1;
    private int index_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Index {
      get { return index_; }
      set {
        index_ = value;
      }
    }

    /// <summary>Field number for the "start" field.</summary>
    public const int StartFieldNumber = 2;
    private int start_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Start {
      get { return start_; }
      set {
        start_ = value;
      }
    }

    /// <summary>Field number for the "end" field.</summary>
    public const int EndFieldNumber = 3;
    private int end_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int End {
      get { return end_; }
      set {
        end_ = value;
      }
    }

    /// <summary>Field number for the "angle" field.</summary>
    public const int AngleFieldNumber = 4;
    private long angle_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public long Angle {
      get { return angle_; }
      set {
        angle_ = value;
      }
    }

    /// <summary>Field number for the "input" field.</summary>
    public const int InputFieldNumber = 5;
    private int input_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Input {
      get { return input_; }
      set {
        input_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as C2SFrameUpdate);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(C2SFrameUpdate other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Index != other.Index) return false;
      if (Start != other.Start) return false;
      if (End != other.End) return false;
      if (Angle != other.Angle) return false;
      if (Input != other.Input) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Index != 0) hash ^= Index.GetHashCode();
      if (Start != 0) hash ^= Start.GetHashCode();
      if (End != 0) hash ^= End.GetHashCode();
      if (Angle != 0L) hash ^= Angle.GetHashCode();
      if (Input != 0) hash ^= Input.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Index != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(Index);
      }
      if (Start != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(Start);
      }
      if (End != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(End);
      }
      if (Angle != 0L) {
        output.WriteRawTag(32);
        output.WriteInt64(Angle);
      }
      if (Input != 0) {
        output.WriteRawTag(40);
        output.WriteInt32(Input);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Index != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Index);
      }
      if (Start != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Start);
      }
      if (End != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(End);
      }
      if (Angle != 0L) {
        size += 1 + pb::CodedOutputStream.ComputeInt64Size(Angle);
      }
      if (Input != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Input);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(C2SFrameUpdate other) {
      if (other == null) {
        return;
      }
      if (other.Index != 0) {
        Index = other.Index;
      }
      if (other.Start != 0) {
        Start = other.Start;
      }
      if (other.End != 0) {
        End = other.End;
      }
      if (other.Angle != 0L) {
        Angle = other.Angle;
      }
      if (other.Input != 0) {
        Input = other.Input;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Index = input.ReadInt32();
            break;
          }
          case 16: {
            Start = input.ReadInt32();
            break;
          }
          case 24: {
            End = input.ReadInt32();
            break;
          }
          case 32: {
            Angle = input.ReadInt64();
            break;
          }
          case 40: {
            Input = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  public sealed partial class S2CFrameData : pb::IMessage<S2CFrameData> {
    private static readonly pb::MessageParser<S2CFrameData> _parser = new pb::MessageParser<S2CFrameData>(() => new S2CFrameData());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<S2CFrameData> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::C2SProtoInterface.ProtoInterfaceReflection.Descriptor.MessageTypes[5]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S2CFrameData() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S2CFrameData(S2CFrameData other) : this() {
      frameIndex_ = other.frameIndex_;
      inputs_ = other.inputs_.Clone();
      inputAngles_ = other.inputAngles_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S2CFrameData Clone() {
      return new S2CFrameData(this);
    }

    /// <summary>Field number for the "frameIndex" field.</summary>
    public const int FrameIndexFieldNumber = 1;
    private int frameIndex_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int FrameIndex {
      get { return frameIndex_; }
      set {
        frameIndex_ = value;
      }
    }

    /// <summary>Field number for the "inputs" field.</summary>
    public const int InputsFieldNumber = 2;
    private static readonly pb::FieldCodec<int> _repeated_inputs_codec
        = pb::FieldCodec.ForInt32(18);
    private readonly pbc::RepeatedField<int> inputs_ = new pbc::RepeatedField<int>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<int> Inputs {
      get { return inputs_; }
    }

    /// <summary>Field number for the "inputAngles" field.</summary>
    public const int InputAnglesFieldNumber = 3;
    private static readonly pb::FieldCodec<long> _repeated_inputAngles_codec
        = pb::FieldCodec.ForInt64(26);
    private readonly pbc::RepeatedField<long> inputAngles_ = new pbc::RepeatedField<long>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<long> InputAngles {
      get { return inputAngles_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as S2CFrameData);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(S2CFrameData other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (FrameIndex != other.FrameIndex) return false;
      if(!inputs_.Equals(other.inputs_)) return false;
      if(!inputAngles_.Equals(other.inputAngles_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (FrameIndex != 0) hash ^= FrameIndex.GetHashCode();
      hash ^= inputs_.GetHashCode();
      hash ^= inputAngles_.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (FrameIndex != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(FrameIndex);
      }
      inputs_.WriteTo(output, _repeated_inputs_codec);
      inputAngles_.WriteTo(output, _repeated_inputAngles_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (FrameIndex != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(FrameIndex);
      }
      size += inputs_.CalculateSize(_repeated_inputs_codec);
      size += inputAngles_.CalculateSize(_repeated_inputAngles_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(S2CFrameData other) {
      if (other == null) {
        return;
      }
      if (other.FrameIndex != 0) {
        FrameIndex = other.FrameIndex;
      }
      inputs_.Add(other.inputs_);
      inputAngles_.Add(other.inputAngles_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            FrameIndex = input.ReadInt32();
            break;
          }
          case 18:
          case 16: {
            inputs_.AddEntriesFrom(input, _repeated_inputs_codec);
            break;
          }
          case 26:
          case 24: {
            inputAngles_.AddEntriesFrom(input, _repeated_inputAngles_codec);
            break;
          }
        }
      }
    }

  }

  public sealed partial class S2CFrameUpdate : pb::IMessage<S2CFrameUpdate> {
    private static readonly pb::MessageParser<S2CFrameUpdate> _parser = new pb::MessageParser<S2CFrameUpdate>(() => new S2CFrameUpdate());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<S2CFrameUpdate> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::C2SProtoInterface.ProtoInterfaceReflection.Descriptor.MessageTypes[6]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S2CFrameUpdate() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S2CFrameUpdate(S2CFrameUpdate other) : this() {
      curServerFrame_ = other.curServerFrame_;
      frameDatas_ = other.frameDatas_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S2CFrameUpdate Clone() {
      return new S2CFrameUpdate(this);
    }

    /// <summary>Field number for the "curServerFrame" field.</summary>
    public const int CurServerFrameFieldNumber = 1;
    private int curServerFrame_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CurServerFrame {
      get { return curServerFrame_; }
      set {
        curServerFrame_ = value;
      }
    }

    /// <summary>Field number for the "frameDatas" field.</summary>
    public const int FrameDatasFieldNumber = 2;
    private static readonly pb::FieldCodec<global::C2SProtoInterface.S2CFrameData> _repeated_frameDatas_codec
        = pb::FieldCodec.ForMessage(18, global::C2SProtoInterface.S2CFrameData.Parser);
    private readonly pbc::RepeatedField<global::C2SProtoInterface.S2CFrameData> frameDatas_ = new pbc::RepeatedField<global::C2SProtoInterface.S2CFrameData>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::C2SProtoInterface.S2CFrameData> FrameDatas {
      get { return frameDatas_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as S2CFrameUpdate);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(S2CFrameUpdate other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (CurServerFrame != other.CurServerFrame) return false;
      if(!frameDatas_.Equals(other.frameDatas_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (CurServerFrame != 0) hash ^= CurServerFrame.GetHashCode();
      hash ^= frameDatas_.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (CurServerFrame != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(CurServerFrame);
      }
      frameDatas_.WriteTo(output, _repeated_frameDatas_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (CurServerFrame != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(CurServerFrame);
      }
      size += frameDatas_.CalculateSize(_repeated_frameDatas_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(S2CFrameUpdate other) {
      if (other == null) {
        return;
      }
      if (other.CurServerFrame != 0) {
        CurServerFrame = other.CurServerFrame;
      }
      frameDatas_.Add(other.frameDatas_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            CurServerFrame = input.ReadInt32();
            break;
          }
          case 18: {
            frameDatas_.AddEntriesFrom(input, _repeated_frameDatas_codec);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code

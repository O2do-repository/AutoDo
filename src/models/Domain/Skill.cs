using System.Runtime.Serialization;


public enum Skill
{
    [EnumMember(Value = "Java")]
    Java,

    [EnumMember(Value = "C#")]
    Csharp,

    [EnumMember(Value = "Python")]
    Python,

    [EnumMember(Value = "JavaScript")]
    JavaScript
}
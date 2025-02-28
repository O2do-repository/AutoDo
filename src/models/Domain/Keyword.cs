using System.Runtime.Serialization;

public enum Keyword
{
    [EnumMember(Value = "Architect")]
    Architect,

    [EnumMember(Value = "CyberSecurity")]
    CyberSecurity,

    [EnumMember(Value = "DevOps")]
    DevOps,

    [EnumMember(Value = "DataScience")]
    DataScience
}
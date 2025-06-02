using System.Runtime.Serialization;

public enum StatutMatching
{
    [EnumMember(Value = "Apply")]
    Apply,

    [EnumMember(Value = "New")]
    New,

    [EnumMember(Value = "Not Apply")]
    NotApply,

    [EnumMember(Value = "Rejected")]
    Rejected,
}
    
using System.ComponentModel;

namespace XDFLib
{
    /// <summary> 比较模式 </summary>
    public enum ECompMode
    {
        [Description("<")]
        Less,
        [Description("<=")]
        LessOrEqual,
        [Description("=")]
        Equal,
        [Description(">=")]
        BiggerOrEqual,
        [Description(">")]
        Bigger,
        [Description("!=")]
        NotEqual
    }
}

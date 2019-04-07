using System;

namespace OW.Experts.Domain
{
    [Flags]
    public enum ExpertFetch
    {
        None = 0x0,
        Associations = 0x1,
        Relations = 0x2
    }
}

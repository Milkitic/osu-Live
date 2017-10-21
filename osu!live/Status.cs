using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_live
{
    public enum IdleStatus
    {
        Listening,
        Playing
    }
    public enum ChangeStatus
    {
        ReadyToChange,
        Changing,
        ChangeFinshed
    }
}

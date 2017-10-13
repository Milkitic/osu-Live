using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_live
{
    enum IdleStatus
    {
        Listening,
        Playing
    }
    enum ChangeStatus
    {
        ReadyToChange,
        Changing,
        ChangeFinshed
    }
}

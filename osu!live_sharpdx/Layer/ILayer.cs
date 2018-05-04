using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_live_sharpdx.Layer
{
    public interface ILayer
    {
        void Measure();
        void Draw();
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallpaperManager
{
    public static class ControlHelpers
    {
        public static void InvokeIfRequired(this ISynchronizeInvoke control, Action<ISynchronizeInvoke> action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new Action(() => action(control)), null);
            }
            else
            {
                action(control);
            }
        }
    }
}

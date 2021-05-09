using UnityEngine;
using ToolbarControl_NS;
using KSP_Log;

namespace KspWalkAbout
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        public static Log Log;

        void Start()
        {
            ToolbarControl.RegisterMod(WalkAbout.MODID, WalkAbout.MODNAME);

#if DEBUG
            Log = new Log("WalkAbout", Log.LEVEL.INFO);
#else
            Log = new Log("WalkAbout", Log.LEVEL.ERROR);
#endif
            Log.Info("RegisterToolbar.Start");
        }
    }
}

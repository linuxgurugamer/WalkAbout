using UnityEngine;
using ToolbarControl_NS;

namespace KspWalkAbout
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(WalkAbout.MODID, WalkAbout.MODNAME);
        }
    }
}

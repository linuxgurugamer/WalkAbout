using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;



namespace KspWalkAbout
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings

    //  HighLogic.CurrentGame.Parameters.CustomParams<WA>().

    public class WA : GameParameters.CustomParameterNode
    {
        public override string Title { get { return ""; } } // Column header
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "WalkAbout"; } }
        public override string DisplaySection { get { return "WalkAbout"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return false; } }

        [GameParameters.CustomParameterUI("Utility Mode",
            toolTip ="When enabled, mod will be available in flight mode to add new locations")]
        public bool UtilityMode = false;

        [GameParameters.CustomIntParameterUI("Top Few", minValue = 1, maxValue = 10,
           toolTip = "How many entrances to show for a single location")]
        public int TopFew = 5;

        [GameParameters.CustomIntParameterUI("Max Inventory Items", minValue = 1, maxValue = 10,
           toolTip = "Maximum number of items in inventory when KIS is installed")]
        public int MaxInventoryItems = 5;

        [GameParameters.CustomFloatParameterUI("Max Inventory Volume", displayFormat = "N0", minValue = 50, maxValue = 500, stepCount = 1, asPercentage = false)]
        public float MaxInventoryVolume = 300f;


        [GameParameters.CustomParameterUI("Reload after placement",
            toolTip = "Reload after placement")]
        public bool reload = true;

        [GameParameters.CustomParameterUI("NoReload after placement",
            toolTip = "Do nothing after placement")]
        public bool noreload = false;

        [GameParameters.CustomParameterUI("JumpTo after placement",
            toolTip = "Jump to placed kerbal after placement")]
        public bool jumpto = false;

        //public enum PostPlacementMode { reload, noreload, jumpto };


        bool initted = false;
        bool oldReload, oldNoreload, oldJumpto;
        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            if (initted)
            {
                if (reload && !oldReload)
                {
                    noreload = false;
                    jumpto = false;
                }
                if (noreload && !oldNoreload)
                {
                    reload = false;
                    jumpto = false;
                }
                if (jumpto && !oldJumpto)
                {
                    reload = false;
                    noreload = false;
                }
            }
            else
                initted = true;

            oldReload = reload;
            oldNoreload = noreload;
            oldJumpto = jumpto;

            return true; //otherwise return true
        }

        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            return true;
        }

        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }

    }
}

/*  Copyright 2017 Clive Pottinger
    This file is part of the WalkAbout Mod.

    WalkAbout is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    WalkAbout is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with WalkAbout.  If not, see<http://www.gnu.org/licenses/>.
*/

using KspWalkAbout.Entities;
using KspWalkAbout.Extensions;
using KspWalkAbout.Guis;
using KspWalkAbout.WalkAboutFiles;
using UnityEngine;
using static KspAccess.CommonKspAccess;
using static KspWalkAbout.Entities.WalkAboutPersistent;
using KSP.UI.Screens;
using ToolbarControl_NS;

namespace KspWalkAbout
{
    /// <summary>
    /// Module to allow the user to add new locations for kerbals to be placed.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class WalkAboutAddUtility : MonoBehaviour
    {
        static internal WalkAboutAddUtility fetch;

        private WalkAboutSettings _config;
        private KnownPlaces _map = null;
        internal AddUtilityGui _addUtilityGui = null;

        bool activeEVA = false;
        internal ToolbarControl toolbarControl = null;

        /// <summary>
        /// Called when the game is loaded. Used to set up all persistent objects and properties.
        /// </summary>
        public void Start()
        {
            fetch = this;
            "WalkAboutAddUtility.Start".Debug();
            activeEVA = false;
            if (!(FlightGlobals.ActiveVessel?.isEVA ?? false))
            {
                "Add Location utility deactivated: not an EVA".Debug();
            }
            else
            {
                var crew = FlightGlobals.ActiveVessel.GetVesselCrew();
                if ((crew?.Count ?? 0) != 1)
                {
                    "Add Location utility deactivated: invalid crew count".Debug();
                } else
                    activeEVA = true;
            }
            GameEvents.onVesselChange.Add(onVesselChange);

            _config = GetModConfig(); "Add Location utility obtained config".Debug();
            if (_config == null) { return; }
            if (!HighLogic.CurrentGame.Parameters.CustomParams<WA>().UtilityMode)
            {
                "Add Location utility deactivated: not in utility mode".Debug();
                return;
            }
            if (activeEVA)
                $"Add Location utility activated on EVA for {FlightGlobals.ActiveVessel.GetVesselCrew()[0].name}".Debug();

            _map = GetLocationMap(); "Add Location utility obtained map object".Debug();
            _addUtilityGui = new AddUtilityGui();


            if (toolbarControl == null)
            {
                toolbarControl = gameObject.AddComponent<ToolbarControl>();
                toolbarControl.AddToAllToolbars(GuiOn, GuiOff,
                    ApplicationLauncher.AppScenes.FLIGHT,
                    WalkAbout.MODID,
                    "walkaboutButton_Flight",
                    "WalkAbout/PluginData/WalkAbout-38",
                    "WalkAbout/PluginData/WalkAbout-24",
                    WalkAbout.MODNAME
                );
            }
        }

        void onVesselChange(Vessel to)
        {
            if (to != null)
                Debug.Log("onVesselChange: " + to.vesselName);

            activeEVA = false;
            if (toolbarControl != null)
                toolbarControl.Enabled = false;
            if (_addUtilityGui != null)
            _addUtilityGui.IsActive = false;

            if (!(FlightGlobals.ActiveVessel?.isEVA ?? false))
            {
                "Add Location utility deactivated: not an EVA".Debug();
                return;
            }
            var crew = FlightGlobals.ActiveVessel.GetVesselCrew();
            if ((crew?.Count ?? 0) != 1)
            {
                "Add Location utility deactivated: invalid crew count".Debug();
                return;
            }
            toolbarControl.Enabled = true;
            activeEVA = true;
        }

        void GuiOn()
        {
            _addUtilityGui.IsActive = true;
            _addUtilityGui.GuiCoordinates = GuiResizer.HandleResizing(_addUtilityGui.GuiCoordinates);
            SaveFiles();
        }
        internal void GuiOff()
        {
            _addUtilityGui.IsActive = false;
        }

        /// <summary>
        /// Called each time the game state is updated.
        /// </summary>
        public void LateUpdate()
        {
#if false
            if (_addUtilityGui == null)
            {
                return;
            }

            if (CheckForModUtilityActivation())
            {
                _addUtilityGui.GuiCoordinates = GuiResizer.HandleResizing(_addUtilityGui.GuiCoordinates);
                SaveFiles();
            }
#endif
            if (activeEVA && !Input.GetMouseButton(0))
            {
                SaveFiles();
            }
        }

        /// <summary>
        /// Called each time the game's GUIs are to be refreshed.
        /// </summary>
        public void OnGUI()
        {
            if (!activeEVA)
                return;
            _addUtilityGui?.Display();

            if (_addUtilityGui?.RequestedLocation == null) return;

            $"Request for new location {_addUtilityGui.RequestedLocation.Name} detected".Debug();
            _map.AddLocation(_addUtilityGui.RequestedLocation);
            _addUtilityGui.RequestedLocation = null;
        }

#if false
        /// <summary>
        /// Determines if the user has requested the WalkAbout mod's utility GUI.
        /// </summary>
        private bool CheckForModUtilityActivation()
        {
            var wasActive = _addUtilityGui.IsActive;
            _addUtilityGui.IsActive |= IsKeyCombinationPressed(_config.AUActivationHotKey, _config.AUActivationHotKeyModifiers);

            if (wasActive != _addUtilityGui.IsActive)
            {
                _map.RefreshLocations();
            }

            return _addUtilityGui.IsActive;
        }
#endif

        /// <summary>
        /// Saves all settings files with pending changes.
        /// </summary>
        private void SaveFiles()
        {

            if (_map != null && _map.IsChanged)
            {
                "Saving map changes".Debug();
                _map.Save();
            }
        }

        void OnDestroy()
        {
            GameEvents.onVesselChange.Remove(onVesselChange);
            if (toolbarControl != null)
            {
                toolbarControl.OnDestroy();
                Destroy(toolbarControl);
                toolbarControl = null;
            }
        }
    }
}
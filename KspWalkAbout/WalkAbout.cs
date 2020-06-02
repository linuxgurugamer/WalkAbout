﻿/*  Copyright 2017 Clive Pottinger
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

using KspAccess;
using KspWalkAbout.Entities;
using KspWalkAbout.Extensions;
using KspWalkAbout.Guis;
using KspWalkAbout.Values;
using KspWalkAbout.WalkAboutFiles;
using UnityEngine;
using Upgradeables;
using static KspAccess.CommonKspAccess;
using static KspWalkAbout.Entities.WalkAboutPersistent;
using KSP.UI.Screens;
using ToolbarControl_NS;
using ClickThroughFix;

namespace KspWalkAbout
{
    /// <summary>
    /// Module to allow user to place kerbals at specific locations.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class WalkAbout : MonoBehaviour
    {
        static internal WalkAbout fetch;
        private WalkAboutSettings _config;
        private KnownPlaces _map;
        private InventoryItems _items;
        private static PlaceKerbalGui _mainGui;

        internal const string MODID = "walkabout_NS";
        internal const string MODNAME = "WalkAbout";

        internal ToolbarControl toolbarControl = null;

        /// <summary>
        /// Called when the game is loaded. Used to set up all persistent objects and properties.
        /// </summary>
        public void Start()
        {
            $"Started [Version={Constants.Version}, Debug={DebugExtensions.DebugIsOn}]".Log();
            fetch = this;
            _config = GetModConfig(); "obtained config".Debug();
            if (_config == null)
            {
                return;
            }

            _map = GetLocationMap(); "obtained map object".Debug();
            _map.RefreshLocations(); // needed to avoid holding on to other games' data 
            _items = GetAllItems(); "obtained items object".Debug();

            _mainGui = PlaceKerbalGui.Instance;
            _mainGui.GuiCoordinates = _config.GetScreenPosition();
            //_mainGui.TopFew = _config.TopFew;
            _mainGui.MaxItems = HighLogic.CurrentGame.Parameters.CustomParams<WA>().MaxInventoryItems;
            _mainGui.MaxVolume = HighLogic.CurrentGame.Parameters.CustomParams<WA>().MaxInventoryVolume;
            $"created MainGui object".Debug();

            GetCentrum(); // Initialize the centrum to avoid errors if AddUtility is opened before WalkAbout.

            GameEvents.OnTechnologyResearched.Remove(ItemRefresh);
            GameEvents.OnKSCFacilityUpgraded.Remove(MapRefresh);
            GameEvents.OnTechnologyResearched.Add(ItemRefresh);
            GameEvents.OnKSCFacilityUpgraded.Add(MapRefresh);

            if (toolbarControl == null)
            {
                toolbarControl = gameObject.AddComponent<ToolbarControl>();
                toolbarControl.AddToAllToolbars(GuiOn, GuiOff,
                    ApplicationLauncher.AppScenes.SPACECENTER,
                    MODID,
                    "walkaboutButton_spacecenter",
                    "WalkAbout/PluginData/WalkAbout-38",
                    "WalkAbout/PluginData/WalkAbout-24",
                    MODNAME
                );
            }
        }

        void GuiOn()
        {
            _mainGui.IsActive = true;
            //_mainGui.GuiCoordinates = GuiResizer.HandleResizing(_mainGui.GuiCoordinates);
            SaveFiles();
        }
        internal void GuiOff()
        {
            _mainGui.IsActive = false;
        }
#if false
        /// <summary>
        /// Called each time the game state is updated.
        /// </summary>
        public void Update()
        {
            if (_mainGui == null)
            {
                return;
            }

            if (CheckForModActivation())
            {
                _mainGui.GuiCoordinates = GuiResizer.HandleResizing(_mainGui.GuiCoordinates);
                SaveFiles();
            }
        }
#endif

        /// <summary>
        /// Called each time the game's GUIs are to be refreshed.
        /// </summary>
        public void OnGUI()
        {
            if (_mainGui?.Display() ?? false)
            {
                _config.SetScreenPosition(_mainGui.GuiCoordinates);
            }

            if (_mainGui?.RequestedPlacement == null) return;

            "placing kerbal".Debug();
            WalkAboutKspAccess.PlaceKerbal(_mainGui.RequestedPlacement);
            "Saving game".Log();
            GamePersistence.SaveGame("persistent", HighLogic.SaveFolder, SaveMode.OVERWRITE);

            PerformPostPlacementAction();

            _items.UpdateQueueing(_mainGui.RequestedPlacement.Items);
            _map.UpdateQueuing(_mainGui.RequestedPlacement.Location.LocationName);
            _mainGui.RequestedPlacement = null;
        }

        /// <summary>
        /// Obtains the directory where the WalkAbout mod is currently installed.
        /// </summary>
        /// <returns>Returns a directoy path.</returns>
        internal static string GetModDirectory()
        {
            return CommonKspAccess.GetModDirectory(Constants.ModName);
        }

        /// <summary>
        /// Finds a vessel in the game.
        /// </summary>
        /// <param name="name">The name of vessel.</param>
        private static Vessel FindVesselByName(string name)
        {
            Vessel found = null;
            var searchName = $"{name} (unloaded)";

            foreach (var vessel in FlightGlobals.Vessels)
            {
                if ((vessel.name == name) || (vessel.name == searchName))
                {
                    found = vessel;
                    break;
                }
            }
            //found = FlightGlobals.Vessels.Find(v => (v.name == name) || (v.name == searchName));

            return found;
        }

        /// <summary>
        /// Reloads the collection of items that can used by kerbals.
        /// </summary>
        private void ItemRefresh(GameEvents.HostTargetAction<RDTech, RDTech.OperationResult> data)
        {
            if (data.target == RDTech.OperationResult.Successful)
            {
                $"Refreshing items due to new technologies".Debug();
                _items.RefreshItems();
            }
        }

        /// <summary>
        /// Reloads the collection of places where kerbals can be placed.
        /// </summary>
        private void MapRefresh(UpgradeableFacility data0, int data1)
        {
            $"Refreshing map due to new facility upgrade".Debug();
            _map.RefreshLocations();
        }

#if false
        /// <summary>
        /// Determines if the user has requested the WalkAbout mod's GUI.
        /// </summary>
        private bool CheckForModActivation()
        {
            _mainGui.IsActive |= IsKeyCombinationPressed(_config.ActivationHotKey, _config.ActivationHotKeyModifiers);
            return _mainGui.IsActive;
        }
#endif
        /// <summary>
        /// Saves all settings files with pending changes.
        /// </summary>
        private void SaveFiles()
        {
            if (_config.IsChanged && !GuiResizer.IsResizing && !Input.GetMouseButton(0))
            {
                _config.Save();
                $"saved settings to {_config.FilePath}".Log();
            }

            if (_map.IsChanged)
            {
                _map.Save();
            }

            if (_items.IsChanged)
            {
                _items.Save();
            }
        }

        private void PerformPostPlacementAction()
        {
            if (HighLogic.CurrentGame.Parameters.CustomParams<WA>().noreload)
            {
                "Suppressed reload of SPACECENTER".Log();
            }
            if (HighLogic.CurrentGame.Parameters.CustomParams<WA>().jumpto)
            {
                var vessel = FindVesselByName(_mainGui.RequestedPlacement.Kerbal.name);
                if (vessel == null)
                {
                    $"Unable to jump to vessel - no vessel found".Log();
                }
                else
                {
                    $"Loading Flight scene for {vessel.name}".Log();
                    FlightDriver.StartAndFocusVessel("persistent", FlightGlobals.Vessels.IndexOf(vessel));
                }
            }
            if (HighLogic.CurrentGame.Parameters.CustomParams<WA>().reload ||
            (!HighLogic.CurrentGame.Parameters.CustomParams<WA>().jumpto && !HighLogic.CurrentGame.Parameters.CustomParams<WA>().noreload))
            {
                "Reloading SPACECENTER".Log();
                HighLogic.LoadScene(GameScenes.SPACECENTER);
            }
        }

        void OnDestroy()
        {
            toolbarControl.OnDestroy();
            Destroy(toolbarControl);
            toolbarControl = null;
        }

    }
}
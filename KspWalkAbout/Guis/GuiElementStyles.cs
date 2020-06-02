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

using UnityEngine;

namespace KspWalkAbout.Guis
{
    /// <summary>Represents usable styles for windows drawn with Unity's GUILayout functions.</summary>
    internal class GuiElementStyles
    {
        public GUIStyle ValidButton { get; private set; }
        public GUIStyle SelectedButton { get; private set; }
        public GUIStyle InvalidButton { get; private set; }
        public GUIStyle HighlightedButton { get; private set; }
        public GUIStyle ValidLabel { get; private set; }
        public GUIStyle InvalidLabel { get; private set; }
        public GUIStyle ValidTextInput { get; private set; }
        public GUIStyle InvalidTextInput { get; private set; }

        // Following makes sure that all the above properties get allocated at instantiation time
        public GuiElementStyles()
        {
            Debug.Log("GuiElementStyles Instantiator");
            ValidButton = new GUIStyle(GUI.skin.button);
            SelectedButton = new GUIStyle(GUI.skin.button) { normal = { textColor = Color.green } };
            InvalidButton = new GUIStyle(GUI.skin.button) { normal = { textColor = Color.yellow } };
            HighlightedButton = new GUIStyle(GUI.skin.button) { normal = { textColor = Color.green, background = Texture2D.whiteTexture } };
            ValidLabel = new GUIStyle(GUI.skin.label);
            InvalidLabel = new GUIStyle(GUI.skin.label) { normal = { textColor = Color.yellow } };
            ValidTextInput = new GUIStyle(GUI.skin.label) { normal = { textColor = Color.black, background = Texture2D.whiteTexture } };
            InvalidTextInput = new GUIStyle(GUI.skin.label) { normal = { textColor = Color.blue, background = Texture2D.whiteTexture } };
        }
    }
}
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

using System;
using KspWalkAbout.Values;
using UnityEngine;
using static KspWalkAbout.RegisterToolbar;

namespace KspWalkAbout.Extensions
{
    /// <summary>Presents utility methods and members for use in logging.</summary>
    internal static class DebugExtensions
    {
        /// <summary>Indicates whether <see cref="Debug(string)"/> operations should write to the log.</summary>
        public static bool DebugIsOn = true; // false;

        /// <summary>Creates an initializes a new instance of the DebugExtensions class.</summary>
        static DebugExtensions()
        {
            DebugIsOn = System.IO.File.Exists($"{WalkAbout.GetModDirectory()}/debug.flg");
        }

        /// <summary>Writes the message to the log.</summary>
        /// <param name="message">Text to be written.</param>
        public static void Log(this string message)
        {
            MonoBehaviour.print($"{Constants.ModName}: {message}");
            RegisterToolbar.Log.Info(message);
        }

        /// <summary>Write the message to the log if the DebugOn flag is set.</summary>
        /// <param name="message">Text to be written.</param>
        /// <param name="at">An additional identifier added to the prefix of the message (default is calling method name).</param>
        public static void Debug(
            this string message,
            [System.Runtime.CompilerServices.CallerMemberName] string at = "")
        {
            UnityEngine.Debug.Log($"{Constants.ModName} ({at}): {message}");
            if (DebugIsOn) MonoBehaviour.print($"{Constants.ModName} ({at}): {message}");
        }

        /// <summary>
        /// Changes whether future <see cref="Debug(string)"/> operations should write to the log.
        /// </summary>
        /// <param name="debugState">Whether or not debug logging should be active.</param>
        public static void SetDebug(bool debugState = true)
        {
            DebugIsOn = debugState;
        }
    }
}

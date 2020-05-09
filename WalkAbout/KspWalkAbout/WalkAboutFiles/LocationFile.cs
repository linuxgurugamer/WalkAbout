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

using KspWalkAbout.Entities;
using KspWalkAbout.KspFiles;
using System.Collections.Generic;
using System.IO;

namespace KspWalkAbout.WalkAboutFiles
{
    /// <summary>Represents a settings file containing a set of locations for use with the mod.</summary>
    public class LocationFile : SettingsFile
    {
        /// <summary>Collection of all locations where kerbals may be placed.</summary>
        [Persistent]
        public List<Location> Locations;

        public object Filename { get { return Path.GetFileName(this.FilePath); } }
    }
}
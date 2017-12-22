/* 
 * This file is part of the Tustan AR distribution 
 * (https://github.com/softserveinc-rnd/tustan-ar).
 * Copyright (c) 2017 Softserve Inc.
 *
 * Tustan 3D model Copyright 2017 Vasyl Rozhko
 * 3D model of the 5th period of the cliffside city-fortress Tustan's log 
 * constructions was created on the basis of the architectural analysis 
 * of graphic reconstructions by Mykhailo Rozhko (1939-2004), an architect,  
 * archeologist and researcher of Tustan. 
 * 3D model's creators: Vasyl Rozhko, Maksym Yasinskyi, Vasyl Dmytruk, 
 * Oleh Rybchynskyi and Andriy Dedyshyn.
 * 
 * This file is part of Tustan AR.
 *
 * Tustan AR is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Tustan AR is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Tustan AR. If not, see <http://www.gnu.org/licenses/>.
 */

using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using UnityEngine;
using System;
using System.IO;
 
public class MyPluginPostProcessBuild
{
    [PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject)
    {
        #if UNITY_IOS
        if ( buildTarget == BuildTarget.iOS )
        {
            // UPDATE INFO.PLIST
            // Get plist
            string plistPath = pathToBuiltProject + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));
           
            // Get root
            PlistElementDict rootDict = plist.root;
            // bundle identifier
            rootDict.SetString("CFBundleIdentifier", "com.softserve.tustanarapp");
           
            // background location
            rootDict.SetString("NSLocationWhenInUseUsageDescription", "Uses location for positioning user on map.");
            rootDict.SetString("NSLocationAlwaysUsageDescription", "Uses background location for sending push notifications when user goes into certain marker area. Choosing this may dramatically decrease battery life.");
			rootDict.SetString("NSLocationAlwaysAndWhenInUseUsageDescription", "Uses location for positioning user on map. Uses background location for sending push notifications when user goes into certain marker area. Choosing this may dramatically decrease battery life.");
            
            // photo saving
            rootDict.SetString("NSPhotoLibraryAddUsageDescription", "Please allow to save photos to library.");
            rootDict.SetString("NSPhotoLibraryUsageDescription", "Please allow to save photos to library.");
            // run only if GPS is accessable
            PlistElementArray requiredModes = rootDict.CreateArray("UIRequiredDeviceCapabilities");
            requiredModes.AddString("location-services");
            requiredModes.AddString("gps");
           
            // background modes
            PlistElementArray bgModes = rootDict.CreateArray("UIBackgroundModes");
            bgModes.AddString("location");
           
            // Write to file
            File.WriteAllText(plistPath, plist.WriteToString());

            // UPDATE FRAMEWORKS
            string projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
            PBXProject project = new PBXProject();
            project.ReadFromString(File.ReadAllText(projectPath));
            string targetName = PBXProject.GetUnityTargetName();
			string projectTarget = project.TargetGuidByName(targetName);
            project.AddFrameworkToProject(projectTarget, "Photos.framework", false);
            File.WriteAllText(projectPath, project.WriteToString());

            // COPY IMAGES TO IOS PROJECT
            string from_path = "Assets/Plugins/iOS/images/";
            string to_path = pathToBuiltProject + "/Data/images/";
            Directory.CreateDirectory(to_path);
            string[] files = Directory.GetFiles(from_path);
            foreach(string filepath in files){
                string ext = Path.GetExtension(filepath).ToLower();
                if(ext == ".jpg" || ext == ".png")
                    File.Copy(filepath, to_path + Path.GetFileName(filepath));
            }
        }
        #endif
    }
}
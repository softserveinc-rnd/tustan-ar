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

using System;
using UnityEngine;
using System.Linq;

namespace TustanAR.Native{
	using Positioning;
	using UI;
	using Locale;

	public interface INativeAdapter{
		void InitAndStartUpdates(Marker[] markers);
		void GoToGPSSettings();
		void ShareFile(string shareIntentMessage, string shareText, string filePath, string fileType);
		void MakeScreenshot(string filename, int bottomMargin, int topMargin, DeviceOrientation orientation);
		float GetFontScale();
		void ShowProgressDialog(String text);
		void HideProgressDialog();
		void LaunchIntroduction();
		void LocalizePlugins(String jsonString);
	}
	
	public class NativeAdapter: INativeAdapter {

		protected NativeAdapter(){}
		protected static NativeAdapter instance = new NativeAdapter();
		public static INativeAdapter Instance { get { return instance; } }

		#if UNITY_ANDROID
		protected const string ANDROID_PLUGIN_PACKAGE_NAME = "com.softserve.tustanar.plugin";
		protected const string ANDROID_CONTROLS_PLUGIN_PACKAGE_NAME = "com.softserve.plugin.controls";
		protected const string ANDROID_INSTRUCTIONS_PLUGIN_PACKAGE_NAME = "com.softserve.plugin.instructions";
		protected const string ANDROID_LOCALE_PLUGIN_PACKAGE_NAME = "com.softserve.plugin.locale";
		#endif

		public void InitAndStartUpdates(Marker[] markers){
			Location[] markerLocations = markers.Select(marker => marker.location).ToArray();
			string[] markerTitles = markers.Select(marker => marker.title).ToArray();
			string[] markerCoordinates = new string[markerLocations.Length];
			string coordsSeparatorString = ",";
			for(int i=0; i<markerLocations.Length; i++){
					markerCoordinates[i] = markerLocations[i].latitude + coordsSeparatorString + markerLocations[i].longitude;
				}
			string separatorString = "<separator />";
			string markersString = string.Join(separatorString, markerCoordinates);
			string markerTitlesString = string.Join(separatorString, markerTitles);
			string localizationString = LocalizationManager.Instance.GetLocalizedValue("crossplatform_notification_title") + separatorString + LocalizationManager.Instance.GetLocalizedValue("crossplatform_notification_message");

			#if UNITY_IOS 
				Background.StartTustanService(markersString, markerTitlesString, localizationString, separatorString, coordsSeparatorString, Config.COOLDOWN_INTERVAL, Config.COOLDOWN_DISTANCE, Config.NOTIFICATION_DISTANCE, Config.ACCEPTABLE_ACCURACY);
			#elif UNITY_ANDROID
				try{
					using(AndroidJavaClass pluginClass = new AndroidJavaClass(ANDROID_PLUGIN_PACKAGE_NAME+".Launcher")){
						AndroidJavaObject launcher = pluginClass.CallStatic<AndroidJavaObject>("instance");
						launcher.Call("startTustanService", markersString, markerTitlesString, localizationString, separatorString, coordsSeparatorString, Config.COOLDOWN_INTERVAL, Config.COOLDOWN_DISTANCE, Config.NOTIFICATION_DISTANCE, Config.ACCEPTABLE_ACCURACY);
					}
				}
				catch(Exception){}
			#endif
		}

		public void GoToGPSSettings(){
			#if UNITY_ANDROID
			try {
				using(AndroidJavaClass handler = new AndroidJavaClass(ANDROID_CONTROLS_PLUGIN_PACKAGE_NAME+".ControlsHandler")){
					handler.CallStatic("GoToGPSSettings");
				}
			}
			catch(Exception){}
			#elif UNITY_IOS
				Background.GoToGPSSettings();
			#endif
		}
		
		public void ShareFile(string shareIntentMessage, string shareText, string filePath, string fileType){
			#if UNITY_ANDROID
			try {
				using(AndroidJavaClass handler = new AndroidJavaClass(ANDROID_CONTROLS_PLUGIN_PACKAGE_NAME+".ControlsHandler")){
					handler.CallStatic("ShareFile", shareIntentMessage, shareText, filePath, fileType);
				}
			}
			catch(Exception){}
            #elif UNITY_IOS
            	
            #endif

        }

        public void MakeScreenshot(string filename, int bottomMargin, int topMargin, DeviceOrientation orientation){
			ScreenshotManager.SCREENSHOT_SHARED = true;
			#if UNITY_ANDROID
			try {
				using(AndroidJavaClass handler = new AndroidJavaClass(ANDROID_CONTROLS_PLUGIN_PACKAGE_NAME+".ControlsHandler")){
					Location l = Location.GetCurrentLocation();
					handler.CallStatic("MakeScreenshot", filename, bottomMargin, topMargin, l.latitude, l.longitude, l.altitude, (int)orientation);
				}
			}
			catch(Exception){}
			#elif UNITY_IOS
				Background.SaveScreenshot(filename, bottomMargin, topMargin, (int)orientation);
			#endif
		}

		public float GetFontScale(){
			#if UNITY_ANDROID
			try {
				using(AndroidJavaClass handler = new AndroidJavaClass(ANDROID_CONTROLS_PLUGIN_PACKAGE_NAME+".Accessibilty")){
					return handler.CallStatic<float>("GetFontScale");
				}
			}
			catch(Exception){
				return 1;
			}
			#elif UNITY_IOS
				return Background.GetFontScale();
			#else
				return 1;
			#endif
		}

		public void ShowProgressDialog(String text){
			#if UNITY_ANDROID
			try {
				using(AndroidJavaClass handler = new AndroidJavaClass(ANDROID_CONTROLS_PLUGIN_PACKAGE_NAME+".ControlsHandler")){
					handler.CallStatic("ShowProgressDialog", text);
				}
			}
			catch(Exception){}
			#elif UNITY_IOS
				Background.ShowProgressDialog(text);
			#endif
		}

		public void HideProgressDialog(){
			#if UNITY_ANDROID
			try {
				using(AndroidJavaClass handler = new AndroidJavaClass(ANDROID_CONTROLS_PLUGIN_PACKAGE_NAME+".ControlsHandler")){
					handler.CallStatic("HideProgressDialog");
				}
			}
			catch(Exception){}
			#elif UNITY_IOS
				Background.HideProgressDialog();
			#endif
		}

		public void LaunchIntroduction(){
			#if UNITY_ANDROID
			try{
				using(AndroidJavaClass handler = new AndroidJavaClass("com.softserve.plugin.activity.TustanActivity")){
						handler.CallStatic("LaunchIntroduction");
				}
			}
			catch(Exception){}
			#elif UNITY_IOS
				Background.LaunchIntroduction();
			#endif
		}

		public void LocalizePlugins(String jsonString){
			#if UNITY_ANDROID
			try{
				using(AndroidJavaClass handler = new AndroidJavaClass(ANDROID_LOCALE_PLUGIN_PACKAGE_NAME+".LocalizationManager")){
					handler.CallStatic("LoadLocalizedText", jsonString);
				}
			}
			catch(Exception){}
			#elif UNITY_IOS
				Background.LoadLocalizedText(jsonString);
			#endif
		}
	}
}
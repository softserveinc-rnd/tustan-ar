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

using UnityEngine;

public class Background : MonoBehaviour {

	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern void _StartTustanService(string coords, string markerTitlesString, string notification_messages, string separator, string coordsSeparator, int cooldownInterval, float cooldownDistance, float notificationDistance, float acceptableAccuracy);
	public static void StartTustanService(string coordinatesString, string markerTitlesString, string localizationString, string separatorString, string coordsSeparatorString, int cooldownInterval, float cooldownDistance, float notificationDistance, float acceptableAccuracy){
		if(Application.platform == RuntimePlatform.IPhonePlayer){
			_StartTustanService(coordinatesString, markerTitlesString, localizationString, separatorString, coordsSeparatorString, cooldownInterval, cooldownDistance, notificationDistance, acceptableAccuracy);
		}
	}

	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern void _SaveScreenshot(string filename, float bottom_margin, float top_margin, int orientation);
	public static void SaveScreenshot(string filename, float bottom_margin, float top_margin, int orientation){
		if(Application.platform == RuntimePlatform.IPhonePlayer){
			_SaveScreenshot(filename, bottom_margin, top_margin, orientation);
		}
	}

	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern void _ShowProgressDialog(string message);
	public static void ShowProgressDialog(string message){
		if(Application.platform == RuntimePlatform.IPhonePlayer){
			_ShowProgressDialog(message);
		}
	}

	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern void _HideProgressDialog();
	public static void HideProgressDialog(){
		if(Application.platform == RuntimePlatform.IPhonePlayer){
			_HideProgressDialog();
		}
	}

	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern float _GetFontScale();
	public static float GetFontScale(){
		if(Application.platform == RuntimePlatform.IPhonePlayer){
			return _GetFontScale();
		}
		else{ 
			return 1;
		}
	}

	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern void _GoToGPSSettings();
	public static void GoToGPSSettings(){
		if(Application.platform == RuntimePlatform.IPhonePlayer){
			_GoToGPSSettings();
		}
	}

	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern void _LaunchIntroduction();
	public static void LaunchIntroduction(){
		if(Application.platform == RuntimePlatform.IPhonePlayer){
			_LaunchIntroduction();
		}
	}

	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern void _LoadLocalizedText(string jsonString);
	public static void LoadLocalizedText(string jsonString){
		if(Application.platform == RuntimePlatform.IPhonePlayer){
			_LoadLocalizedText(jsonString);
		}
	}
}

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
using Vuforia;
using System;
using System.Collections;

namespace TustanAR{
	using UI;
	using UI.Windows;
	using Positioning;
	using Locale;
	using Native;
	
	///
	/// App UI entry point.
	///
	public sealed class Init : MonoBehaviour {

		[Header("General")]
		public GameObject toastPrefab;
		[Space(5)]

		[Header("Map Window")]
		public Sprite map;
		public Sprite outOfMapIcon;

		// Runtime vars
		private Location lastLocation;

		void Awake () {
			Input.location.Start();
			
			Runtime.canvas = gameObject;
			Runtime.toastPrefab = toastPrefab;

			CameraManager.Instance.Init();
			InfoManager.Instance.Init();
		}

		IEnumerator Start () {

            yield return LocalizationManager.Instance.Load(Application.systemLanguage);

			NativeAdapter.Instance.InitAndStartUpdates(GetComponent<Config>().Markers);
			MapManager.Instance.Init(map, outOfMapIcon, GetComponent<Config>().Markers);

			VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
			VuforiaARController.Instance.RegisterOnPauseCallback(OnPaused);

			try {
				MapManager.Instance.UpdateLocation(Input.location.lastData);
			}
			catch(Exception ex){
				UnityEngine.Debug.LogWarning("@trom: If you see this message, most likely you are running project on desktop where location services are missing or prohibited for Unity. Don't worry, all is okay :). If not, try to log true error in " + ex.Source);
			}
			lastLocation = new Location();
		}

		void Update () {
			// user tapped back button
			if (Input.GetKeyDown(KeyCode.Escape)) 
				History.GoBack();

			try{
				Location currentLocation = Location.GetCurrentLocation();
				if(currentLocation != lastLocation){
					MapManager.Instance.UpdateLocation(Input.location.lastData);
					lastLocation = currentLocation;
				}
			}
			catch(Exception){}
		}

		void OnPaused(bool paused)
		{
			if (!paused) // resumed
			{
				this.SetupFocusAndExposure();
			}
		}

		private void OnVuforiaStarted()
		{
			NativeAdapter.Instance.HideProgressDialog();
			CameraManager.Instance.ShowCameraWindow();
			this.SetupFocusAndExposure();
			NativeAdapter.Instance.LaunchIntroduction();
		}

		private void SetupFocusAndExposure(){
			if(!CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO)){
				CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_NORMAL);
			}
		}
	}
}

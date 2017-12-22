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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

namespace TustanAR.VuforiaExtensions{
	using UI;

	///
	/// Class that extends Vuforia behaviour, allows reset Tracking when Extended Tracking is enabled
	///
	public sealed class ExtendedTrackingBehaviour : MonoBehaviour {

		/* EXTERNAL METHODS */
		/// Resets both Tracking and Extended Tracking.
		public static void ResetExtendedTracking(){
			TrackerManager.Instance.GetTracker<ObjectTracker>().Stop();
			TrackerManager.Instance.GetTracker<ObjectTracker>().ResetExtendedTracking();
			TrackerManager.Instance.GetTracker<ObjectTracker>().Start();
		}

		/* UNITY LIFECYCLE METHODS */
		void Start () {
			VuforiaTargets.OnVuforiaExtendedTrackingStarted += OnVuforiaExtendedTrackingStarted;
			VuforiaTargets.OnVuforiaExtendedTrackingFinished += OnVuforiaExtendedTrackingFinished;
		}

		/* PRIVATE VARIABLES */
		private IEnumerator runningCoroutine;

		/* PRIVATE METHODS */
		/// Shows button that resets Extended Tracking if it is showing for a long time.
		private void OnVuforiaExtendedTrackingStarted(GameObject root, int pointNumber){
			runningCoroutine = showResetButtonAfter(seconds:7.0f); // running coroutine should be stored in variable in order to be stopped
			StartCoroutine(runningCoroutine);
		}

		/// Hides the button that resets Extended Tracking.
		private void OnVuforiaExtendedTrackingFinished(GameObject root, int pointNumber){
			if(Runtime.resetExtendedTrackingButton != null)
				Runtime.resetExtendedTrackingButton.SetActive(false);

			if(runningCoroutine != null)
				StopCoroutine(runningCoroutine);
		}

		private IEnumerator showResetButtonAfter(float seconds){
			yield return new WaitForSeconds(seconds);
			if(Runtime.resetExtendedTrackingButton != null)
				Runtime.resetExtendedTrackingButton.SetActive(true);
		}
	}
}

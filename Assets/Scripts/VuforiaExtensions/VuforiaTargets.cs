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
using Vuforia;

namespace TustanAR.VuforiaExtensions {

	///
	/// Tracks all changes with Vuforia targets on scene.
	///
	public class VuforiaTargets : MonoBehaviour {

		public delegate void VuforiaTargetEvent(GameObject root, int pointNumber);

		public static event VuforiaTargetEvent OnVuforiaImageTargetFound;
		public static event VuforiaTargetEvent OnVuforiaImageTargetLost;
		
		public static event VuforiaTargetEvent OnVuforiaExtendedTrackingStarted;
		public static event VuforiaTargetEvent OnVuforiaExtendedTrackingFinished;

		private TrackableBehaviour.Status lastTrackableStatus = TrackableBehaviour.Status.UNKNOWN;
        private GameObject trackableFound;
		private static readonly int trackableDefaultValue = int.MinValue;
		private int trackableFoundNumber = trackableDefaultValue; 
		
		void Update () {

			StateManager vuforiaStateManager = TrackerManager.Instance.GetStateManager();
			TrackableBehaviour trackable = default(TrackableBehaviour);
			int trackablesCount = 0;
			foreach (TrackableBehaviour t in vuforiaStateManager.GetActiveTrackableBehaviours())
			{
				trackablesCount++;
				trackable = t;
			}

			UpdateExtendedTracking(trackable, trackablesCount);
			UpdateTarget(trackable, trackablesCount);
		}

		/// Extended tracking started / finished.
		private void UpdateExtendedTracking(TrackableBehaviour trackable, int trackablesCount){
			TrackableBehaviour.Status newTrackableStatus;
			if(trackablesCount > 0){
				newTrackableStatus = trackable.CurrentStatus;
			}
			else{
				newTrackableStatus = TrackableBehaviour.Status.UNDEFINED;
			}

			if(newTrackableStatus == TrackableBehaviour.Status.EXTENDED_TRACKED && lastTrackableStatus != TrackableBehaviour.Status.EXTENDED_TRACKED)
				OnVuforiaExtendedTrackingStarted(trackableFound, trackableFoundNumber);

			if(newTrackableStatus != TrackableBehaviour.Status.EXTENDED_TRACKED && lastTrackableStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
				OnVuforiaExtendedTrackingFinished(trackableFound, trackableFoundNumber);

			lastTrackableStatus = newTrackableStatus;
		}

		/// Image target found / lost.
		private void UpdateTarget(TrackableBehaviour trackable, int trackablesCount){
			if(trackablesCount > 0){
				
				// trackable found
				GameObject newTrackableFound = trackable.gameObject;
				int newTrackableFoundNumber = GetImageTargetNumber(newTrackableFound.name);
				if(newTrackableFoundNumber != trackableFoundNumber){

					if(trackableFoundNumber != trackableDefaultValue)
						OnVuforiaImageTargetLost(trackableFound, trackableFoundNumber);

					OnVuforiaImageTargetFound(newTrackableFound, newTrackableFoundNumber);
					trackableFound = newTrackableFound;
					trackableFoundNumber = newTrackableFoundNumber;
				}
				
			}
			else{
				if(trackableFound != null){
					// trackable lost
					OnVuforiaImageTargetLost(trackableFound, trackableFoundNumber);
					trackableFoundNumber = trackableDefaultValue;
					trackableFound = null;
				}

			}
		}

		private int GetImageTargetNumber(string imageTargetName){
            string pointNumberStr = imageTargetName.Substring("Point_".Length);
            
            if(pointNumberStr.Contains("_"))
                pointNumberStr = pointNumberStr.Substring(0, pointNumberStr.IndexOf('_'));

            return int.Parse(pointNumberStr);
        }
	}
}

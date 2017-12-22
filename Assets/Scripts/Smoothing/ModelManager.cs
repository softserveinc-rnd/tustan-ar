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

namespace TustanAR.Smoothing {
	using VuforiaExtensions;

	/// Entry point for handling model smoothing.
	public class ModelManager : MonoBehaviour {

		[SerializeField]
		private bool _smoothEnabled = true;
		[SerializeField]
		private uint _framesDelay = 8;

		public bool smoothEnabled {
			get { 
				return _smoothEnabled; 
			}
			set {
				_smoothEnabled = value;
				updateSmoothing(); // updates smoothing on runtime
			}
		}
		public uint framesDelay {
			get {
				return _framesDelay;
			}
			set {
				_framesDelay = value;
				updateSmoothing(); // updates smoothing on runtime
			}
		}

		private List<ModelItem> modelList = new List<ModelItem>();
		private GameObject castle;
		private GameObject invisibleModels;
		private GameObject visibleModels;
		private ISmoothManager smoothManager;

		void Start () {
			VuforiaTargets.OnVuforiaImageTargetFound += OnVuforiaImageTargetFound;
			VuforiaTargets.OnVuforiaImageTargetLost += OnVuforiaImageTargetLost;

			setupAndHideAllModels();

			updateSmoothing();
		}
		
		void Update () {
			// update position on castle, if on scene
			smoothManager.OnTargetUpdate();
		}

		private void OnVuforiaImageTargetFound(GameObject root, int pointNumber){
			List<ModelItem> models = modelList.FindAll(ml => ml.pointNumber == pointNumber);
			if(models.Count == 0){
				UnityEngine.Debug.LogError("There is no point #" + pointNumber + " defined on scene in Points GameObject in component ModelManager.prefabsList");
				return;
			}

			castle = models[0].gameObject;
			castle.SetActive(true);
			castle.transform.SetParent(visibleModels.transform);

			smoothManager.OnTargetFound(castle: castle, imageTarget: root);
		}

		private void OnVuforiaImageTargetLost(GameObject root, int pointNumber){
			castle.transform.SetParent(invisibleModels.transform);
			castle.SetActive(false);

			smoothManager.OnTargetLost();
		}

		// this methods creates references to all models and hides them into object "Points/Invisible Models"
		private void setupAndHideAllModels(){

			invisibleModels = new GameObject("Invisible Models");
			invisibleModels.transform.SetParent(transform);
			invisibleModels.transform.SetAsFirstSibling();

			visibleModels = new GameObject("Visible Models");
			visibleModels.transform.SetParent(transform);
			visibleModels.transform.SetSiblingIndex(1);

			foreach(Transform transform in GetComponentsInChildren<Transform>()){

				// find all ImageTargets 
				if(transform.parent == this.transform && transform.name.Contains("Point")){
					transform.position = Vector3.zero;
					
					foreach(Transform childTransform in transform){

						// find all model prefabs
						if(childTransform.parent == transform){
							childTransform.transform.SetParent(invisibleModels.transform);
							childTransform.localScale = transform.localScale;
							childTransform.gameObject.SetActive(false);

							int pointNumber = GetImageTargetNumber(childTransform.name);
							int itemIndex = modelList.FindIndex(ml => ml.pointNumber == pointNumber);
							if(itemIndex == -1){ 
								ModelItem newModelItem = new ModelItem{
									gameObject = childTransform.gameObject,
									pointNumber = pointNumber
								};
								modelList.Add(newModelItem);
							}
						}
					}
				}
			}
		}

		private static int GetImageTargetNumber(string imageTargetName){
            string pointNumberStr = imageTargetName.Substring("Point_".Length);
            
            if(pointNumberStr.Contains("_"))
                pointNumberStr = pointNumberStr.Substring(0, pointNumberStr.IndexOf('_'));

            return int.Parse(pointNumberStr);
        }

		/// Select proper Smooth Manager.
		private void updateSmoothing(){
			if(smoothEnabled){
				smoothManager = new OpticalSmoothManager(withFramesDelay: framesDelay);
			}
			else{
				smoothManager = new NoSmoothManager();
			}
		}
	}
}

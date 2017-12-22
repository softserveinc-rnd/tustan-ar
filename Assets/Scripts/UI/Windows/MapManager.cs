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
using System.Collections.Generic;
using UnityEngine;

namespace TustanAR.UI.Windows{
	using Positioning;
	using Controls;
	using Locale;
	using Native;
	
	///
	/// Sigleton which works with UI Canvas on Map window.
	///
	public sealed class MapManager: IUIWindow {

	#region SPRITES
		private Sprite map;
		private Sprite outOfMapIcon;
		private Marker[] markers;
		private ImageBorders mapBorders = Config.MapBorders;

	#endregion

	#region GAMEOBJECTS
		private GameObject mapWindow;
		private GameObject mapObject;
		private IImage backArrowImage;
		private IImage locationIconImage;
		private IImage outOfMapIconImage;
		private List<IImage> markerImages;
	#endregion

		private bool gps_off_message_shown = false;

	#region SINGLETON_DECLARATION
		private static MapManager instance;

		private MapManager() {}

		public static MapManager Instance
		{
			get 
			{
				if (instance == null)
				{
					instance = new MapManager();
				}
				return instance;
			}
		}
	#endregion

		public void Init (Sprite map, Sprite outOfMapIcon, Marker[] markers) {
			this.map = map;
			this.outOfMapIcon = outOfMapIcon;
			this.markers = markers;
		}

		public void ShowMap(){
			markerImages = new List<IImage>();
			mapWindow = new GameObject(Names.MAP_WINDOW_NAME);
			mapWindow.transform.SetParent(Runtime.canvas.transform);
			mapWindow.transform.localPosition = Vector3.zero;

			InstantiateMapToFullScreen();
			CreateMarkers();
			CreateIcons();
			CreateUpperPanel();
			UpdateMarkersLocation();

			if(!Input.location.isEnabledByUser){
				if(!gps_off_message_shown){

					new Controls.Dialog.Builder()
						.SetIcon(SpriteLoader.LoadSprite("compas_ico"))
						.SetButton(LocalizationManager.Instance.GetLocalizedValue("ok"), NativeAdapter.Instance.GoToGPSSettings)
						.SetText(LocalizationManager.Instance.GetLocalizedValue("gps_disabled"))
						.SetView(Relative.MAP_WINDOW_RECT)
						.build()
						.show();

					gps_off_message_shown = true;
				}
			}
			else {
				gps_off_message_shown = false;
			}

			UpdateLocation(Input.location.lastData);

			History.AddWindow(this);
		}

		public void Close(){
			UnityEngine.Object.Destroy(mapWindow);

			backArrowImage.Dispose();
			locationIconImage.Dispose();

			foreach(var image in markerImages)
				image.Dispose();

			Resources.UnloadUnusedAssets();
		}

	#region MAP_GAMEOBJECTS_CREATING
		private void InstantiateMapToFullScreen(){
			mapObject = new GameObject(Names.MAP_NAME, typeof(UnityEngine.UI.Image));
			mapObject.GetComponent<UnityEngine.UI.Image>().sprite = map;
			mapObject.transform.SetParent(mapWindow.transform);
			mapObject.transform.localPosition = Vector3.zero; // in screen center
			mapObject.GetComponent<RectTransform>().sizeDelta = Vector2.one * Mathf.Max(Screen.width, Screen.height);
		}

		private void CreateIcons(){
			// Warning: outOfMapIconObject must be created before UI buttons!!! in this case it won't overlay them on the screen
			outOfMapIconImage = new Image.Builder()
				.SetName(Names.OUT_OF_MAP_ICON_NAME)
				.SetImage(outOfMapIcon)
				.SetParent(mapWindow.transform)
				.SetSize(Vector2.one * Relative.OUT_OF_MAP_ARROW_LENGH)
				.SetActive(false)
				.build();

			locationIconImage = new Controls.Image.Builder()
				.SetName(Names.LOCATION_ICON_NAME)
				.SetImage(SpriteLoader.LoadSprite("marker_user"))
				.SetParent(mapWindow.transform)
				.SetSize(new Vector2(Relative.MARKER_ICON_WIDTH, Relative.MARKER_ICON_HEIGHT))
				.SetRotationPoint(RotationPoint.Center)
				.SetActive(false)
				.build();
		}

		private void CreateMarkers(){
			for(int i=0; i<markers.Length; i++){
				string title = markers[i].title;
				IImage markerImage = new Image.Builder()
					.SetName(Names.MARKER_BADGE_NAME + " " + markers[i].id)
					.AddOnClickListener(new Toast(title, Toast.LENGTH_LONG).show)
					.SetImage(markers[i].sprite)
					.SetParent(mapWindow.transform)
					.SetSize(size: new Vector2(Relative.OUT_MARKER_ICON_WIDTH, Relative.OUT_MARKER_ICON_HEIGHT), additionalClickableArea:3f)
					.SetRotationPoint(UI.RotationPoint.Bottom)
					.build();

				markerImages.Add(markerImage); // save marker object to list for success processing in future
			}
		}

		private void CreateUpperPanel(){
			GameObject upperPanelObject = new GameObject("Upper Panel", typeof(UnityEngine.UI.Image));
            upperPanelObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Relative.UPPER_PANEL_HEIGTH);
            upperPanelObject.transform.position = new Vector2(Screen.width / 2f, Screen.height - Relative.UPPER_PANEL_HEIGTH / 2f);
            upperPanelObject.GetComponent<UnityEngine.UI.Image>().color = Color.black;
            upperPanelObject.transform.SetParent(mapWindow.transform);

			backArrowImage = new Controls.Image.Builder()
				.SetName(Names.BACK_ARROW_ICON_NAME)
				.AddOnClickListener(History.GoBack)
				.SetImage(SpriteLoader.LoadSprite("back_arrow_ico"))
				.SetParent(upperPanelObject.transform)
				.SetSize(size: Vector2.one * Relative.BUTTON_HEIGHT, additionalClickableArea:2f)
				.SetPosition(new Vector2(Relative.BUTTON_MARGIN + Relative.BUTTON_HEIGHT / 2f, Screen.height - Relative.BUTTON_MARGIN - Relative.BUTTON_HEIGHT / 3f))
				.SetRotationPoint(UI.RotationPoint.Center)
				.build();

			GameObject upperPanelText = new GameObject("Text", typeof(UnityEngine.UI.Text));
			upperPanelText.transform.SetParent(upperPanelObject.transform);
			upperPanelText.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width - 4 * Relative.BUTTON_MARGIN - Relative.BUTTON_HEIGHT / 2f, Relative.UPPER_PANEL_HEIGTH);
			upperPanelText.GetComponent<RectTransform>().position = new Vector2(Screen.width / 2f + 2 * Relative.BUTTON_MARGIN + Relative.BUTTON_HEIGHT / 4f, Screen.height - Relative.UPPER_PANEL_HEIGTH / 2f);
			upperPanelText.GetComponent<UnityEngine.UI.Text>().font = Resources.Load<Font>("fonts/Roboto-Regular");
			upperPanelText.GetComponent<UnityEngine.UI.Text>().fontSize = Relative.SMALL_FONT_SIZE;
			upperPanelText.GetComponent<UnityEngine.UI.Text>().alignment = TextAnchor.MiddleLeft;
			upperPanelText.GetComponent<UnityEngine.UI.Text>().color = Color.white;
			upperPanelText.GetComponent<UnityEngine.UI.Text>().text = Locale.LocalizationManager.Instance.GetLocalizedValue("ar_points_location");
		}

	#endregion

	#region MAP_GAMEOBJECTS_LOCATION_UPDATING
		public void UpdateLocation(LocationInfo location){
			
			// hotfix for Unity launch
			if (locationIconImage.gameObject == null){
				return;
			}
			
			if(location.latitude * location.longitude != 0){
				locationIconImage.gameObject.SetActive(true);
				Vector2 location_in_pixels = UpdateImageLocationFromGlobalCoords(locationIconImage, new Location(location));
				float half_marker_height = Relative.MAP_MARKER_HEIGHT / 2f;
				Rect view = Relative.MAP_WINDOW_RECT;
				if(location_in_pixels.x < view.xMin - half_marker_height 
				|| location_in_pixels.x > view.xMax + half_marker_height 
				|| location_in_pixels.y < view.yMin - half_marker_height 
				|| location_in_pixels.y > view.yMax + half_marker_height){
					// location is out of map
					outOfMapIconImage.gameObject.SetActive(true);
					UpdateOutOfMapArrow(location_in_pixels);

				} else {
					// location is in screen bounds
					outOfMapIconImage.gameObject.SetActive(false);
				}
			}
			else { // unknown location
				locationIconImage.gameObject.SetActive(false);
			}
		}

		private void UpdateOutOfMapArrow(Vector2 location)
		{
			Rect view = Relative.MAP_WINDOW_RECT;
			float x = location.x;
			float y = location.y;
			float w = view.width;
			float h = view.height;
			float loc_tan = (y - h/2) / (x  - w/2); // tangent of line between location and screen center
			float half_arrow_length = Relative.OUT_OF_MAP_ARROW_LENGH / 2f;
			Vector2 new_arrow_position = new Vector2();
			float new_arrow_angle = 0f;
			Vector2 v = half_arrow_length * new Vector2(Mathf.Cos(Mathf.Atan(Mathf.Abs(loc_tan))), Mathf.Sin(Mathf.Atan(Mathf.Abs(loc_tan))));
			
			if(y > (-h/w)*x + h){
				// top or right
				if(y > (h/w)*x){
					//top
					Vector2 arrow_directed_to = new Vector2((w*y-w*h+h*x) / (2*y-h), h);
					new_arrow_position = arrow_directed_to - new Vector2(Mathf.Sign(loc_tan)*v.x, v.y);
					new_arrow_angle = Mathf.Atan(loc_tan) * Mathf.Rad2Deg + (loc_tan < 0 ? 180f : 0f);
				}
				else{
					// right
					Vector2 arrow_directed_to = new Vector2(w, (w*y-w*h+h*x) / (2*x-w));
					new_arrow_position = arrow_directed_to - new Vector2(v.x, Mathf.Sign(loc_tan)*v.y);
					new_arrow_angle = Mathf.Atan(loc_tan) * Mathf.Rad2Deg;
				}
			}
			else {
				// left or bottom
				if(y > (h/w)*x){
					//left
					Vector2 arrow_directed_to = new Vector2(0f, (h*x-w*y) / (2*x-w));
					new_arrow_position = arrow_directed_to + new Vector2(v.x, Mathf.Sign(loc_tan)*v.y);
					new_arrow_angle = Mathf.Atan(loc_tan) * Mathf.Rad2Deg + 180f;
				}
				else{
					//bottom
					Vector2 arrow_directed_to = new Vector2((w*y-h*x) / (2*y-h), 0f);
					new_arrow_position = arrow_directed_to + new Vector2(Mathf.Sign(loc_tan)*v.x, v.y);
					new_arrow_angle = Mathf.Atan(loc_tan) * Mathf.Rad2Deg + (loc_tan > 0 ? 180f : 0f);
				}
			}

			outOfMapIconImage.gameObject.transform.rotation = Quaternion.Euler(0f,0f,new_arrow_angle);
			outOfMapIconImage.UpdatePosition(new_arrow_position);
		}

		private void UpdateMarkersLocation(){
			// fix on Unity launch
			if(markers.Length == 0)
				return;
				
			for(int i=0; i<markers.Length; i++){
				UpdateImageLocationFromGlobalCoords(markerImages[i], markers[i].location);
			}
		}

		private Vector2 UpdateImageLocationFromGlobalCoords(IImage image, Location location){

			// get image creen coords
			Vector3[] imageCorners = new Vector3[4];
			RectTransform mapObjectRT = mapObject.GetComponent<RectTransform>();
			mapObjectRT.GetWorldCorners(imageCorners);

			// corners in pixels
			var localMapCoords = new ImageBorders(
				_leftUpper: new Location(imageCorners[0]), 
				_rightBottom: new Location(imageCorners[2]));

			var localMarkerLocation = new Vector2();
			localMarkerLocation.x = (float)(localMapCoords.leftUpper.longitude + ((location.longitude - mapBorders.leftUpper.longitude)/(mapBorders.rightBottom.longitude - mapBorders.leftUpper.longitude))*(localMapCoords.rightBottom.longitude - localMapCoords.leftUpper.longitude));
			localMarkerLocation.y = (float)(Screen.height - (localMapCoords.leftUpper.latitude + ((location.latitude - mapBorders.leftUpper.latitude)/(mapBorders.rightBottom.latitude - mapBorders.leftUpper.latitude))*(localMapCoords.rightBottom.latitude - localMapCoords.leftUpper.latitude)));
			
			if(image.rotationPoint == RotationPoint.Bottom)
				localMarkerLocation += new Vector2(0, Relative.MAP_MARKER_HEIGHT / 2f);
				
			// changing position of marker
			image.UpdatePosition(localMarkerLocation);

			return localMarkerLocation;
		}

	#endregion
	}
}
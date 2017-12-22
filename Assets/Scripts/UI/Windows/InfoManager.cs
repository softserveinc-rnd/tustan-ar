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
using UnityEngine.UI;

namespace TustanAR.UI.Windows{
	using Controls;
	using Locale;

	public sealed class InfoManager: IUIWindow {

		private Sprite infoTitleBackground;

		private GameObject infoObject;
		private IImage xIconImage;
		private IImage backArrowImage;
		private GameObject scrollablePanelObject;
		private GameObject scrollableContent;
		private GameObject titleObject;
		private GameObject textContentObject;
		private GameObject titleTextObject;
		private GameObject textBackgroundObject;
		private GameObject upperPanelObject;
		private float textContentToAllRatio;

	#region SINGLETON_DECLARATION
		private static InfoManager instance;

		private InfoManager() {}

		public static InfoManager Instance
		{
			get 
			{
				if (instance == null)
				{
					instance = new InfoManager();
				}
				return instance;
			}
		}
	#endregion

		public void Init(){
			this.infoTitleBackground = SpriteLoader.LoadSprite("castle_background");
		}

		public void ShowInfo(){
			infoObject = new GameObject(Names.INFO_WINDOW_NAME);
			infoObject.transform.SetParent(Runtime.canvas.transform);

			CreateScrollableTextPanel();
			CreateXIcon();
			UpdateScrollable();
			CreateUpperPanel();

			History.AddWindow(this);
		}

		public void Close(){
			Object.Destroy(infoObject);

			xIconImage.Dispose();
			backArrowImage.Dispose();

			Resources.UnloadUnusedAssets();
		}

		private void CreateXIcon(){
			xIconImage = new Image.Builder()
				.SetName(Names.X_ICON_NAME)
				.AddOnClickListener(History.GoBack)
				.SetImage(SpriteLoader.LoadSprite("back_arrow_ico"))
				.SetParent(infoObject.transform)
				.SetSize(size: Vector2.one * Relative.BUTTON_HEIGHT, additionalClickableArea:3f)
				.SetPosition(new Vector2(Relative.BUTTON_MARGIN + Relative.BUTTON_HEIGHT / 2f, Screen.height - (Relative.BUTTON_MARGIN + Relative.BUTTON_HEIGHT / 2f)))
				.SetRotationPoint(RotationPoint.Center)
				.build();
		}

		private GameObject CreateTitle(){

			// creating background image
			GameObject titleObject = new GameObject(Names.INFO_WINDOW_TITLE_NAME, typeof(UnityEngine.UI.Image));
			titleObject.GetComponent<UnityEngine.UI.Image>().sprite = infoTitleBackground;
			titleObject.transform.SetParent(infoObject.transform);

			// creating title text
			titleTextObject = new GameObject(Names.INFO_WINDOW_TITLE_NAME + " Text", typeof(Text), typeof(ContentSizeFitter));
			titleTextObject.transform.SetParent(titleObject.transform);

			// setting title options
			var titleTextTextComponent = titleTextObject.GetComponent<Text>();
			titleTextTextComponent.font = Resources.Load<Font>("fonts/Roboto-Regular");
			titleTextTextComponent.color = Color.white;
			titleTextTextComponent.lineSpacing = Config.FONT_LINE_SPACING;
			titleTextObject.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		
			return titleObject;
		}

		private GameObject CreateScrollableTextPanel() {

			// create Scrollable Panel and set it to full screen
			scrollablePanelObject = new GameObject(Names.SCROLLABLE_PANEL_NAME, typeof(UnityEngine.UI.Image), typeof(ScrollRect), typeof(Mask));
			scrollablePanelObject.transform.SetParent(infoObject.transform);

			scrollableContent = new GameObject("Scrollable Content", typeof(UnityEngine.UI.Image));
			scrollableContent.transform.SetParent(scrollablePanelObject.transform);
			scrollableContent.GetComponent<UnityEngine.UI.Image>().color = Color.clear;
			scrollablePanelObject.GetComponent<ScrollRect>().content = scrollableContent.GetComponent<RectTransform>();
			scrollablePanelObject.GetComponent<ScrollRect>().horizontal = false;

			// get Title ready
			titleObject = CreateTitle();
			titleObject.transform.SetParent(scrollableContent.transform);

			textBackgroundObject = new GameObject("Text Body", typeof(UnityEngine.UI.Image));
			textBackgroundObject.GetComponent<UnityEngine.UI.Image>().color = Color.white;
			textBackgroundObject.transform.SetParent(scrollableContent.transform);
			
			textContentObject = new GameObject("Text Content", typeof(Text), typeof(ContentSizeFitter));
			textContentObject.transform.SetParent(textBackgroundObject.transform);

			textContentObject.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			var textContentObjectRectTransform = textContentObject.GetComponent<RectTransform>();
			textContentObjectRectTransform.anchorMin = new Vector2(0, 1); // upper left
			textContentObjectRectTransform.anchorMax = new Vector2(0, 1); // upper left
			textContentObjectRectTransform.pivot = new Vector2(0, 1);

			var textContentObjectTextComponent = textContentObject.GetComponent<Text>();
			textContentObjectTextComponent.font = Resources.Load<Font>("fonts/Roboto-Regular");
			textContentObjectTextComponent.lineSpacing = Config.FONT_LINE_SPACING;
			textContentObjectTextComponent.color = Color.black;

			scrollableContent.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1); // upper left
			scrollableContent.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1); // upper left
			scrollableContent.GetComponent<RectTransform>().pivot = new Vector2(0, 1);

			scrollablePanelObject.transform.localScale = Vector3.one;

			return scrollablePanelObject;
		}
		
		private void UpdateScrollable(){

			scrollablePanelObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
			scrollablePanelObject.transform.position = new Vector2(Screen.width, Screen.height) / 2f;
			textContentObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width - 2 * Relative.TEXT_HORIZONTAL_MARGIN, textContentObject.GetComponent<RectTransform>().sizeDelta.y); // set left text margin

			textContentObject.GetComponent<Text>().text = "\n";

			textContentObject.GetComponent<Text>().text += "\n<size=" + (int)Relative.MEDIUM_FONT_SIZE + ">" + LocalizationManager.Instance.GetLocalizedValue("info_window_content_title_part1") + "\n</size>";
			textContentObject.GetComponent<Text>().text += "<size=" + (int)Relative.SMALL_FONT_SIZE + "><color=grey>" + LocalizationManager.Instance.GetLocalizedValue("info_window_content_subtitle_part1") + "</color>\n\n" + LocalizationManager.Instance.GetLocalizedValue("info_window_content_body_part1") + "\n\n</size>";

			textContentObject.GetComponent<Text>().text += "\n<size=" + (int)Relative.MEDIUM_FONT_SIZE + ">" + LocalizationManager.Instance.GetLocalizedValue("info_window_content_title_part2") + "\n</size>";
			textContentObject.GetComponent<Text>().text += "<size=" + (int)Relative.SMALL_FONT_SIZE + "><color=grey>" + LocalizationManager.Instance.GetLocalizedValue("info_window_content_subtitle_part2") + "</color>\n\n" + LocalizationManager.Instance.GetLocalizedValue("info_window_content_body_part2") + "\n\n</size>";

			textContentObject.GetComponent<Text>().text += "\n<size=" + (int)Relative.MEDIUM_FONT_SIZE + ">" + LocalizationManager.Instance.GetLocalizedValue("info_window_content_title_part3") + "\n</size>";
			textContentObject.GetComponent<Text>().text += "<size=" + (int)Relative.SMALL_FONT_SIZE + "><color=grey>" + LocalizationManager.Instance.GetLocalizedValue("info_window_content_subtitle_part3") + "</color>\n\n" + LocalizationManager.Instance.GetLocalizedValue("info_window_content_body_part3") + "\n\n</size>";

			textContentObject.GetComponent<Text>().text += "\n<size=" + (int)Relative.SMALL_FONT_SIZE + "><color=grey>" + Config.LICENSE_INFO + "</color>\n\n</size>";
			
			float actualImageHeight = Screen.width * infoTitleBackground.rect.height / infoTitleBackground.rect.width;
			float cuttedImageHeight = UpdateTitle();
			float scrollableContentHeigthInPixels = textContentObject.GetComponent<Text>().preferredHeight + cuttedImageHeight;
			scrollableContent.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, scrollableContentHeigthInPixels);
			textBackgroundObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, textContentObject.GetComponent<Text>().preferredHeight);
			scrollableContent.transform.position = new Vector2(0f, Screen.height / 2f); // need only to center horizontally scrollable content
			titleObject.transform.localPosition = new Vector2(Screen.width / 2f, - actualImageHeight / 2f);
			textBackgroundObject.transform.localPosition = new Vector2(Screen.width / 2f, - cuttedImageHeight - textContentObject.GetComponent<Text>().preferredHeight / 2f);
			textContentObject.transform.localPosition = new Vector3(- Screen.width / 2f + Relative.TEXT_HORIZONTAL_MARGIN, textContentObject.GetComponent<Text>().preferredHeight / 2f);
		
			textContentToAllRatio = scrollableContentHeigthInPixels / (scrollableContentHeigthInPixels + cuttedImageHeight);
		}

		private float UpdateTitle(){

			float imageWidth = Screen.width;
			float actualImageHeight = imageWidth * infoTitleBackground.rect.height / infoTitleBackground.rect.width;
			float cuttedImageHeight = Mathf.Min(actualImageHeight, Screen.height * 7f / 16f);
			titleObject.GetComponent<RectTransform>().sizeDelta = new Vector2(imageWidth, actualImageHeight);
			titleTextObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width - 2 * Relative.TEXT_HORIZONTAL_MARGIN, 100f); // TODO: what is the second argument?? height of block in pixels??

			var titleText = titleTextObject.GetComponent<RectTransform>();
			titleText.sizeDelta = new Vector2(Screen.width - 2 * Relative.TEXT_HORIZONTAL_MARGIN, 100f);
			titleText.anchorMin = new Vector2(0f, 0f); // lower left
			titleText.anchorMax = new Vector2(0f, 0f); // lower left
			titleText.pivot = new Vector2(0f, 0f);

			titleTextObject.GetComponent<Text>().text = "<size=" + (int)Relative.LARGE_FONT_SIZE + ">" + LocalizationManager.Instance.GetLocalizedValue("rock_fortress_city_tustan") + "</size>\n";
			titleTextObject.GetComponent<Text>().text += "<size=" + (int)Relative.MEDIUM_FONT_SIZE + ">" + LocalizationManager.Instance.GetLocalizedValue("info_window_subtitle") + "</size>\n";
			
			titleTextObject.transform.localPosition = new Vector2(- imageWidth / 2 + Relative.TEXT_HORIZONTAL_MARGIN, actualImageHeight / 2f - cuttedImageHeight);

			return cuttedImageHeight;
		} 

		private void CreateUpperPanel(){
			upperPanelObject = new GameObject("Upper Panel", typeof(UnityEngine.UI.Image));
            upperPanelObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Relative.UPPER_PANEL_HEIGTH);
            upperPanelObject.transform.position = new Vector2(Screen.width / 2f, Screen.height - Relative.UPPER_PANEL_HEIGTH / 2f);
            upperPanelObject.GetComponent<UnityEngine.UI.Image>().color = Color.black;
            upperPanelObject.transform.SetParent(infoObject.transform);

			backArrowImage = new Image.Builder()
				.SetName(Names.BACK_ARROW_ICON_NAME)
				.AddOnClickListener(History.GoBack)
				.SetImage(SpriteLoader.LoadSprite("back_arrow_ico"))
				.SetParent(upperPanelObject.transform)
				.SetSize(size: Vector2.one * Relative.BUTTON_HEIGHT, additionalClickableArea:2f)
				.SetPosition(new Vector2(Relative.BUTTON_MARGIN + Relative.BUTTON_HEIGHT / 2f, Screen.height - Relative.BUTTON_MARGIN - Relative.BUTTON_HEIGHT / 2f))
				.SetRotationPoint(UI.RotationPoint.Center)
				.build();

			GameObject upperPanelText = new GameObject("Text", typeof(Text));
			upperPanelText.transform.SetParent(upperPanelObject.transform);
			upperPanelText.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width - 4 * Relative.BUTTON_MARGIN - Relative.BUTTON_HEIGHT / 2f, Relative.UPPER_PANEL_HEIGTH);
			upperPanelText.GetComponent<RectTransform>().position = new Vector2(Screen.width / 2f + 2 * Relative.BUTTON_MARGIN + Relative.BUTTON_HEIGHT / 4f, Screen.height - Relative.UPPER_PANEL_HEIGTH / 2f);
			upperPanelText.GetComponent<Text>().font = Resources.Load<Font>("fonts/Roboto-Regular");
			upperPanelText.GetComponent<Text>().fontSize = Relative.SMALL_FONT_SIZE;
			upperPanelText.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
			upperPanelText.GetComponent<Text>().color = Color.white;
			upperPanelText.GetComponent<Text>().text = LocalizationManager.Instance.GetLocalizedValue("rock_fortress_city_tustan");

			upperPanelObject.SetActive(false);

			scrollablePanelObject.GetComponent<ScrollRect>().onValueChanged.AddListener(
				(Vector2 value) => upperPanelObject.SetActive(value.y < textContentToAllRatio)
			);
		}
	}
}
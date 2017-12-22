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
using System.IO;

namespace TustanAR.Locale{
    using Native;

    public sealed class LocalizationManager : MonoBehaviour {

        public static LocalizationManager Instance { get; internal set; }

        private Dictionary<string, string> localizedText;
        private string missingTextString = "Localized text not found";
        private bool isReady = false;

        // Use this for initialization
        void Awake () 
        {
            if(Instance == null)
                Instance = this;
        }

        public IEnumerator Load(SystemLanguage systemLanguage){
            string langPacName;
            switch(systemLanguage){
				case SystemLanguage.Ukrainian: langPacName = "uk.json"; break;
				case SystemLanguage.English: langPacName = "en.json"; break;
				default: langPacName = "en.json"; break;
			}
			
			yield return LocalizationManager.Instance.LoadLocalizedText(langPacName);
			while(!LocalizationManager.Instance.GetIsReady())
				yield return null;
        }
        
        private IEnumerator LoadLocalizedText(string fileName){

            localizedText = new Dictionary<string, string> ();
            string filePath = Path.Combine (Application.streamingAssetsPath,fileName);

            string dataAsJson = default(string);

            if (filePath.Contains("://")) { // android
            
                UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
                yield return www.SendWebRequest();
                dataAsJson = www.downloadHandler.text;

                if(string.IsNullOrEmpty(dataAsJson)){
                    UnityEngine.Debug.LogError ("Cannot find file!");
                    yield break;
                }

            } else{ // ios/windows/mac
                
                if (File.Exists (filePath)) {
                    dataAsJson = File.ReadAllText (filePath);
                } else 
                {
                    UnityEngine.Debug.LogError ("Cannot find file!");
                    yield break;
                }
            }

            LocalizationData loadedData = LocalizationData.CreateFromJSON(dataAsJson);
            NativeAdapter.Instance.LocalizePlugins(dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++) 
            {
                localizedText.Add (loadedData.items [i].key, loadedData.items [i].value);   
            }

            UnityEngine.Debug.Log ("Data loaded, dictionary contains: " + localizedText.Count + " entries");
            isReady = true;
        }

        public string GetLocalizedValue(string key){
            
            string result = missingTextString;
            if (localizedText.ContainsKey (key)) 
            {
                result = localizedText [key];
            }

            return result;

        }

        public bool GetIsReady(){
            return isReady;
        }
    }
}
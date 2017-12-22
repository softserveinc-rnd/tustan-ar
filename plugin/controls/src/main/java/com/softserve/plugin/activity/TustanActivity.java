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

package com.softserve.plugin.activity;

import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.SharedPreferences;
import android.os.Bundle;

import com.softserve.plugin.controls.R;
import com.softserve.plugin.instructions.ImageAnimationFactory;
import com.softserve.plugin.locale.LocalizationManager;
import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

public class TustanActivity extends UnityPlayerActivity{
    private static final String NEVER_ASK_AGAIN_KEY = "NEVER_ASK_AGAIN_KEY";
    private static final String SHARED_PREFERENCES_NAME = "SHARED_PREFERENCES_NAME";
    private static TustanActivity activity;
    private boolean never_ask_again;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        activity = this;

        SharedPreferences sharedPref = getSharedPreferences(SHARED_PREFERENCES_NAME, Context.MODE_PRIVATE);
        never_ask_again = sharedPref.getBoolean(NEVER_ASK_AGAIN_KEY, false);
    }

    public static void LaunchIntroduction(){
        activity.launchIntroduction();
    }

    private void launchIntroduction(){
        if(never_ask_again){
            return;
        }

        CharSequence[] never_ask_again = { LocalizationManager.Instance.GetLocalizedValue("never_ask_again") };
        final List<Boolean> never_ask_again_checked = new ArrayList<>();
        never_ask_again_checked.add(false);

        new AlertDialog.Builder(UnityPlayer.currentActivity)
                .setTitle(LocalizationManager.Instance.GetLocalizedValue("intro_subtitle"))
                .setPositiveButton(LocalizationManager.Instance.GetLocalizedValue("start"), new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        dialog.dismiss();
                        GoToFirstSection();

                        if(never_ask_again_checked.get(0))
                            neverAskAgain();

                    }
                })
                .setNegativeButton(LocalizationManager.Instance.GetLocalizedValue("hide"), new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {
                        if(never_ask_again_checked.get(0))
                            neverAskAgain();
                    }
                })
                .setMultiChoiceItems(never_ask_again, null, new DialogInterface.OnMultiChoiceClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i, boolean b) {
                        never_ask_again_checked.set(0, b);
                    }
                })
                .create()
                .show();
    }

    private void neverAskAgain(){
        SharedPreferences sharedPref = getSharedPreferences(SHARED_PREFERENCES_NAME, Context.MODE_PRIVATE);
        SharedPreferences.Editor editor = sharedPref.edit();
        editor.putBoolean(NEVER_ASK_AGAIN_KEY, true);
        editor.apply();

    }

    private void GoToFirstSection() {
        ImageAnimationFactory.ShowDialogWithImage(
                /* message: */ LocalizationManager.Instance.GetLocalizedValue("intro_part1"),
                /* images: */ Arrays.asList(R.drawable.map),
                /* buttonText: */ LocalizationManager.Instance.GetLocalizedValue("next"),
                /* skipButtonText: */ LocalizationManager.Instance.GetLocalizedValue("skip"),
                /* callback: */ new ImageAnimationFactory.Callback() {
                    @Override
                    public void Continue() {
                        GoToSecondSection();
                    }
                }
        );
    };

    private void GoToSecondSection(){
        ImageAnimationFactory.ShowDialogWithImage(
                /* message: */ LocalizationManager.Instance.GetLocalizedValue("intro_part2"),
                /* images: */ Arrays.asList(R.drawable.point_6_before, R.drawable.point_6_after),
                /* buttonText: */ LocalizationManager.Instance.GetLocalizedValue("next"),
                /* skipButtonText: */ LocalizationManager.Instance.GetLocalizedValue("skip"),
                /* callback: */ new ImageAnimationFactory.Callback() {
                    @Override
                    public void Continue() {
                        GoToThirdSection();
                    }
                }
        );
    };

    private void GoToThirdSection () {
        int camera_window = LocalizationManager.Instance.GetLocalizedValue("lang").equals("uk")
                ?R.drawable.camera_window_uk
                :R.drawable.camera_window_en;

        ImageAnimationFactory.ShowDialogWithImage(
                /* message: */ LocalizationManager.Instance.GetLocalizedValue("intro_part3"),
                /* images: */ Arrays.asList(camera_window),
                /* buttonText: */ LocalizationManager.Instance.GetLocalizedValue("next"),
                /* skipButtonText: */ LocalizationManager.Instance.GetLocalizedValue("skip"),
                /* callback: */ new ImageAnimationFactory.Callback() {
                    @Override
                    public void Continue() {
                        GoToFourthSection();
                    }
                }
        );
    }

    private void GoToFourthSection(){
        List<Integer> play_store_images = LocalizationManager.Instance.GetLocalizedValue("lang").equals("uk")
                ? Arrays.asList(R.drawable.play_store_uk_1, R.drawable.play_store_uk_2, R.drawable.play_store_uk_3)
                : Arrays.asList(R.drawable.play_store_en_1, R.drawable.play_store_en_2, R.drawable.play_store_en_3);

        ImageAnimationFactory.ShowDialogWithImage(
                /* message: */ LocalizationManager.Instance.GetLocalizedValue("intro_part4"),
                /* images: */ play_store_images,
                /* buttonText: */ LocalizationManager.Instance.GetLocalizedValue("close"),
                /* skipButtonText: */ "",
                /* callback: */ new ImageAnimationFactory.Callback() {
                    @Override
                    public void Continue() {
                        System.gc();
                        UnityPlayer.UnitySendMessage("PluginCallback", "IntroFinished", "");
                    }
                }
        );
    };
}
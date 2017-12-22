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

package com.softserve.plugin.instructions;

import android.app.AlertDialog;
import android.content.DialogInterface;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.softserve.plugin.controls.R;
import com.unity3d.player.UnityPlayer;

import java.util.List;

public class ImageAnimationFactory {
    public static void ShowDialogWithImage(final String message, final List<Integer> imageRes, final String buttonText, final String skipButtonText, final Callback callback){
        final LinearLayout descriptionView = (LinearLayout) UnityPlayer.currentActivity.getLayoutInflater().inflate(R.layout.image_dialog, null);
        ((TextView)descriptionView.findViewById(R.id.image_dialog_text)).setText(message);
        final ImageView imageView = ((ImageView)descriptionView.findViewById(R.id.image_dialog_image));

        final ImageAnimation imageAnimation = new ImageAnimation<Integer>(
                    /* images: */ imageRes,
                    /* slideshow time (millis): */ 1000,
                    /* on animate: */ new ImageAnimation.OnAnimationListener() {
            @Override
            public void onAnimate(Object next) {
                imageView.setImageResource((Integer)next);
            }
        }
        );

        imageAnimation.startAnimation();

        AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(UnityPlayer.currentActivity)
                .setView(descriptionView)
                .setPositiveButton(buttonText, new DialogInterface.OnClickListener() {
                            @Override
                            public void onClick(DialogInterface dialog, int which) {
                                UnityPlayer.currentActivity.removeDialog(which);
                                imageAnimation.dismiss();
                                ((ViewGroup)imageView.getParent()).removeAllViews();
                                callback.Continue();
                            }
                        }
                );

        if(!skipButtonText.isEmpty())
            dialogBuilder.setNeutralButton(skipButtonText, null);

        AlertDialog dialog = dialogBuilder.create();

        dialog.show();
    }

    public interface Callback{
        void Continue();
    }
}

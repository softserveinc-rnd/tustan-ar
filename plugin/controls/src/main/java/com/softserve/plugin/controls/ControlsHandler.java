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

package com.softserve.plugin.controls;

import android.app.AlertDialog;
import android.app.ProgressDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.graphics.Bitmap;
import android.net.Uri;
import android.opengl.GLES20;
import android.widget.ProgressBar;

import com.unity3d.player.UnityPlayer;

import java.io.File;
import java.nio.IntBuffer;

public class ControlsHandler {

    private static ProgressDialog progressDialog;

    public static void GoToGPSSettings(){
        Intent gpsOptionsIntent = new Intent(
                android.provider.Settings.ACTION_LOCATION_SOURCE_SETTINGS);
        UnityPlayer.currentActivity.startActivity(gpsOptionsIntent);
    }

    public static void ShareFile(String message, String shareText, String path, String type){
        File file = new File(path);
        file.setReadable(true, false);

        Intent shareIntent = new Intent();
        shareIntent.setAction(Intent.ACTION_SEND);
        shareIntent.putExtra(Intent.EXTRA_STREAM, Uri.fromFile(file));
        shareIntent.putExtra(Intent.EXTRA_TEXT, shareText);
        shareIntent.setType(type);
        UnityPlayer.currentActivity.startActivity(Intent.createChooser(shareIntent, message));
    }

    public static void MakeScreenshot(final String filename, final int bottom_margin, final int top_margin, final double latitude, final double longitude, final double altitude, final int orientation){
        int window_width = UnityPlayer.currentActivity.getWindow().getDecorView().getWidth();
        int window_heigth = top_margin - bottom_margin;
        int lower_left_x = 0;
        int lower_left_y = bottom_margin;

        saveScreenShot(lower_left_x, lower_left_y, window_width, window_heigth, filename, latitude, longitude, altitude, orientation);
    }

    private static void saveScreenShot(int x, int y, int width, int height, String filename, double latitude, double longitude, double altitude, int orientation) {
        Bitmap bmp = grabPixels(x, y, width, height);

        ImageSaver r = new ImageSaver(bmp, filename);
        r.setLocation(latitude, longitude, altitude);
        r.setOrientation(orientation);
        new Thread(r).start();
    }
    private static Bitmap grabPixels(int x, int y, int w, int h) {
        int b[] = new int[w * (y + h)];
        int bt[] = new int[w * h];
        IntBuffer ib = IntBuffer.wrap(b);
        ib.position(0);

        GLES20.glReadPixels(x, y, w, h,
                GLES20.GL_RGBA, GLES20.GL_UNSIGNED_BYTE, ib);

        for (int i = 0, k = 0; i < h; i++, k++) {
            for (int j = 0; j < w; j++) {
                int pix = b[i * w + j];
                int pb = (pix >> 16) & 0xff;
                int pr = (pix << 16) & 0x00ff0000;
                int pix1 = (pix & 0xff00ff00) | pr | pb;
                bt[(h - k - 1) * w + j] = pix1;
            }
        }
        Bitmap sb = Bitmap.createBitmap(bt, w, h, Bitmap.Config.ARGB_8888);
        return sb;
    }

    public static void ShowProgressDialog(String message){
        if(progressDialog != null)
            progressDialog.dismiss();

        progressDialog = new ProgressDialog(UnityPlayer.currentActivity);
        progressDialog.setMessage(message);
        progressDialog.show();
    }

    public static void HideProgressDialog(){
        if(progressDialog != null){
            progressDialog.dismiss();
            progressDialog = null;
        }
    }
}

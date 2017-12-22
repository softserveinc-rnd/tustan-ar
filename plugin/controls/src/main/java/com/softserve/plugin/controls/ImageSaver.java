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

import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.location.Location;
import android.media.ExifInterface;
import android.net.Uri;
import android.os.Build;
import android.os.Environment;
import android.provider.MediaStore;
import android.util.Log;
import android.util.Rational;
import android.view.Display;
import android.view.Surface;
import android.view.WindowManager;
import android.widget.Toast;

import com.unity3d.player.UnityPlayer;

import java.io.File;
import java.io.FileOutputStream;
import java.text.NumberFormat;
import java.text.ParseException;
import java.util.Calendar;
import java.util.Date;
import java.util.Locale;

public class ImageSaver implements Runnable {
    private String filename;
    private Bitmap bitmap;
    private boolean isLocationSet = false;
    private Location location;
    private DeviceOrientation orientation = DeviceOrientation.Unknown;

    public ImageSaver(Bitmap bitmap, String filename){
        this.bitmap = bitmap;
        this.filename = filename;
    }

    public void setLocation(double latitude, double longitude, double altitude){
        isLocationSet = true;
        location = new Location("");
        location.setLatitude(latitude);
        location.setLongitude(longitude);
        location.setAltitude(altitude);
    }

    public void setOrientation(int orientation){
        this.orientation = DeviceOrientation.getOrientation(orientation);
    }

    public void run() {
        try {
            File directory = new File(Environment.getExternalStorageDirectory() + "/DCIM/Screenshots");
            directory.mkdirs();

            final File file = new File(directory, filename + ".jpg");
            file.createNewFile();

            FileOutputStream fos = new FileOutputStream(file);
            bitmap.compress(Bitmap.CompressFormat.JPEG, 100, fos);

            fos.flush();

            fos.close();

            // refresh media
            UnityPlayer.currentActivity.sendBroadcast(new Intent(Intent.ACTION_MEDIA_SCANNER_SCAN_FILE, Uri.fromFile(file)));


            if (android.os.Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
                final ExifInterface exifInterface = new ExifInterface(file.getAbsolutePath());

                // timestamp
                exifInterface.setAttribute(ExifInterface.TAG_DATETIME, Long.toString(Calendar.getInstance().getTimeInMillis()));

                // rotation
                int orientation_rotate = ExifInterface.ORIENTATION_NORMAL;
                switch(orientation) {
                    case LandscapeLeft:
                        orientation_rotate = ExifInterface.ORIENTATION_ROTATE_270; break;
                    case LandscapeRight:
                        orientation_rotate = ExifInterface.ORIENTATION_ROTATE_90; break;
                    case PortraitUpsideDown:
                        orientation_rotate = ExifInterface.ORIENTATION_ROTATE_180; break;
                    case Unknown:
                    case Portrait:
                    case FaceUp:
                    case FaceDown:
                        orientation_rotate = ExifInterface.ORIENTATION_NORMAL; break;
                }
                exifInterface.setAttribute(ExifInterface.TAG_ORIENTATION, Integer.toString(orientation_rotate));

                // location
                if(isLocationSet && isNotNullLocation(location)) {

                    exifInterface.setAttribute(ExifInterface.TAG_GPS_LATITUDE, convertToDMS(location.getLatitude()));
                    exifInterface.setAttribute(ExifInterface.TAG_GPS_LATITUDE_REF, location.getLatitude() > 0 ? "N" : "S");

                    exifInterface.setAttribute(ExifInterface.TAG_GPS_LONGITUDE, convertToDMS(location.getLongitude()));
                    exifInterface.setAttribute(ExifInterface.TAG_GPS_LONGITUDE_REF, location.getLongitude() > 0 ? "E" : "W");

                    exifInterface.setAttribute(ExifInterface.TAG_GPS_ALTITUDE, convertAltitudeToDMS(location.getAltitude()));
                    exifInterface.setAttribute(ExifInterface.TAG_GPS_ALTITUDE_REF, location.getAltitude() > 0 ? "0" : "1");
                }

                exifInterface.saveAttributes();
            }

            UnityPlayer.UnitySendMessage("PluginCallback", "ScreenshotReady", file.getPath());
        }
        catch(final Exception e){
            Log.e("Unity", e.getMessage());
        }
    }

    private static String convertToDMS(double component){
        String[] dms = Location.convert(component, Location.FORMAT_SECONDS).split(":");

        double num1 = 0, num2 = 0, num3 = 0, denom1 = 1, denom2 = 1, denom3 = 1;
        NumberFormat numberFormat = NumberFormat.getNumberInstance(Locale.getDefault());
        try {
            num1 = numberFormat.parse(dms[0]).doubleValue();
            num2 = numberFormat.parse(dms[1]).doubleValue();
            num3 = numberFormat.parse(dms[2]).doubleValue();
        }
        catch(ParseException ex){
            Log.e("Unity", ex.getMessage());
        }

        if(num1 != 0)
            while(num1 != Math.floor(num1)){
                num1 *= 10;
                denom1 *= 10;
            }

        if(num2 != 0)
            while(num2 != Math.floor(num2)){
                num2 *= 10;
                denom2 *= 10;
            }

        if(num3 != 0)
            while(num3 != Math.floor(num3)){
                num3 *= 10;
                denom3 *= 10;
            }

        return (long)num1 + "/" + (long)denom1 + "," + (long)(long)num2 + "/" + (long)denom2 + "," + (long)num3 + "/" + (long)denom3;
    }

    private static String convertAltitudeToDMS(double altitude){
        double num = altitude;
        double denom = 1;

        if(num != 0)
            while(num != Math.floor(num)){
                num *= 10;
                denom *= 10;
            }

            return (long)num + "/" + (long)denom;
    }

    private static boolean isNotNullLocation(Location location){
        return !(location.getLatitude() == 0 && location.getLongitude() == 0 && location.getAltitude() == 0);
    }
}

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

#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>
#import <CoreLocation/CoreLocation.h>
#import <QuartzCore/QuartzCore.h>
#import <Photos/Photos.h>

#import "UnityAppController.h"

static CLLocation *lastLocation;
static NSMutableArray *globalMarkerLocations;
static NSMutableArray *canSendNotifications;
static NSMutableArray *maxDistance;
static NSMutableArray *lastNotifTime;
static NSMutableArray *markerTitles;
static NSString *notificationTitle;

// default values, may be overwritten
static NSNumber *COOLDOWN_INTERVAL = [NSNumber numberWithInteger: 300000]; // in milliseconds
static NSNumber *COOLDOWN_DISTANCE = [NSNumber numberWithFloat: 50.0f]; // in metres
static NSNumber *NOTIFICATION_DISTANCE = [NSNumber numberWithFloat: 20.0f]; // in metres
static NSNumber *ACCEPTABLE_ACCURACY = [NSNumber numberWithFloat:20.0f]; // in metres

static UIImage* lastSharedImage;
static NSMutableDictionary* LocalizationData = [[NSMutableDictionary alloc]init];

@interface BackgroundController : UnityAppController <CLLocationManagerDelegate>{
    CLLocationManager *locationManager;
}
- (void)SaveScreenshotWithName:(const char*)filename bottomMargin:(float)bottom_margin topMargin:(float)top_margin orientation:(int)orientation;
@end

static BackgroundController* controller;

@implementation BackgroundController{
}

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
    UIUserNotificationSettings *settings = [UIUserNotificationSettings settingsForTypes: UIUserNotificationTypeAlert | UIUserNotificationTypeSound | UIUserNotificationTypeBadge categories:nil];
    [[UIApplication sharedApplication] registerUserNotificationSettings:settings];
    [application beginBackgroundTaskWithName: @"BackgroundUpdating" expirationHandler: nil];
    
    return [super application:application didFinishLaunchingWithOptions:launchOptions];
}

-(void) startUnity:(UIApplication*) application {
    controller = self;
    [super startUnity:application];

    [self startStandardUpdates];
}

- (void)startStandardUpdates
{
    if (nil == locationManager)
        locationManager = [[CLLocationManager alloc] init];
    
    locationManager.delegate = self;
    locationManager.desiredAccuracy = kCLLocationAccuracyBest;
    
    locationManager.distanceFilter = kCLLocationAccuracyBest; 
    
    [locationManager requestAlwaysAuthorization];
//    [locationManager requestWhenInUseAuthorization];
    
    [locationManager startUpdatingLocation];
}

- (void)applicationDidEnterBackground:(UIApplication *)application {
    [[UIApplication sharedApplication] cancelAllLocalNotifications];
    [[UIApplication sharedApplication] setApplicationIconBadgeNumber: 0];
    
    [super applicationDidEnterBackground: application];
}

-(void)locationManager:(CLLocationManager *)manager didUpdateLocations:(NSArray *)locations {
    CLLocation* location = [locations lastObject];
    lastLocation = location;
    [self checkDistanceToMarkers: location];
}

- (void)checkDistanceToMarkers:(CLLocation *)location {

    if(location.horizontalAccuracy <= [ACCEPTABLE_ACCURACY floatValue]){
        
        NSNumber *current_time = [NSNumber numberWithInteger:(int)[[NSDate date] timeIntervalSince1970] * 1000];
        
        for(int i =0; i<[globalMarkerLocations count]; i++){
            NSNumber *distance_to_marker = [NSNumber numberWithFloat:[globalMarkerLocations[i] distanceFromLocation:location]];
            
            if([distance_to_marker floatValue] >= [maxDistance[i] floatValue])
                maxDistance[i] = [NSNumber numberWithFloat:[distance_to_marker floatValue]];
            
            if(([current_time floatValue] - [lastNotifTime[i] floatValue]) >= [COOLDOWN_INTERVAL floatValue] && [maxDistance[i] floatValue] >= [COOLDOWN_DISTANCE floatValue])
                canSendNotifications[i]=@YES;
            
            if([distance_to_marker floatValue] <= [NOTIFICATION_DISTANCE floatValue] && [canSendNotifications[i] isEqual: @YES]){
                [self ShowNotification: [NSString stringWithFormat:@"%@ %@",notificationTitle, markerTitles[i]]];
                lastNotifTime[i] = [NSNumber numberWithInteger:[current_time integerValue]];
                maxDistance[i] = [NSNumber numberWithFloat:0.0f];

                canSendNotifications[i] = @NO;
            }
        }
    }
    else for(int i=0; i<[globalMarkerLocations count]; i++)
            maxDistance[i] = [NSNumber numberWithFloat:[COOLDOWN_DISTANCE floatValue]];
}

- (void)ShowNotification:(NSString*)message {

//    //show notifications only in background
//    if([[UIApplication sharedApplication] applicationState] != UIApplicationStateBackground)
//        return;

    [[UIApplication sharedApplication] cancelAllLocalNotifications];
    [[UIApplication sharedApplication] setApplicationIconBadgeNumber: 0];

     UILocalNotification *notification = [[UILocalNotification alloc] init];

     notification.alertBody = message;
     notification.applicationIconBadgeNumber = 1;
     notification.soundName = UILocalNotificationDefaultSoundName;
     notification.hasAction = true;
     [[UIApplication sharedApplication] presentLocalNotificationNow:notification];
 }

// INTRODUCTION

- (void)showAlertWithTitle:(NSString*)title message:(NSString*)message image:(UIImage*)image buttonTitle:(NSString*)buttonTitle buttonAction:(void (^)(UIAlertAction*))buttonAction{
    UIAlertController * alert = [UIAlertController
                                 alertControllerWithTitle:title
                                 message:message
                                 preferredStyle:UIAlertControllerStyleAlert];

    UIAlertAction* alertImage = [UIAlertAction
                                actionWithTitle:@""
                                style:UIAlertActionStyleDefault
                                handler:buttonAction];
    [alertImage setValue:[image imageWithRenderingMode:UIImageRenderingModeAlwaysOriginal] forKey:@"image"];
    
    UIAlertAction* nextButton = [UIAlertAction
                                  actionWithTitle:buttonTitle
                                  style:UIAlertActionStyleDefault
                                  handler:buttonAction];
    
    [alert addAction:alertImage];
    [alert addAction:nextButton];
    
    [self.rootViewController presentViewController:alert animated:YES completion:nil];
}

- (UIImage*) scaleUIImage:(UIImage*)image toWidth:(float)desiredWidth{
    float realImageWidth = image.size.width;
    float scaleFactor = realImageWidth / desiredWidth;
    
    UIImage *scaledImage =
    [UIImage imageWithCGImage:[image CGImage]
                        scale:(image.scale * scaleFactor)
                  orientation:(image.imageOrientation)];
    
    return scaledImage;
}

- (UIImage*)getUIImageWithName:(NSString*)name{
    NSString* libraryDirectory = [[NSBundle mainBundle] resourcePath];
    NSString* imagePath = [NSString stringWithFormat:@"%@/%@/%@",libraryDirectory,@"Data/images",name];
    
    UIImage *image = [UIImage imageWithContentsOfFile:imagePath];
    
    return image;
}

- (void)StartIntroduction{
    
    float imageWidth = 250; // UIAlertController has width 250px on every device
    
    UIImage *mapImage = [self getUIImageWithName:@"map.jpg"];
    mapImage = [self scaleUIImage:mapImage toWidth:imageWidth];
    UIImage *mapImageAnimation = [UIImage animatedImageWithImages:@[mapImage] duration:2.5f];
    
    UIImage *point6before = [self getUIImageWithName:@"point_6_before.png"];
    point6before = [self scaleUIImage:point6before toWidth:imageWidth];
    UIImage *point6after = [self getUIImageWithName:@"point_6_after.png"];
    point6after = [self scaleUIImage:point6after toWidth:imageWidth];
    UIImage *pointsAnimation = [UIImage animatedImageWithImages:@[point6before,point6after] duration:2.5f];
    
    UIImage *cameraWindowImage = [self getUIImageWithName:@"camera_window_en.png"];
    cameraWindowImage = [self scaleUIImage:cameraWindowImage toWidth:imageWidth];
    UIImage *cameraWindowAnimation = [UIImage animatedImageWithImages:@[cameraWindowImage] duration:2.5f];
    
    UIImage *playStoreImage1 = [self getUIImageWithName:@"play_store_1.png"];
    playStoreImage1 = [self scaleUIImage:playStoreImage1 toWidth:imageWidth];
    UIImage *playStoreImage2 = [self getUIImageWithName:@"play_store_2.png"];
    playStoreImage2 = [self scaleUIImage:playStoreImage2 toWidth:imageWidth];
    UIImage *playStoreImages = [UIImage animatedImageWithImages:@[playStoreImage1,playStoreImage2] duration:2.5f];
    
    void (^goToFourthSection)(UIAlertAction*) = ^(UIAlertAction* action){
        [self showAlertWithTitle:@""
                         message: LocalizationData[@"intro_part4"]
                           image:playStoreImages
                     buttonTitle: LocalizationData[@"close"]
                    buttonAction:nil];
    };
    
    void (^goToThirdSection)(UIAlertAction*) = ^(UIAlertAction* action){
        [self showAlertWithTitle:@""
                message: LocalizationData[@"intro_part3"]
                image:cameraWindowAnimation
                buttonTitle: LocalizationData[@"next"]
                buttonAction:goToFourthSection];
    };
    
    void (^goToSecondSection)(UIAlertAction*) = ^(UIAlertAction* action){
        [self showAlertWithTitle:@""
                message: LocalizationData[@"intro_part2"]
                image:pointsAnimation
                buttonTitle: LocalizationData[@"next"]
                buttonAction:goToThirdSection];
    };
    
    [self showAlertWithTitle:@""
                message: LocalizationData[@"intro_part1"]
                image:mapImageAnimation
                buttonTitle: LocalizationData[@"next"]
                buttonAction:goToSecondSection];
}

- (void)LaunchIntroduction{
    UIAlertController * alert = [UIAlertController
                                 alertControllerWithTitle: LocalizationData[@"intro_title"]
                                 message: LocalizationData[@"intro_subtitle"]
                                 preferredStyle:UIAlertControllerStyleAlert];
    
    UIAlertAction* startButton = [UIAlertAction
                                  actionWithTitle: LocalizationData[@"start"]
                                  style:UIAlertActionStyleDefault
                                  handler:^(UIAlertAction * action) {
                                      [self StartIntroduction];
                                  }];
    
    UIAlertAction* closeButton = [UIAlertAction
                                 actionWithTitle: LocalizationData[@"close"]
                                 style:UIAlertActionStyleCancel
                                 handler:^(UIAlertAction * action) {
                                     
                                 }];
    [alert addAction:startButton];
    [alert addAction:closeButton];
    
    [self.rootViewController presentViewController:alert animated:YES completion:nil];
}

// SCREENSHOTS

- (void)thisImage:(UIImage *)image hasBeenSavedInPhotoAlbumWithError:(NSError *)error usingContextInfo:(void*)ctxInfo {
    if (error) {
        UnitySendMessage("PluginCallback", "ScreenshotErrorCapturing", "");
    } else {
        UnitySendMessage("PluginCallback", "ScreenshotCaptured", "");
    }
}

-(UIImage *)cropImage:(UIImage*)image withRect:(CGRect)rect
{
    CGImageRef subImage = CGImageCreateWithImageInRect(image.CGImage, rect);
    UIImage *croppedImage = [UIImage imageWithCGImage:subImage];
    CGImageRelease(subImage);
    return croppedImage;
}

-(void)shareImage:(UIImage *)image{
    NSArray * shareItems = @[image];
    UIActivityViewController * avc = [[UIActivityViewController alloc] initWithActivityItems:shareItems applicationActivities:nil];
    [self.rootViewController presentViewController:avc animated:YES completion:^(void){
        UnitySendMessage("PluginCallback", "ScreenshotCaptured", "");
    }];
};

-(void)saveImage:(UIImage*)image{
    if ([PHPhotoLibrary authorizationStatus] == PHAuthorizationStatusNotDetermined)
        [PHPhotoLibrary requestAuthorization:^(PHAuthorizationStatus status) {
            UIImageWriteToSavedPhotosAlbum(image, self, @selector(thisImage:hasBeenSavedInPhotoAlbumWithError:usingContextInfo:), nil);
        }];
    else
        UIImageWriteToSavedPhotosAlbum(image, self, @selector(thisImage:hasBeenSavedInPhotoAlbumWithError:usingContextInfo:), nil);
}

- (void)SaveScreenshotWithName:(const char*)filename bottomMargin:(float)bottom_margin topMargin:(float)top_margin orientation:(int)orientation{
    
    UIView* view = self.rootView;
    UIGraphicsBeginImageContext(view.bounds.size);
    [view drawViewHierarchyInRect:(view.bounds) afterScreenUpdates:TRUE];
    UIImage *image = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    
    // crop image
    image = [self cropImage:image withRect:CGRectMake(0, image.size.height - top_margin / [UIScreen mainScreen].scale, image.size.width, (top_margin - bottom_margin) / [UIScreen mainScreen].scale)];
    
    // set orientation
    UIImageOrientation desiredOrientation;
    switch(orientation){
        case 0: desiredOrientation = UIImageOrientationUp; break;
        case 1: desiredOrientation = UIImageOrientationUp; break;
        case 2: desiredOrientation = UIImageOrientationDown; break;
        case 3: desiredOrientation = UIImageOrientationLeft; break;
        case 4: desiredOrientation = UIImageOrientationRight; break;
        case 5: desiredOrientation = UIImageOrientationUp; break;
        case 6: desiredOrientation = UIImageOrientationUp; break;
    }
    image = [UIImage imageWithCGImage:[image CGImage] scale:[image scale] orientation: desiredOrientation]; // rotated image
    
    lastSharedImage = image;
    
    float scaleFactor = 1.5;
    if(lastSharedImage.imageOrientation == UIImageOrientationLeft || lastSharedImage.imageOrientation == UIImageOrientationRight)
        scaleFactor *= lastSharedImage.size.width / lastSharedImage.size.height;
    
    UIImage *scaledImage =
    [UIImage imageWithCGImage:[lastSharedImage CGImage]
                        scale:(lastSharedImage.scale * scaleFactor)
                  orientation:(lastSharedImage.imageOrientation)];
    
    UIAlertController * alert = [UIAlertController
                                 alertControllerWithTitle:LocalizationData[@"photo_captured"]
                                 message:@""
                                 preferredStyle:UIAlertControllerStyleAlert];

    UIAlertAction* alertImage = [UIAlertAction
                                actionWithTitle:@""
                                style:UIAlertActionStyleDefault
                                handler:^(UIAlertAction * action) {
                                    [self saveImage:image];
                                }];
    
    [alertImage setValue:[scaledImage imageWithRenderingMode:UIImageRenderingModeAlwaysOriginal] forKey:@"image"];
    
    UIAlertAction* shareButton = [UIAlertAction
                                  actionWithTitle: LocalizationData[@"share"]
                                  style:UIAlertActionStyleDefault
                                  handler:^(UIAlertAction * action) {
                                      [self shareImage:image];
                                  }];
    
    UIAlertAction* saveButton = [UIAlertAction
                                  actionWithTitle: LocalizationData[@"save"]
                                  style:UIAlertActionStyleDefault
                                  handler:^(UIAlertAction * action) {
                                      [self saveImage:image];
                                  }];
    
    [alert addAction:alertImage];
    [alert addAction:shareButton];
    [alert addAction:saveButton];
    
    [self.rootViewController presentViewController:alert animated:YES completion:nil];
}

@end

extern "C" void _StartTustanService(const char* _coords, const char* _marker_titles, const char* _notification_messages, const char* _separator, const char* _coords_separator, int cooldownInterval, float cooldownDistance, float notificationDistance, float acceptableAccuracy)
{
    NSString *separator = [NSString stringWithUTF8String:_separator];
    NSString *coords_separator = [NSString stringWithUTF8String: _coords_separator];
    notificationTitle = [[NSString stringWithUTF8String: _notification_messages] componentsSeparatedByString:separator][0];
    
    NSString *coords = [NSString stringWithUTF8String: _coords];
    NSString *marker_titles = [NSString stringWithUTF8String: _marker_titles];
    
    NSArray *coords_array = [[NSArray alloc] init];
    NSArray *marker_titles_array = [[NSArray alloc] init];
    coords_array = [coords componentsSeparatedByString:separator];
    marker_titles_array = [marker_titles componentsSeparatedByString:separator];
    
    globalMarkerLocations = [[NSMutableArray alloc] init];
    canSendNotifications = [[NSMutableArray alloc] init];
    maxDistance = [[NSMutableArray alloc] init];
    lastNotifTime = [[NSMutableArray alloc] init];
    
    for(int i=0; i<[coords_array count]; i++){
        NSString *curr_array = coords_array[i];
        NSArray *single_coords_array = [[NSArray alloc] init];
        single_coords_array = [curr_array componentsSeparatedByString:coords_separator];
        
        [globalMarkerLocations addObject:[[CLLocation alloc] initWithLatitude:[single_coords_array[0] floatValue] longitude:[single_coords_array[1] floatValue]]];
        [markerTitles addObject: marker_titles_array[i]];
        [canSendNotifications addObject:@YES];
        [lastNotifTime addObject:[NSNumber numberWithInteger:0]];
        [maxDistance addObject:COOLDOWN_DISTANCE];
    }
    
    COOLDOWN_INTERVAL = [NSNumber numberWithInteger: cooldownInterval];
    COOLDOWN_DISTANCE = [NSNumber numberWithFloat: cooldownDistance];
    NOTIFICATION_DISTANCE = [NSNumber numberWithFloat: notificationDistance];
    ACCEPTABLE_ACCURACY = [NSNumber numberWithFloat: acceptableAccuracy];
}

extern "C" {
    
    // SCREENSHOTS
    void _SaveScreenshot(const char* filename, float bottom_margin, float top_margin, int orientation){
        if(controller != nil)
            [controller SaveScreenshotWithName:filename bottomMargin:bottom_margin topMargin:top_margin orientation:orientation];
    }
    
    // START INDICATOR
    UIActivityIndicatorView *indicator;
    
    void _ShowProgressDialog(const char* message){
        UIView* view = UnityGetGLView();
        
        indicator = [[UIActivityIndicatorView alloc]initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleWhiteLarge];
        indicator.center = view.center;
        [view addSubview:indicator];
        indicator.hidesWhenStopped = YES;
        [indicator startAnimating];
    }
    
    void _HideProgressDialog(){
        [indicator stopAnimating];
    }
}

extern "C" float _GetFontScale(){
    return 1;
}

extern "C" void _GoToGPSSettings(){
    [[UIApplication sharedApplication] openURL:[NSURL URLWithString:@"App-Prefs:root=Privacy&path=LOCATION"] options:@{} completionHandler:nil];
}

extern "C" void _LaunchIntroduction(){
    if(controller != nil)
        [controller LaunchIntroduction];
}

extern "C" void _LoadLocalizedText(const char* jsonText){
    NSString* jsonString = [NSString stringWithUTF8String:jsonText];
    
    NSError *jsonError;
    NSData* jsonData = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSDictionary *jsonObject = [NSJSONSerialization JSONObjectWithData:jsonData options:kNilOptions error:&jsonError];
    
    NSMutableArray* items = [jsonObject objectForKey:@"items"];
    for(int i=0; i<items.count; i++){
        NSDictionary* localizationItem = items[i];
        LocalizationData[[localizationItem objectForKey:@"key"]] = [localizationItem objectForKey:@"value"];
    }
}

IMPL_APP_CONTROLLER_SUBCLASS(BackgroundController)

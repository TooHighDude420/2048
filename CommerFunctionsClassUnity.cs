using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


   public class CommerFunctionsClassUnity : MonoBehaviour
   {
       private const string BannerTestId = "ca-app-pub-3940256099942544/6300978111";
       private const string full_ad_id = "ca-app-pub-3940256099942544/1033173712";
       private bool ads_Init;
       public BannerView banner;
       public InterstitialAd interstitial;
       Action<InitializationStatus> initStatus;

       //common ad functions
       public IEnumerator initAds()
       {
           MobileAds.Initialize(initStatus => { ads_Init = true; });
           while (!ads_Init)
           {
               yield return null;
           }
       }
       //functions for full ads
       public void LoadInterstitialAd()
       {
           // Clean up the old ad before loading a new one.
           if (interstitial != null)
           {
               interstitial.Destroy();
               interstitial = null;
           }
        
        Debug.Log("Loading the interstitial ad.");

            // create our request used to load the ad.
            var adRequest = new AdRequest();

            // send the request to load the ad.
            InterstitialAd.Load(full_ad_id, adRequest,
                (InterstitialAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        Debug.LogError("interstitial ad failed to load an ad " +
                                       "with error : " + error);
                        return;
                    }

                    Debug.Log("Interstitial ad loaded with response : "
                              + ad.GetResponseInfo());

                    interstitial = ad;
                });
        }
        public void ShowInterstitialAd()
        {
            if (interstitial != null && interstitial.CanShowAd())
            {
                Debug.Log("Showing interstitial ad.");
                interstitial.Show();
            }
            else
            {
                Debug.LogError("Interstitial ad is not ready yet.");
            }
        }
        public void RegisterEventHandlers(InterstitialAd interstitialAd)
        {
            // Raised when the ad is estimated to have earned money.
            interstitialAd.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            interstitialAd.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Interstitial ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            interstitialAd.OnAdClicked += () =>
            {
                Debug.Log("Interstitial ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            interstitialAd.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Interstitial ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial ad full screen content closed.");
            };
            // Raised when the ad failed to open full screen content.
            interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Interstitial ad failed to open full screen content " +
                               "with error : " + error);
            };
        }
        public void RegisterReloadHandler(InterstitialAd interstitialAd)
        {
            // Raised when the ad closed full screen content.
            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial Ad full screen content closed.");

                // Reload the ad so that we can show another as soon as possible.
                LoadInterstitialAd();
            };
            // Raised when the ad failed to open full screen content.
            interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Interstitial ad failed to open full screen content " +
                               "with error : " + error);

                // Reload the ad so that we can show another as soon as possible.
                LoadInterstitialAd();
            };
        }
        public void DoneWithFullAd()
        {
            if (interstitial != null)
            {
                interstitial.Destroy();
                interstitial = null;
            }

        }

        //functions for banner ads
        public void ListenToAdEvents()
        {
            // Raised when an ad is loaded into the banner view.
            banner.OnBannerAdLoaded += () =>
            {
                Debug.Log("Banner view loaded an ad with response : "
                    + banner.GetResponseInfo());
            };
            // Raised when an ad fails to load into the banner view.
            banner.OnBannerAdLoadFailed += (LoadAdError error) =>
            {
                Debug.LogError("Banner view failed to load an ad with error : "
                    + error);
            };
            // Raised when the ad is estimated to have earned money.
            banner.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Banner view paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            banner.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Banner view recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            banner.OnAdClicked += () =>
            {
                Debug.Log("Banner view was clicked.");
            };
            // Raised when an ad opened full screen content.
            banner.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Banner view full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            banner.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Banner view full screen content closed.");
            };
        }
        public void CreateBannerAd()
        {
            Debug.Log("Creating banner view");

            // If we already have a banner, destroy the old one.
            if (banner != null)
            {
                banner.Destroy();
                banner = null;
            }
            // Create a 320x50 banner at the bottom of the screen
            banner = new BannerView(BannerTestId, AdSize.Banner, AdPosition.Bottom);

        }
        public void LoadAd()
        {
            // create an instance of a banner view first.
            if (banner == null)
            {
                CreateBannerAd();
            }

            // create our request used to load the ad.
            var adRequest = new AdRequest();

            // send the request to load the ad.
            Debug.Log("Loading banner ad.");
            banner.LoadAd(adRequest);
        }
   }


using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using Twitter;

public class TwitterAPIComponent : MonoBehaviour
{
  private static readonly string PREFS_TWITTER_ACCESS_KEY = "twitter-access-data";//"TwitterAccessToken";
  private static readonly int PIN_LENGTH = 7;

  public string twitterSecretJsonPath = "Data/twitter.secret";

  public FileImageComponent fileImageComponent;
  public Text tweetText;
  public GameObject pinInputObject;

  private string consumerKey;
  private string consumerSecret;


  private string pin;
  private TwitterRequestToken requestToken;
  private TwitterAccessToken accessToken;
  private TwitterClient twitterClient;

  private bool isPromptingForPin = false;

  void Start()
  {
    LoadStoredAccessToken();
    TextAsset twitterSecret = CachedResource.Load<TextAsset>(twitterSecretJsonPath);
    string[] values = twitterSecret.text.Split('|');
    consumerKey = values[0];
    consumerSecret = values[1];

    if (String.IsNullOrEmpty(consumerKey) || String.IsNullOrEmpty(consumerSecret))
    {
      throw new Exception("CONSUMER KEY/SECRET ARE EMPTY");
    }

    twitterClient = new TwitterClient(consumerKey, consumerSecret);
  }

  void OnGUI()
  {
    if (pinInputObject != null && pinInputObject.activeSelf)
    {
      pin = pinInputObject.GetComponentInChildren<Text>().text;      
    }
  }

  public void ShareImage()
  {
    if (accessToken == null && !isPromptingForPin)
    {
      StartCoroutine(twitterClient.GenerateRequestToken(ProcessRequestTokenResponse));
    }
    else if (isPromptingForPin && hasPinEntered())
    {
      StartCoroutine(twitterClient.GenerateAccessToken(requestToken.oauth_token, pin, ProcessAccessTokenResponse));
    }
    else
    {
      SubmitTweetWithImage();
    }
  }

  private void ProcessRequestTokenResponse(bool success, string responseJson)
  {
    Logger.Log("Custom Request Token Response", responseJson);
    if (success)
    {
      requestToken = JsonUtility.FromJson<Twitter.TwitterRequestToken>(responseJson);
      Logger.Log(requestToken.oauth_token, requestToken.oauth_token_secret, requestToken.oauth_callback_confirmed);
      Application.OpenURL(twitterClient.GetAppAuthorizationUrl(requestToken.oauth_token));

      /// TODO: Prompt For PIN
      pinInputObject.SetActive(true);
      isPromptingForPin = true;
    }
    else
    {
      Logger.Log("Error while getting RequestToken");
    }
  }
  private void ProcessAccessTokenResponse(bool success, string response)
  {
    if (success)
    {
      Logger.Log("Access Token Response: ", response);
      var token = JsonUtility.FromJson<TwitterRawAccessToken>(response);
      accessToken = new TwitterAccessToken(
        token.oauth_token,
        token.oauth_token_secret,
        token.user_id,
        token.screen_name
      );
      Logger.Log("Token", accessToken);

      StoreAccessToken(accessToken);
      SubmitTweetWithImage();      
    }
    else
    {
      Logger.Log("FAILED", response);
    }
  }

  private void SubmitTweetWithImage()
  {
    Logger.Log("Sending Tweet with image ", fileImageComponent.filePath);
    byte[] imgBinary = File.ReadAllBytes(fileImageComponent.filePath);
    string imgbase64 = System.Convert.ToBase64String(imgBinary);
    StartCoroutine(twitterClient.PostTweetWithMedia(
      tweetText.text,
      imgbase64,
      accessToken.Token,
      accessToken.TokenSecret,
      this.OnTweetPosted));
  }

  private void OnTweetPosted(bool success, string response)
  {
    if (success)
    {
      Logger.Log("SUCCESS", response);
    }
    else
    {
      Logger.Log("FAILED", response);
    }
  }

  private bool hasPinEntered()
  {
    return pinInputObject != null && pin != null && pin.Length == PIN_LENGTH;
  }

  private void LoadStoredAccessToken()
  {
    string accessDetails = PlayerPrefs.GetString(PREFS_TWITTER_ACCESS_KEY);
    Logger.Log("Found String is: ", accessDetails);

    accessToken = TwitterAccessToken.FromString(accessDetails);
    if(accessToken == null ||
       string.IsNullOrEmpty(accessToken.Token) ||
       string.IsNullOrEmpty(accessToken.TokenSecret) ||
       string.IsNullOrEmpty(accessToken.UserId) ||
       string.IsNullOrEmpty(accessToken.ScreenName)){
         Logger.Log("Stored Access Token has Empty Values. Discarding");
         accessToken = null;
       }
  }

  private void StoreAccessToken(TwitterAccessToken token)
  {
    PlayerPrefs.SetString(PREFS_TWITTER_ACCESS_KEY, token.ToString());
  }


}
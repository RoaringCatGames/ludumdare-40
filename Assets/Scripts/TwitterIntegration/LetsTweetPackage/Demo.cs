
using UnityEngine;
using System.Collections;
using System.IO;

public class Demo : MonoBehaviour
{
  public float USER_LOG_IN_X;
  public float USER_LOG_IN_Y;
  public float USER_LOG_IN_WIDTH;
  public float USER_LOG_IN_HEIGHT;

  public float PIN_INPUT_X;
  public float PIN_INPUT_Y;
  public float PIN_INPUT_WIDTH;
  public float PIN_INPUT_HEIGHT;

  public float PIN_ENTER_X;
  public float PIN_ENTER_Y;
  public float PIN_ENTER_WIDTH;
  public float PIN_ENTER_HEIGHT;

  public float TWEET_INPUT_X;
  public float TWEET_INPUT_Y;
  public float TWEET_INPUT_WIDTH;
  public float TWEET_INPUT_HEIGHT;

  public float POST_TWEET_X;
  public float POST_TWEET_Y;
  public float POST_TWEET_WIDTH;
  public float POST_TWEET_HEIGHT;

  // You need to register your game or application in Twitter to get cosumer key and secret.
  // Go to this page for registration: http://dev.twitter.com/apps/new
  public string CONSUMER_KEY;
  public string CONSUMER_SECRET;

  // You need to save access token and secret for later use.
  // You can keep using them whenever you need to access the user's Twitter account. 
  // They will be always valid until the user revokes the access to your application.

  const string TWITTER_ACCESS_TOKEN_KEY = "TwitterAccessToken";
  // const string PLAYER_PREFS_TWITTER_USER_ID           = "TwitterUserID";
  // const string PLAYER_PREFS_TWITTER_USER_SCREEN_NAME  = "TwitterUserScreenName";
  // const string PLAYER_PREFS_TWITTER_USER_TOKEN        = "TwitterUserToken";
  // const string PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET = "TwitterUserTokenSecret";

  Twitter.TwitterRequestToken twitterRequestToken;
  Twitter.TwitterAccessToken twitterAccessToken;

  string m_PIN = "Please enter your PIN here.";
  string m_Tweet = "Please enter your tweet here.";

  private Twitter.TwitterClient twitterClient;

  // Use this for initialization
  void Start()
  {
    //PlayerPrefs.DeleteAll();
    LoadTwitterUserInfo();

    twitterClient = new Twitter.TwitterClient(CONSUMER_KEY, CONSUMER_SECRET);
  }

  // Update is called once per frame
  void Update()
  {
  }

  // GUI
  void OnGUI()
  {
    // LogIn/Register Button
    Rect rect = new Rect(Screen.width * USER_LOG_IN_X,
                         Screen.height * USER_LOG_IN_Y,
                         Screen.width * USER_LOG_IN_WIDTH,
                         Screen.height * USER_LOG_IN_HEIGHT);

    if (string.IsNullOrEmpty(CONSUMER_KEY) || string.IsNullOrEmpty(CONSUMER_SECRET))
    {
      string text = "You need to register your game or application first.\n Click this button, register and fill CONSUMER_KEY and CONSUMER_SECRET of Demo game object.";
      if (GUI.Button(rect, text))
      {
        Application.OpenURL("http://dev.twitter.com/apps/new");
      }
    }
    else
    {
      string text = string.Empty;

      if (!string.IsNullOrEmpty(twitterAccessToken.ScreenName))
      {
        text = twitterAccessToken.ScreenName + "\nClick to register with a different Twitter account";
      }

      else
      {
        text = "You need to register your game or application first.";
      }

      if (GUI.Button(rect, text))
      {

        StartCoroutine(twitterClient.GenerateRequestToken(this.OnRequestTokenCallbackNew));
      }
    }

    // PIN Input
    rect.x = Screen.width * PIN_INPUT_X;
    rect.y = Screen.height * PIN_INPUT_Y;
    rect.width = Screen.width * PIN_INPUT_WIDTH;
    rect.height = Screen.height * PIN_INPUT_HEIGHT;

    m_PIN = GUI.TextField(rect, m_PIN);

    // PIN Enter Button
    rect.x = Screen.width * PIN_ENTER_X;
    rect.y = Screen.height * PIN_ENTER_Y;
    rect.width = Screen.width * PIN_ENTER_WIDTH;
    rect.height = Screen.height * PIN_ENTER_HEIGHT;

    if (GUI.Button(rect, "Enter PIN"))
    {
      StartCoroutine(twitterClient.GenerateAccessToken(twitterRequestToken.oauth_token, m_PIN, this.OnAccessTokenCallbackNew));
      // StartCoroutine(Twitter.API.GetAccessToken(CONSUMER_KEY, CONSUMER_SECRET, m_RequestTokenResponse.oauth_token, m_PIN,
      //                new Twitter.AccessTokenCallback(this.OnAccessTokenCallback)));
    }

    // Tweet Input
    rect.x = Screen.width * TWEET_INPUT_X;
    rect.y = Screen.height * TWEET_INPUT_Y;
    rect.width = Screen.width * TWEET_INPUT_WIDTH;
    rect.height = Screen.height * TWEET_INPUT_HEIGHT;

    m_Tweet = GUI.TextField(rect, m_Tweet);

    // Post Tweet Button
    rect.x = Screen.width * POST_TWEET_X;
    rect.y = Screen.height * POST_TWEET_Y;
    rect.width = Screen.width * POST_TWEET_WIDTH;
    rect.height = Screen.height * POST_TWEET_HEIGHT;

    if (GUI.Button(rect, "Post Tweet"))
    {

      byte[] imgBinary = File.ReadAllBytes("calligratree-201712171756375.png");
      string imgbase64 = System.Convert.ToBase64String(imgBinary);
      StartCoroutine(twitterClient.PostTweetWithMedia(
          "Have a Cherry Merry Christams From RCG! #calligratree #testingFromUnity", 
          imgbase64, 
          twitterAccessToken.Token, 
          twitterAccessToken.TokenSecret,
          this.OnPostTweetNew));
      //StartCoroutine(twitterClient.PostTweet(m_Tweet, null, twitterAccessToken.Token, twitterAccessToken.TokenSecret, this.OnPostTweetNew));

      // StartCoroutine(Twitter.API.PostTweet(m_Tweet, CONSUMER_KEY, CONSUMER_SECRET, m_AccessTokenResponse,
      //                new Twitter.PostTweetCallback(this.OnPostTweet)));
    }
  }


  void LoadTwitterUserInfo()
  {
    string storedAccessToken = PlayerPrefs.GetString(TWITTER_ACCESS_TOKEN_KEY);
    if (string.IsNullOrEmpty(storedAccessToken))
    {
      twitterAccessToken = new Twitter.TwitterAccessToken();
    }
    else
    {
      twitterAccessToken = Twitter.TwitterAccessToken.FromString(storedAccessToken);
    }


    // m_AccessTokenResponse.UserId        = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_ID);
    // m_AccessTokenResponse.ScreenName    = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_SCREEN_NAME);
    // m_AccessTokenResponse.Token         = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_TOKEN);
    // m_AccessTokenResponse.TokenSecret   = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET);

    if (!string.IsNullOrEmpty(twitterAccessToken.Token) &&
        !string.IsNullOrEmpty(twitterAccessToken.ScreenName) &&
        !string.IsNullOrEmpty(twitterAccessToken.Token) &&
        !string.IsNullOrEmpty(twitterAccessToken.TokenSecret))
    {
      string log = "LoadTwitterUserInfo - succeeded";
      log += "\n    UserId : " + twitterAccessToken.UserId;
      log += "\n    ScreenName : " + twitterAccessToken.ScreenName;
      log += "\n    Token : " + twitterAccessToken.Token;
      log += "\n    TokenSecret : " + twitterAccessToken.TokenSecret;
      print(log);
    }
  }

  // void OnRequestTokenCallback(bool success, Twitter.RequestTokenResponse response)
  // {
  //     if (success)
  //     {
  //         string log = "OnRequestTokenCallback - succeeded";
  //         log += "\n    Token : " + response.Token;
  //         log += "\n    TokenSecret : " + response.TokenSecret;
  //         print(log);

  //         m_RequestTokenResponse = new Twitter.TwitterRequestToken(){
  //             oauth_token = response.Token,
  //             oauth_token_secret = response.TokenSecret,
  //             oauth_callback_confirmed = true
  //         };

  //         Twitter.API.OpenAuthorizationPage(response.Token);
  //     }
  //     else
  //     {
  //         print("OnRequestTokenCallback - failed.");
  //     }
  // }

  void OnRequestTokenCallbackNew(bool success, string responseJson)
  {
    Logger.Log("Custom Request Token Response", responseJson);
    if (success)
    {
      twitterRequestToken = JsonUtility.FromJson<Twitter.TwitterRequestToken>(responseJson);
      Logger.Log(twitterRequestToken.oauth_token, twitterRequestToken.oauth_token_secret, twitterRequestToken.oauth_callback_confirmed);
      Application.OpenURL(twitterClient.GetAppAuthorizationUrl(twitterRequestToken.oauth_token));
    }
    else
    {
      Logger.Log("Error while getting RequestToken");
    }
  }

  void OnAccessTokenCallbackNew(bool success, string responseJson)
  {
    if (success)
    {
      var rawResponse = JsonUtility.FromJson<Twitter.TwitterRawAccessToken>(responseJson);
      Logger.Log("Response JSON for Access Token: ", responseJson);
      twitterAccessToken = new Twitter.TwitterAccessToken(
          rawResponse.oauth_token,
          rawResponse.oauth_token_secret,
          rawResponse.user_id,
          rawResponse.screen_name);
      PlayerPrefs.SetString(TWITTER_ACCESS_TOKEN_KEY, twitterAccessToken.ToString());
    }
    else
    {
      Logger.Log("Error On AccessToken Callback", responseJson);
    }
  }
  // void OnAccessTokenCallback(bool success, Twitter.AccessTokenResponse response)
  // {
  //     if (success)
  //     {
  //         string log = "OnAccessTokenCallback - succeeded";
  //         log += "\n    UserId : " + response.UserId;
  //         log += "\n    ScreenName : " + response.ScreenName;
  //         log += "\n    Token : " + response.Token;
  //         log += "\n    TokenSecret : " + response.TokenSecret;
  //         print(log);

  //         m_AccessTokenResponse = response;

  //         PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_ID, response.UserId);
  //         PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_SCREEN_NAME, response.ScreenName);
  //         PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_TOKEN, response.Token);
  //         PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET, response.TokenSecret);
  //     }
  //     else
  //     {
  //         print("OnAccessTokenCallback - failed.");
  //     }
  // }

  void OnPostTweetNew(bool success, string response)
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
  void OnPostTweet(bool success)
  {
    print("OnPostTweet - " + (success ? "succedded." : "failed."));
  }
}

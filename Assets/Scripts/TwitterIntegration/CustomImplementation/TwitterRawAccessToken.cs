using System;

namespace Twitter {
  
  [Serializable]
  public class TwitterRawAccessToken {

    public string oauth_token;
    public string oauth_token_secret;
    public string user_id;
    public string screen_name;
  }
}
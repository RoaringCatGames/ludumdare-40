using System;

namespace Twitter {

  [System.Serializable]
  public class TwitterRequestToken {
    public string oauth_token;
    public string oauth_token_secret;
    public bool oauth_callback_confirmed;
  }
}
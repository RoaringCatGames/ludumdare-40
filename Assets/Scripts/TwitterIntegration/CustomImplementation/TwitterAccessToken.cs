using System;

namespace Twitter
{
  public class TwitterAccessToken
  {
    private static readonly string DELIMITTER = "|";
    public static readonly int USER_ID_POS = 0;
    public static readonly int SCREEN_NAME_POS = 1;
    public static readonly int TOKEN_POS = 2;
    public static readonly int TOKEN_SECRET_POS = 3;

    public TwitterAccessToken() { }
    public TwitterAccessToken(string token, string tokenSecret, string userId, string screenName)
    {
      this.Token = token;
      this.TokenSecret = tokenSecret;
      this.UserId = userId;
      this.ScreenName = screenName;
    }

    public string Token { get; set; }
    public string TokenSecret { get; set; }
    public string UserId { get; set; }
    public string ScreenName { get; set; }


    public override string ToString()
    {
      return String.Concat(UserId, DELIMITTER, ScreenName, DELIMITTER, Token, DELIMITTER, TokenSecret);
    }

    public static TwitterAccessToken FromString(string input)
    {
      if (!String.IsNullOrEmpty(input))
      {

        string[] details = input.Split(DELIMITTER.ToCharArray());
        return new TwitterAccessToken(
          details[TOKEN_POS],
          details[TOKEN_SECRET_POS],
          details[USER_ID_POS],
          details[SCREEN_NAME_POS]
        );
      }
      else
      {
        return null;
      }
    }
  }
}
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace Twitter
{

  public delegate void TwitterRequestCallback(bool success, string response);

  public class TwitterClient
  {
    private static readonly int URI_ESCAPE_LIMIT = 32766;
    private static readonly string REQUEST_TOKEN_URL = "https://api.twitter.com/oauth/request_token";
    private static readonly string APP_AUTHORIZATION_URL = "https://api.twitter.com/oauth/authenticate?oauth_token={0}";
    private static readonly string ACCESS_TOKEN_URL = "https://api.twitter.com/oauth/access_token";
    private static readonly string POST_TWEET_URL = "https://api.twitter.com/1.1/statuses/update.json";
    private static readonly string UPLOAD_MEDIA_URL = "https://upload.twitter.com/1.1/media/upload.json";


    private string _consumerKey;
    private string _consumerSecret;

    public TwitterClient(string consumerKey, string consumerSecret)
    {
      this._consumerKey = consumerKey;
      this._consumerSecret = consumerSecret;
    }

    /**
      Sends a request to request_token API endpoint to get a token
        OAuth realm="Twitter API",
          oauth_consumer_key="pzHZkONahyeMyuzsALpt6Nyn5",
          oauth_signature_method="HMAC-SHA1",
          oauth_timestamp="1513742652",
          oauth_version="1.0",
          oauth_signature="mDSm6c94%2F1YmeiSYIRQlNfRQfMc%3D"
    */
    public IEnumerator GenerateRequestToken(TwitterRequestCallback callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("oauth_callback", "oob");
      var sortedParameters = _generateParameters(parameters);

      WWWForm form = new WWWForm();
      form.AddField("oauth_callback", "oob");

      UnityWebRequest request = UnityWebRequest.Post(REQUEST_TOKEN_URL, form);
      string authHeader = _generateRequestTokenAuthHeader("POST", REQUEST_TOKEN_URL, sortedParameters);
      request.SetRequestHeader("Authorization", authHeader);

      return SubmitRequest(request, true, callback);
    }

    public string GetAppAuthorizationUrl(string requestToken){
      return string.Format(APP_AUTHORIZATION_URL, requestToken);
    }

    public IEnumerator GenerateAccessToken(string requestToken, string pin, TwitterRequestCallback callback){
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("oauth_token", requestToken);
      parameters.Add("oauth_verifier", pin);
      var sortedParameters = _generateParameters(parameters);

      UnityWebRequest request = UnityWebRequest.Post(ACCESS_TOKEN_URL, "");
      string authHeader = _generateRequestTokenAuthHeader("POST", ACCESS_TOKEN_URL, sortedParameters);
      request.SetRequestHeader("Authorization", authHeader);

      return SubmitRequest(request, true, callback);
    }

    public IEnumerator PostTweet(string message, string mediaIds, string accessToken, string accessTokenSecret, TwitterRequestCallback callback){
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("oauth_token", accessToken);
      parameters.Add("oauth_token_secret", accessTokenSecret);
      parameters.Add("status", message);

      WWWForm form = new WWWForm();
      form.AddField("status", message);

      Logger.Log("Status Message", message, mediaIds);

      if(mediaIds != null){
        parameters.Add("media_ids", mediaIds);
        form.AddField("media_ids", mediaIds);
      }
      var sortedParams = _generateParameters(parameters);          

      UnityWebRequest req = UnityWebRequest.Post(POST_TWEET_URL, form);
      req.SetRequestHeader("Authorization", _generateRequestTokenAuthHeader("POST", POST_TWEET_URL, sortedParams));

      return SubmitRequest(req, false, callback);
    }

    public IEnumerator PostTweetWithMedia(string message, string base64ImageData, string accessToken, string accessTokenSecret, TwitterRequestCallback callback){
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("oauth_token", accessToken);
      parameters.Add("oauth_token_secret", accessTokenSecret);
      // parameters.Add("media_data", base64ImageData);
      var sortedParams = _generateParameters(parameters);   

      WWWForm form = new WWWForm();
      form.AddBinaryData("media", Convert.FromBase64String(base64ImageData), "", "");

      UnityWebRequest req = UnityWebRequest.Post(UPLOAD_MEDIA_URL, form);
      var authHeader = _generateRequestTokenAuthHeader("POST", UPLOAD_MEDIA_URL, sortedParams);
      Logger.Log("Auth Header for Media: ", authHeader);
      req.SetRequestHeader("Authorization", authHeader);   
      //req.SetRequestHeader("Content-Type", "multipart/form-data");   

      yield return req.SendWebRequest();
      
      if(req.isNetworkError){
        callback(false, JsonResponseUtil.ArrayToObject(req.error));
      }else {
        Logger.Log("Media Upload Response JSON: ", req.downloadHandler.text);
        var mediaUpload = JsonUtility.FromJson<Twitter.TwitterMediaUploadResponse>(req.downloadHandler.text);
        yield return PostTweet(message, mediaUpload.media_id.ToString(), accessToken, accessTokenSecret, callback);
      }      
    }


    private IEnumerator SubmitRequest(UnityWebRequest request, bool shouldParseFromFormData, TwitterRequestCallback callback)
    {
      yield return request.SendWebRequest();

  
      if (request.isNetworkError)
      {        
        callback(false, JsonResponseUtil.ArrayToObject(request.error));
      }
      else
      {
        string json = request.downloadHandler.text;
        if(shouldParseFromFormData){
          json = JsonResponseUtil.FormDataToJson(json);
        }
        json = JsonResponseUtil.ArrayToObject(json);
        bool isSuccessful = (request.responseCode == 200 || request.responseCode == 201);

        callback(isSuccessful, json);
      }
    }

    private SortedDictionary<string, string> _generateParameters(Dictionary<string, string> inParams)
    {
      SortedDictionary<string, string> sortedParams = new SortedDictionary<string, string>(){
        { "oauth_consumer_key", _consumerKey },
        { "oauth_consumer_secret", _consumerSecret },
        { "oauth_nonce", GenerateNonce() },
        { "oauth_signature_method", "HMAC-SHA1" },
        { "oauth_timestamp", GenerateTimeStamp() },
        { "oauth_version", "1.0" }
      };

      foreach (KeyValuePair<string, string> kvp in inParams)
      {
        sortedParams.Add(kvp.Key, kvp.Value);
      }

      return sortedParams;
    }

    // The below help methods are modified from "WebRequestBuilder.cs" in Twitterizer(http://www.twitterizer.net/).
    // Here is its license.
    //-----------------------------------------------------------------------
    // <copyright file="WebRequestBuilder.cs" company="Patrick 'Ricky' Smith">
    //  This file is part of the Twitterizer library (http://www.twitterizer.net/)
    // 
    //  Copyright (c) 2010, Patrick "Ricky" Smith (ricky@digitally-born.com)
    //  All rights reserved.
    //  
    //  Redistribution and use in source and binary forms, with or without modification, are 
    //  permitted provided that the following conditions are met:
    // 
    //  - Redistributions of source code must retain the above copyright notice, this list 
    //    of conditions and the following disclaimer.
    //  - Redistributions in binary form must reproduce the above copyright notice, this list 
    //    of conditions and the following disclaimer in the documentation and/or other 
    //    materials provided with the distribution.
    //  - Neither the name of the Twitterizer nor the names of its contributors may be 
    //    used to endorse or promote products derived from this software without specific 
    //    prior written permission.
    // 
    //  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
    //  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
    //  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
    //  IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
    //  INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
    //  NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR 
    //  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
    //  WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
    //  ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
    //  POSSIBILITY OF SUCH DAMAGE.
    // </copyright>
    // <author>Ricky Smith</author>
    // <summary>Provides the means of preparing and executing Anonymous and OAuth signed web requests.</summary>
    //-----------------------------------------------------------------------

    private string _generateRequestTokenAuthHeader(string method, string url, SortedDictionary<string, string> parameters)
    {
      // Add the signature to the oauth parameters
      string signature = GenerateSignature(method, url, parameters);

      parameters.Add("oauth_signature", signature);

      StringBuilder authHeaderBuilder = new StringBuilder();
      authHeaderBuilder.AppendFormat("OAuth realm=\"{0}\"", "Twitter API");

      var sortedParameters = from p in parameters
                             where OAuthParametersToIncludeInHeader.Contains(p.Key)
                             orderby p.Key, p.Value
                             select p;

      foreach (var item in sortedParameters)
      {       
        var encodedKey = UrlEncode(item.Key);
        var encodedValue = item.Key.ToLower() != "media_data" ? UrlEncode(item.Value) : item.Value;
        authHeaderBuilder.AppendFormat(",{0}=\"{1}\"", encodedKey, encodedValue);
      }

      authHeaderBuilder.AppendFormat(",oauth_signature=\"{0}\"", UrlEncode(parameters["oauth_signature"]));

      return authHeaderBuilder.ToString();
    }


    private readonly string[] OAuthParametersToIncludeInHeader = new[]
    {
      "oauth_version",
      "oauth_nonce",
      "oauth_timestamp",
      "oauth_signature_method",
      "oauth_consumer_key",
      "oauth_token",
      "oauth_verifier"
      // Leave signature omitted from the list, it is added manually
      // "oauth_signature",
    };

    private readonly string[] SecretParameters = new[]
    {
      "oauth_consumer_secret",
      "oauth_token_secret",
      "oauth_signature"
    };
    private string GenerateSignature(string httpMethod, string url, SortedDictionary<string, string> parameters)
    {
      var nonSecretParameters = (from p in parameters
                                 where !SecretParameters.Contains(p.Key)
                                 select p);

      // Create the base string. This is the string that will be hashed for the signature.
      string signatureBaseString = string.Format(CultureInfo.InvariantCulture,
                                                 "{0}&{1}&{2}",
                                                 httpMethod,
                                                 UrlEncode(NormalizeUrl(new Uri(url))),
                                                 UrlEncode(nonSecretParameters));

      // Create our hash key (you might say this is a password)
      string key = string.Format(CultureInfo.InvariantCulture,
                                 "{0}&{1}",
                                 UrlEncode(parameters["oauth_consumer_secret"]),
                                 parameters.ContainsKey("oauth_token_secret") ? UrlEncode(parameters["oauth_token_secret"]) : string.Empty);


      // Generate the hash
      HMACSHA1 hmacsha1 = new HMACSHA1(Encoding.ASCII.GetBytes(key));
      byte[] signatureBytes = hmacsha1.ComputeHash(Encoding.ASCII.GetBytes(signatureBaseString));
      return Convert.ToBase64String(signatureBytes);
    }

    private string NormalizeUrl(Uri url)
    {
      string normalizedUrl = string.Format(CultureInfo.InvariantCulture, "{0}://{1}", url.Scheme, url.Host);
      if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
      {
        normalizedUrl += ":" + url.Port;
      }

      normalizedUrl += url.AbsolutePath;
      return normalizedUrl;
    }
    private string GenerateTimeStamp()
    {
      TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
      return Convert.ToInt64(ts.TotalSeconds).ToString();
    }

    private string GenerateNonce()
    {
      return new System.Random().Next(123400, int.MaxValue).ToString("X");
    }

    private string UrlEncode(string value)
    {
      if (string.IsNullOrEmpty(value))
      {
        return string.Empty;
      }

      
      value = EscapeDataStringLong(value); //Uri.EscapeDataString(value);

      // UrlEncode escapes with lowercase characters (e.g. %2f) but oAuth needs %2F
      value = Regex.Replace(value, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());

      // these characters are not escaped by UrlEncode() but needed to be escaped
      value = value
          .Replace("(", "%28")
          .Replace(")", "%29")
          .Replace("$", "%24")
          .Replace("!", "%21")
          .Replace("*", "%2A")
          .Replace("'", "%27");

      // these characters are escaped by UrlEncode() but will fail if unescaped!
      value = value.Replace("%7E", "~");

      return value;
    }

    private string EscapeDataStringLong(string value){
      //32766

      if(value.Length < URI_ESCAPE_LIMIT){
        return Uri.EscapeDataString(value);
      }else{
        StringBuilder sb = new StringBuilder();
        int loops = value.Length / URI_ESCAPE_LIMIT;

        for (int i = 0; i <= loops; i++)
        {
            if (i < loops)
            {
                sb.Append(Uri.EscapeDataString(value.Substring(URI_ESCAPE_LIMIT * i, URI_ESCAPE_LIMIT)));
            }
            else
            {
                sb.Append(Uri.EscapeDataString(value.Substring(URI_ESCAPE_LIMIT * i)));
            }
        }

        return sb.ToString();
      }
    }

    private string UrlEncode(IEnumerable<KeyValuePair<string, string>> parameters)
    {
      StringBuilder parameterString = new StringBuilder();

      var paramsSorted = from p in parameters
                         orderby p.Key, p.Value
                         select p;

      foreach (var item in paramsSorted)
      {
        if (parameterString.Length > 0)
        {
          parameterString.Append("&");
        }
        
        var encodedKey = UrlEncode(item.Key);
        var encodedValue = UrlEncode(item.Value);
        parameterString.Append(
            string.Format(
                CultureInfo.InvariantCulture,
                "{0}={1}",
                encodedKey,
                encodedValue));
      }

      return UrlEncode(parameterString.ToString());
    }

  }
}
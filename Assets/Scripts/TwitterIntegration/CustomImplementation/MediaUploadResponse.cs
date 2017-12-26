using System;

namespace Twitter {

  [Serializable]
  public class TwitterMediaUploadResponse {
    public long media_id;
    public string media_id_string;
    public int size;
    public int expires_after_secs;
  }
}
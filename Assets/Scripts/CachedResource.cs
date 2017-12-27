using System.Collections.Generic;
using System.IO;
using UnityEngine;

/**
 Copied from @ericlathrop which he shared in #unit-users in warpzone slcak
 **/
public static class CachedResource {
  private static readonly Dictionary<string, Object> cache = new Dictionary<string, Object>();
  private static readonly string FILE_PATH_PREFIX = "fileio_";
  public static void Clear() {
    cache.Clear();
  }
  public static T Load<T>(string path) where T: Object {
    if (!cache.ContainsKey(path)) {
      cache[path] = Resources.Load<T>(path);
    }
    return cache[path] as T;
  }

  /// 
  /// If you need to load a texture from a file that is not
  ///  marked as an Asset in Resources, use this method and
  ///  subsequent calls to load the file will be cached just like
  ///  with CachedResources.Load
  ///
  public static Texture2D LoadFile(string filePath) {
    string key = FILE_PATH_PREFIX + filePath;
    if(!cache.ContainsKey(key)) {
      Texture2D texture = _loadTexture2D(filePath);
      if(texture != null) {
        cache[key] = texture;
      }

      return texture;
    }else{
      return cache[key] as Texture2D;
    }

  }

  ///
  /// Loads a Texture2D from a given file. The texture will be
  ///  automatically sized based on the image size.
  ///
  private static Texture2D _loadTexture2D(string filePath)
  {

    // Load a PNG or JPG file from disk to a Texture2D
    // Returns null if load fails
    Texture2D texture;
    byte[] fileData;

    if (File.Exists(filePath))
    {
      fileData = File.ReadAllBytes(filePath);
      texture = new Texture2D(2, 2);           // Create new "empty" texture
      if (texture.LoadImage(fileData))           // Load the imagedata into the texture (size is set automatically)
        return texture;                 // If data = readable -> return texture
    }
    return null;                     // Return null if load failed
  }
}
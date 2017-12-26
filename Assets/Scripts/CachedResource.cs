using System.Collections.Generic;
using UnityEngine;

/**
 Copied from @ericlathrop which he shared in #unit-users in warpzone slcak
 **/
public static class CachedResource {
  private static readonly Dictionary<string, Object> cache = new Dictionary<string, Object>();
  public static void Clear() {
    cache.Clear();
  }
  public static T Load<T>(string path) where T: Object {
    if (!cache.ContainsKey(path)) {
      cache[path] = Resources.Load<T>(path);
    }
    return cache[path] as T;
  }
}
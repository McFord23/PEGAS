using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public static class JsonReader<TKey, TValue>
{
    public static Dictionary<TKey, TValue> LoadFile(string path)
    {
        var resourceJson = Resources.Load<TextAsset>(path);
        return JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(resourceJson.text);
    }
}

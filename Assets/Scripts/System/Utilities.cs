using System.Collections;
using UnityEngine;
using Unity.Collections;

public static class Utilities
{
    public delegate void SetColor(Color color);
    public delegate Color GetColor();
    
    public static IEnumerator ColorLerp(SetColor setColor, GetColor getColor, Color color, float speed)
    {
        float progress = 0;
        var startColor = getColor();
        
        while (getColor() != color)
        {
            progress += speed * Time.deltaTime;
            setColor(Color.Lerp(startColor, color, progress));
            yield return null;
        }
    }
    
    public static IEnumerator LocalPosSlerp(Transform transform, Vector3 targetPos, float speed)
    {
        float progress = 0;
        var startPos = transform.localPosition; 
        
        while (transform.localPosition != targetPos)
        {
            progress += speed * Time.deltaTime;
            transform.localPosition = Vector3.Slerp(startPos, targetPos, progress);
            yield return null;
        }
    }
    
    public static FixedString64Bytes GetPath(Transform objectTransform)
    {
        string path = objectTransform.name;

        while (objectTransform.parent != null)
        {
            objectTransform = objectTransform.parent;
            path = objectTransform.name + "/" + path;
        }

        return new FixedString64Bytes(path);
    }

    public static void PlayRandomSound(AudioSource audioPlayer, AudioClip[] sounds)
    {
        var index = Random.Range(0, sounds.Length);
        audioPlayer.clip = sounds[index];
        audioPlayer.Play();
    }

    public static string ToCamelCase(string key)
    {
        return char.ToLower(key[0]) + key[1..];
    }
    
    public static Vector2 Vector2Slerp(Vector2 firstPoint, Vector2 secondPoint, float interpolation)
    {
        var result = Vector3.Slerp(Vector2ToVector3(firstPoint), Vector2ToVector3(secondPoint), interpolation);
        return new Vector2(result.x, result.y);
    }
    
    private static Vector3 Vector2ToVector3(Vector2 vector2)
    {
        return new Vector3(vector2.x, vector2.y, 0);
    }
}

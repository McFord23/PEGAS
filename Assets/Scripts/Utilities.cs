using UnityEngine;
using Unity.Collections;

public static class Utilities
{
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

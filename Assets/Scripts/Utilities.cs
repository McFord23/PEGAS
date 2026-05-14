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
}

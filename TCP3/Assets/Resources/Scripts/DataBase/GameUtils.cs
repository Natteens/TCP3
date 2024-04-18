using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameUtils
{
    public static Ray CenterOfScreenRay()
    {
        return Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
    }
}

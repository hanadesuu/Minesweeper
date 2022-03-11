using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CubeIndex : MonoBehaviour
{
    public TMP_Text index;
    public void SetIndex(int v)
    {
        index.text = v.ToString();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    #region singleton
    private static CubeManager pm;
    private void Awake()
    {
        if (pm == null)
        {
            pm = this;
        }
    }
    public static CubeManager GetT()
    {
        return pm;
    }
    #endregion
    /// <summary>
    /// 根据position找到对应的格子
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="num"></param>
    public bool  SetNum(Vector2Int pos,int num)
    {
        Vector3 t = GameManager.GetT().Vector2IntToPos(pos);
        foreach (Transform item in this.transform)
        {
            if (item.position==t)
            {
              bool s=  item.GetComponent<Cube>().Show(GameManager.GetT().info[pos.x,pos.y]);
                return s;
            }
        }
        return false;
    }
    public int SetFlag(Vector2Int pos)
    {
        Vector3 t = GameManager.GetT().Vector2IntToPos(pos);
        foreach (Transform item in this.transform)
        {
            if (item.position==t)
            {
                int state= item.GetComponent<Cube>().SetFlag();
                return state;
            }
        }
        return -1;
    }
  
}
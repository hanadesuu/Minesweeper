using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cube : MonoBehaviour
{
    public Transform canvas;
    public Transform flag;
    public TMP_Text num;
    public List<Material> ms = new List<Material>();//0 default,1 mouse over,2 showed
    private bool isShowed=false;
    private bool isFlaged = false;
   

    private void OnMouseOver()
    {
        if (isShowed|| isFlaged)
        {
            return;
        }
        this.GetComponent<MeshRenderer>().material = ms[1];
    }
    private void OnMouseExit()
    {
        if (isShowed||isFlaged)
        {
            return;
        }
        this.GetComponent<MeshRenderer>().material = ms[0];
    }
    /// <summary>
    /// 单纯的显示出此格子上的数字
    /// </summary>
    /// <param name="num"></param>
    public bool Show(int num)
    {
        if (isFlaged||isShowed)
        {
            return false;
        }
        canvas.gameObject.SetActive(true);
        isShowed = true;
        this.num.text = num.ToString();
        this.GetComponent<MeshRenderer>().material = ms[2];
        return true;
    }
    /// <summary>
    /// 点击显示，同时会进行Check周围是否为空
    /// </summary>
    //public void Show()
    //{
    //    if (isShowed|| isFlaged)
    //    {
    //        return;
    //    }
    //    if (GameManager.GetT().isFirstClick)
    //    {
    //        GameManager.GetT().GenerateMine(this.transform.position);
    //    }
    //    GameManager.GetT().ShowNum(this.transform.position, this.num);
    //    canvas.gameObject.SetActive(true);
    //    isShowed = true;
    //    this.GetComponent<MeshRenderer>().material = ms[2];
    //}
    /// <summary>
    /// 插或取消旗子
    /// </summary>
   public int SetFlag()
    {
        if (isShowed )
        {
            return -1;
        }       
        //如果旗子数量到达上限，则不会继续插旗
        if (GameManager.GetT().IfOverMineCout())
        {
            return -1;
        }
        isFlaged = flag.gameObject.activeSelf;
        if (isFlaged)
        {
            flag.gameObject.SetActive(false);
            isFlaged = false;
            return 0;
        }
        else
        {
            flag.gameObject.SetActive(true);
            isFlaged = true;
            return 1;
        }
        
    }
}
//21650919
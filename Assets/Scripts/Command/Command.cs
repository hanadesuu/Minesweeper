using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command 
{ 
}
/// <summary>
/// 抽象命令类
/// </summary>
public interface  ICommand
{
  
     void Execute();
    void FlashBack();
}
//////////////////////////////////////////////////////
public class ClickCommand : ICommand
{
    Vector2Int clickPos;
    public ClickCommand(Vector2Int vector2Int)
    {
        this.clickPos = vector2Int;
    }
    public void Execute()
    {
        Debug.Log("点击："+clickPos);
        GameManager.GetT().eventCenter.click.Invoke(clickPos);
    }

    public void FlashBack()
    {
       // throw new System.NotImplementedException();
    }
}
public class FlagCommand : ICommand
{
    Vector2Int flagPos;
    public FlagCommand(Vector2Int vector2Int)
    {

        this.flagPos = vector2Int;
    }
    public void Execute()
    {
        Debug.Log("插旗：" + flagPos);
        GameManager.GetT().eventCenter.flag.Invoke(flagPos);
    }

    public void FlashBack()
    {
       // throw new System.NotImplementedException();
    }
}
public enum CommType 
{
    Click=0,Flag=1,RollBack=2
}
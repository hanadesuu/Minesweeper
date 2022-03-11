using System.Collections.Generic;

public class CommandHandler : Singleton<CommandHandler>
{

    //命令集合
    private List<ICommand> mCommands = new List<ICommand>();
    private List<ICommand> recordCommands = new List<ICommand>();
    private void AddCommand(ICommand command)
    {
        mCommands.Add(command);
    }
    /// <summary>
    /// 使用params参数一次添加多个命令
    /// </summary>
    private void AddCommand(params ICommand[] commands)
    {
        foreach (ICommand command in commands)
        {
            mCommands.Add(command);
        }
    }

    /// <summary>
    /// 执行所有命令，执行完并清除
    /// </summary>
    public void ExecuteCommand(params ICommand[] commands)
    {
        foreach (ICommand command in commands)
        {
            command.Execute();           
        }
        AddCommand(commands);
        index = mCommands.Count;
    }
    int index = 0;
    public void FlashBackCommand()
    {
        if (index>0)
        {
            index--;
            mCommands[index].FlashBack();
        }
        else
        {
            UnityEngine.Debug.Log("已回溯到起点");
        }
    }
}


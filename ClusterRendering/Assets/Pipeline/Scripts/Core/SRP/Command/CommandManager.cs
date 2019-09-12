using Framework;
using System.Collections.Generic;

class CommandManager : Singleton<CommandManager>
{

    private List<CommandBase> commandList = new List<CommandBase>();

    public override void Init()
    {

    }

    protected override void OnDestroy()
    {

    }

    public void Add(CommandBase command)
    {
        commandList.Add(command);
    }

    public void Remove(CommandBase command)
    {
        commandList.Remove(command);
    }

    public List<CommandBase> GetCommonds()
    {
        return commandList;
    }

}
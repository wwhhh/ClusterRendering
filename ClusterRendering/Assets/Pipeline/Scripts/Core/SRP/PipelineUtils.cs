using UnityEngine.Rendering;
using static PipelineComponent;

class PipelineUtils
{

    public static void ExecuteCommand(ref PipelineCommandData data, CommandBuffer command)
    {
        data.context.ExecuteCommandBuffer(command);
        command.Clear();
    }

}
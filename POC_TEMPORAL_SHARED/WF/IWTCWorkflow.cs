namespace POC_TEMPORAL_SHARED.WFs;

[Workflow]
public interface IWTCWorkflow
{
    [WorkflowRun]
    Task RunWTCWorkflowAsync();
}
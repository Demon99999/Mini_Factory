namespace Project.Scripts.Infrastructure.StateMachine
{
    public interface IPayloadState<in TPayload> : IExitableState
    {
        void Enter(TPayload payload);
    }
}
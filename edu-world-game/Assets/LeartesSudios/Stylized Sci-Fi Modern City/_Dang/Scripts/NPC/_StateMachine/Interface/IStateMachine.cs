namespace EduWorld
{
    public interface IStateMachine
    {
        void ChangeState(ENpcState newState);
        void Update();
    }
}

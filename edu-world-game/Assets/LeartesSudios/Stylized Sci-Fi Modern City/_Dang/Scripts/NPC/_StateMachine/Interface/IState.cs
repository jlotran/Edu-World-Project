namespace EduWorld
{
    public interface IState
    {
        void Enter();
        void StateUpdate();
        void Exit();
    }
}

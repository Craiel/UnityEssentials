namespace Craiel.UnityEssentials.Contracts
{
    using Msg;

    /// <summary>
    /// The state of a state machine defines the logic of the entities that enter, exit and last this state. 
    /// Additionally, a state may be delegated by an entity to handle its messages.
    /// </summary>
    /// <typeparam name="T">is the type of the entity handled by this state machine</typeparam>
    public interface IState<in T>
        where T : class
    {
        void Enter(T entity);

        void Update(T entity);

        void Exit(T entity);

        bool OnMessage(T entity, Telegram telegram);
    }
}

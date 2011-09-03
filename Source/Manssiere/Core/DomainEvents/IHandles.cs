namespace Manssiere.Core.DomainEvents
{
    /// <summary>
    /// domain event handler
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHandles<T> where T : IDomainEvent {
        /// <summary>
        /// Handles the specified @event.
        /// </summary>
        /// <param name="event">The @event.</param>
        void Handle(T @event);
    }
}
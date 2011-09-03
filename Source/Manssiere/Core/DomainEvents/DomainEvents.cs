namespace Manssiere.Core.DomainEvents
{
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Domain event entrence
    /// </summary>
    public static class DomainEvents {
        
        /// <summary>
        /// Raises the given domain event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args">The args.</param>
        public static void Raise<T>(T args) where T : IDomainEvent {
            
            var serviceLocator = ServiceLocator.Current;
            if (serviceLocator == null)
                return;

            var enumerable = serviceLocator.GetAllInstances<IHandles<T>>();
            if (enumerable == null) return;

            foreach (var handler in enumerable) {
                handler.Handle(args);
            }
        }
    }
}
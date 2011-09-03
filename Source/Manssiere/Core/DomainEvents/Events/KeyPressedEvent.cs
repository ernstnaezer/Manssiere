namespace Manssiere.Core.DomainEvents.Events
{
    using System.Collections.Generic;
    using OpenTK.Input;

    public class KeyPressedEvent : IDomainEvent
    {
        public IEnumerable<Key> PressedKeys { get; set; }

        public KeyPressedEvent(IEnumerable<Key> pressedKeys)
        {
            PressedKeys = pressedKeys;
        }
    }
}
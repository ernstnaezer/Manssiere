namespace Manssiere.Core.DemoFlow
{
    using System;

    public class StateChangeEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateChangeEventArgs"/> class.
        /// </summary>
        /// <param name="controlDefinition">The control definition.</param>
        public StateChangeEventArgs(AbstractDemoFlow.ControlDefinition controlDefinition)
        {
            if (controlDefinition == null) throw new ArgumentNullException("controlDefinition");
            ControlDefinition = controlDefinition;
        }

        /// <summary>
        /// Gets or sets the control definition.
        /// </summary>
        /// <value>The control definition.</value>
        public AbstractDemoFlow.ControlDefinition ControlDefinition { get; private set; }
    }
}
namespace Manssiere.Core.DemoFlow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Controls;
    using Manssiere.Core.Graphics.Transition;

    /// <summary>
    /// The demo flow controlls the order of the effects
    /// </summary>
    public abstract class AbstractDemoFlow
    {
        #region Delegates

        public delegate void StateChangedEventHandler(AbstractDemoFlow sender, StateChangeEventArgs args);

        #endregion

        private readonly List<ControlDefinition> _controls = new List<ControlDefinition>();
        private int _activeControl;

        /// <summary>
        /// Gets the state of the previous.
        /// </summary>
        /// <value>The state of the previous.</value>
        public ControlDefinition PreviousState
        {
            get { return _activeControl > 0 ? _controls[_activeControl - 1] : null; }
        }

        /// <summary>
        /// Gets the current scene.
        /// </summary>
        /// <value>The current scene.</value>
        public ControlDefinition CurrentScene
        {
            get { return _activeControl >= 0 && _activeControl < _controls.Count() ? _controls[_activeControl] : null; }
        }

        /// <summary>
        /// Show this effect without a transition.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected ControlDefinition Show<T>()
            where T : UserControl
        {
            var controlDefinition = new ControlDefinition(typeof(T), null, this);
            _controls.Add(controlDefinition);

            return new ControlDefinition(this);
        }

        /// <summary>
        /// Show this effect with a fadein.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected ControlDefinition FadeIn<T>()
            where T : UserControl
        {
            var controlDefinition = new ControlDefinition(typeof(T), typeof(FadeIn), this);
            _controls.Add(controlDefinition);

            return new ControlDefinition(this);
        }

        /// <summary>
        /// Show this effect with a fadein.
        /// </summary>
        /// <returns></returns>
        protected ControlDefinition FadeOut()
        {
            var controlDefinition = new ControlDefinition(null, typeof(FadeOut), this);
            _controls.Add(controlDefinition);

            return new ControlDefinition(this);
        }

        /// <summary>
        ///   Global audio sync event. Register to this point to receive
        ///   messages from the audio queue.
        /// </summary>
        public event StateChangedEventHandler StateChanged;

        /// <summary>
        /// Invokes the state changed.
        /// </summary>
        /// <param name="args">The <see cref="StateChangeEventArgs"/> instance containing the event data.</param>
        private void InvokeStateChanged(StateChangeEventArgs args)
        {
            var handler = StateChanged;
            if (handler != null) handler(this, args);
        }

        /// <summary>
        /// Moves to the next scene.
        /// </summary>
        public void NextScene()
        {
            if (_activeControl >= _controls.Count()) return;

            _activeControl++;

            if (_activeControl >= _controls.Count()) return;
            InvokeStateChanged(new StateChangeEventArgs(CurrentScene));
        }

        /// <summary>
        /// Moves to the previouse.
        /// </summary>
        public void PreviousScene()
        {
            if (_activeControl <= 0) return;

            _activeControl--;

            // on the back state we don't send a transition, this speeds up skipping & eliminates errors.
            // we could do the same for 'manual' forward moving. Myabe we can use differentt keys for this.
            if (_activeControl < 0) return;
            InvokeStateChanged(new StateChangeEventArgs(new ControlDefinition(CurrentScene.ControlType, null, this)));
        }

        /// <summary>
        /// Gets a value indicating whether this instance has scenes.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has scenes; otherwise, <c>false</c>.
        /// </value>
        public bool HasScenes
        {
            get { return _controls.Any(); }
        }

        #region Nested type: ControlDefinition

        public class ControlDefinition
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ControlDefinition"/> class.
            /// </summary>
            /// <param name="controlType">Type of the control.</param>
            /// <param name="transitionType">Type of the transition.</param>
            /// <param name="demoFlow">The demo flow.</param>
            public ControlDefinition(Type controlType, Type transitionType, AbstractDemoFlow demoFlow)
            {
                if (demoFlow == null) throw new ArgumentNullException("demoFlow");

                DemoFlow = demoFlow;
                ControlType = controlType;
                TransitionType = transitionType;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ControlDefinition"/> class.
            /// </summary>
            /// <param name="demoFlow">The demo flow.</param>
            public ControlDefinition(AbstractDemoFlow demoFlow)
            {
                DemoFlow = demoFlow;
            }

            /// <summary>
            /// Gets or sets the demo flow.
            /// </summary>
            /// <value>The demo flow.</value>
            private AbstractDemoFlow DemoFlow { get; set; }

            /// <summary>
            /// Gets or sets the type of the control.
            /// </summary>
            /// <value>The type of the control.</value>
            public Type ControlType { get; private set; }

            /// <summary>
            /// Gets or sets the type of the transition.
            /// </summary>
            /// <value>The type of the transition.</value>
            public Type TransitionType { get; private set; }

            /// <summary>
            /// Define the new effect to show
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public UsingDefinition TransitionTo<T>()
            {
                ControlType = typeof(T);

                DemoFlow._controls.Add(this);

                return new UsingDefinition(this);
            }

            #region Nested type: UsingDefinition

            public class UsingDefinition
            {
                private readonly ControlDefinition _controlDefinition;

                /// <summary>
                /// Initializes a new instance of the <see cref="UsingDefinition"/> class.
                /// </summary>
                /// <param name="controlDefinition">The control definition.</param>
                public UsingDefinition(ControlDefinition controlDefinition)
                {
                    _controlDefinition = controlDefinition;
                }

                /// <summary>
                /// Define a transition between the effects.
                /// </summary>
                /// <typeparam name="T"></typeparam>
                /// <returns></returns>
                public ControlDefinition Using<T>()
                    where T : ITransition
                {
                    _controlDefinition.TransitionType = typeof(T);
                    var controlDefinition = new ControlDefinition(_controlDefinition.DemoFlow);

                    return controlDefinition;
                }
            }

            #endregion
        }

        #endregion
    }
}
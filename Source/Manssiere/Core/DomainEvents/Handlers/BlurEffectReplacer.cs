namespace Manssiere.Core.DomainEvents.Handlers
{
    using Events;
    using Graphics.Effects;
    using BlurEffect = System.Windows.Media.Effects.BlurEffect;

    public class BlurEffectReplacer : IHandles<XamlObjectLoadedEvent>
    {
        /// <summary>
        /// Handles the specified @event.
        /// </summary>
        /// <param name="event">The @event.</param>
        public void Handle(XamlObjectLoadedEvent @event)
        {
            if (@event.FrameworkElement.Effect is BlurEffect == false)
                return;

            var blurEffect = new Graphics.Effects.BlurEffect
                                 {
                                     BlurShaderType = BlurShaderType.Gaussian,
                                     //Radius = (float)((BlurEffect)@event.FrameworkElement.Effect).Radius
                                     Radius = 0
                                 };

            blurEffect.Initialize();
            @event.FrameworkElement.Effect = blurEffect;
            @event.FrameworkElement.Effect = null;
        }
    }
}
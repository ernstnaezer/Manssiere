namespace Manssiere.Core.Graphics.Effects
{
    using System.Windows;

    public interface IEffect
    {
        /// <summary>
        /// Indicates wheter this shader is initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Initiailizes this effect
        /// </summary>
        void Initialize();

        /// <summary>
        /// Event triggered before the xaml rendering
        /// </summary>
        /// <param name="element"></param>
        void PreRenderAction(UIElement element);

        /// <summary>
        /// Event triggered after the xaml rendering
        /// </summary>
        /// <param name="element"></param>
        void PostRenderAction(UIElement element);
    }
}
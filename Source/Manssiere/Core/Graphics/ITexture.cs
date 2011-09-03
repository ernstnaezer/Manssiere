namespace Manssiere.Core.Graphics
{
    using System;

    public interface ITexture : IDisposable
    {
        /// <summary>
        ///   Gets the texture handle.
        /// </summary>
        uint TextureHandle { get; }
    }
}
namespace Manssiere.Core.DomainEvents.Handlers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;
    using Castle.Core;
    using Core.DomainEvents;
    using Core.DomainEvents.Events;
    using Core.Graphics;
    using Core.Helpers;
    using Image = System.Drawing.Image;
    using Size = System.Drawing.Size;

    /// <summary>
    ///   Load images for imagebrushes in a resouce dictionary, 
    ///   image tags inside the xaml and images inside a material applied to a geometry.
    /// </summary>
    public class LoadImagesForXamlObject : IHandles<XamlObjectLoadedEvent>
    {
        private static readonly IDictionary<string, Texture> TextureCache = new Dictionary<string, Texture>();

        /// <summary>
        ///   Handles the specified @event.
        /// </summary>
        /// <param name = "event">The @event.</param>
        public void Handle(XamlObjectLoadedEvent @event)
        {
            InitializeImages(@event.FrameworkElement);
            InitializeImages(@event.FrameworkElement.Resources);
        }

        /// <summary>
        ///   Initializes the images inside a dictionary.
        /// </summary>
        /// <param name = "resources">The resources.</param>
        private static void InitializeImages(IDictionary resources)
        {
            resources
                .Values.OfType<ImageBrush>()
                .ForEach(imageBrush =>
                             {
                                 var texture = LoadImage(imageBrush.ImageSource);
                                 DemoDependencyHelper.SetTexture(imageBrush, texture);
                             });
        }

        /// <summary>
        ///   Raises the loaded event.
        /// </summary>
        /// <param name = "xamlObject">The xaml object.</param>
        private static void InitializeImages(FrameworkElement xamlObject)
        {
            if (xamlObject is System.Windows.Controls.Image)
            {
                var texture = LoadImage(((System.Windows.Controls.Image) xamlObject).Source);
                DemoDependencyHelper.SetTexture(xamlObject, texture);
            }
            else if (xamlObject is Viewport3D)
            {
                ((Viewport3D) xamlObject).Children.ForEach(Process3DModels);
            }
        }

        private static void Process3DModels(Visual3D visual)
        {
            if (!(visual is ModelVisual3D)) return;
            Process3DModels(((ModelVisual3D) visual).Content);
        }

        /// <summary>
        ///   Looks for image brushed to load the textures.
        /// </summary>
        /// <param name = "visual">The model</param>
        private static void Process3DModels(Model3D visual)
        {
            if (visual is Model3DGroup)
            {
                ((Model3DGroup) visual).Children.ForEach(Process3DModels);
            }
            else if (visual is GeometryModel3D)
            {
                ProcessMaterial(((GeometryModel3D) visual).Material);
                ProcessMaterial(((GeometryModel3D) visual).BackMaterial);
            }
        }

        /// <summary>
        ///   Processes a material (or group) in search for image brushes inside a diffuse material.
        /// </summary>
        /// <param name = "material"></param>
        private static void ProcessMaterial(Material material)
        {
            if (material is MaterialGroup)
            {
                ((MaterialGroup) material).Children.ForEach(ProcessMaterial);
            }
            else if (material is DiffuseMaterial)
            {
                if (((DiffuseMaterial) material).Brush as ImageBrush == null) return;

                var imageBrush = (ImageBrush) ((DiffuseMaterial) material).Brush;

                var texture = LoadImage(imageBrush.ImageSource);
                DemoDependencyHelper.SetTexture(imageBrush, texture);
            }
        }

        /// <summary>
        ///   Load an image to opengl.
        /// </summary>
        /// <param name = "image"></param>
        private static Texture LoadImage(ImageSource image)
        {
            if (image == null) return null;

            // we use the location as a cache key... bit of a hack I guess, but hey, it's a demo :)
            var imageLocation = image.ToString();

            // prevent loading the same image source multiple times.
            if (TextureCache.ContainsKey(imageLocation))
            {
                return TextureCache[imageLocation];
            }
;
            var bitmap = XamlHelper.GdiBitmapFromWpfBitmap((BitmapSource) image);

            var texture = Texture.FromBitmap(bitmap);

            // cache the image
            TextureCache[imageLocation] = texture;

            return texture;
        }
    }
}
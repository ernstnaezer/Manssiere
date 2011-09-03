namespace Manssiere.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using System.Text.RegularExpressions;
    using System.Windows.Media.Imaging;

    public static class ResourceLoader {

        /// <summary>
        /// Gets the resource names.
        /// </summary>
        /// <param name="resourceFolder">The resource folder.</param>
        /// <returns></returns>
        public static IEnumerable<string> GetResourceNames(string resourceFolder) {
            
            var pattern = WildcardToResourceRegex(resourceFolder);

            var resourceNames = GetManifestResourceNames()
                .Where(path => Regex.Match(path, pattern, RegexOptions.IgnoreCase).Success);

            return resourceNames;
        }

        /// <summary>
        /// Creates an wpf image and the and upload image.
        /// </summary>
        /// <param name="resourceLocation">The resource location.</param>
        /// <returns></returns>
        public static BitmapImage CreateImage(string resourceLocation) {
            
            var image = new BitmapImage();

            image.BeginInit();
            image.UriSource = new Uri(
                string.Format(@"pack://application:,,,/{0};component/{1}",
                              "Manssiere",
                              resourceLocation));
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();

            image.Freeze();

            return image;
        }

        /// <summary>
        /// Gets the manifest resource names.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<string> GetManifestResourceNames() {
            
            var assembly = Assembly.GetExecutingAssembly();

            var r = (from n in assembly.GetManifestResourceNames()
                     where n.EndsWith("g.resources")
                     let stream = assembly.GetManifestResourceStream(n)
                     where stream != null
                     select new ResourceReader(stream)).First();

            return from DictionaryEntry entry in r
                   let key = (string)entry.Key
                   select key;
        }

        /// <summary>
        /// Wildcards to resource regex.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        private static string WildcardToResourceRegex(string pattern) {
            return string.Format(@"{0}",
                                 Regex.Escape(pattern)
                                     .Replace("\\*", ".*")
                                     .Replace("\\?", ".")
                                     .Substring(1)
                );
        }
    }
}
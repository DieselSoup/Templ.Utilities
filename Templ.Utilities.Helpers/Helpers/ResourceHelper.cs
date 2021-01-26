using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace Templ.Utilities.Helpers
{
    public static class ResourceHelper
    {
        public static string GetContents(string resourcePath, string prefix = null)
        {
            string retVal = null;

            var resourceName = GetResourceName(resourcePath, prefix);
            var assembly = typeof(ResourceHelper).GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        retVal = reader.ReadToEnd();
                    }
                }
                else
                {
                    throw new Exception($"Unable to find resource - {resourceName}");
                }
            }

            return retVal;
        }

        public static T GetObject<T>(string resourcePath, string prefix = null)
        {
            T retVal = default(T);

            var resourceName = GetResourceName(resourcePath, prefix);
            var json = GetJsonStringResource(resourceName);

            try
            {
                retVal = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occured while processing resource - {resourceName} - {ex.Message}", ex);
            }

            return retVal;
        }



        private static string GetResourceName(string resourcePath, string prefix = null)
        {
            if (String.IsNullOrEmpty(prefix))
            {
                return $"{resourcePath}";
            }
            else
            {
                return $"{prefix}.{resourcePath}";
            }
        }

        private static string GetJsonStringResource(string resourceName)
        {
            string retVal = null;
            var assembly = typeof(ResourceHelper).GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        retVal = reader.ReadToEnd();
                    }
                }
                else
                {
                    throw new Exception($"Unable to find resource - {resourceName}");
                }
            }

            return retVal;
        }
    }
}

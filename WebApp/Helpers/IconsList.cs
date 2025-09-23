using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebApp.Helpers
{
    public static class IconsList
    {
        public static List<string> GetAvailableIcons()
        {
            try
            {
                var iconPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "landing-page", "images", "icon");
                if (Directory.Exists(iconPath))
                {
                    var iconFiles = Directory.GetFiles(iconPath, "*.svg")
                        .Select(Path.GetFileName)
                        .Where(file => !string.IsNullOrEmpty(file))
                        .OrderBy(file => file)
                        .ToList();
                    return iconFiles;
                }
                return new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}

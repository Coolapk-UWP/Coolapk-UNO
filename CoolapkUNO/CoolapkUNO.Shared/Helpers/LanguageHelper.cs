using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Windows.System.UserProfile;

namespace CoolapkUNO.Helpers
{
    internal static class LanguageHelper
    {
        public static string AutoLanguageCode = "auto";

        public static List<string> SupportLanguages = new List<string>
        {
            "en-US",
            "zh-CN"
        };

        private static readonly List<string> SupportLanguageCodes = new List<string>
        {
            "en, en-au, en-ca, en-gb, en-ie, en-in, en-nz, en-sg, en-us, en-za, en-bz, en-hk, en-id, en-jm, en-kz, en-mt, en-my, en-ph, en-pk, en-tt, en-vn, en-zw, en-053, en-021, en-029, en-011, en-018, en-014",
            "zh-Hans, zh-cn, zh-hans-cn, zh-sg, zh-hans-sg"
        };

        public static List<CultureInfo> SupportCultures = SupportLanguages.Select(x => new CultureInfo(x)).ToList();

        public static string GetCurrentLanguage()
        {
            IReadOnlyList<string> languages = GlobalizationPreferences.Languages;
            foreach (string language in languages)
            {
                foreach (string code in SupportLanguageCodes)
                {
                    if (code.ToLower().Contains(language.ToLower()))
                    {
                        int temp = SupportLanguageCodes.IndexOf(code);
                        return SupportLanguages[temp];
                    }
                }
            }
            return SupportLanguages[1];
        }
    }
}

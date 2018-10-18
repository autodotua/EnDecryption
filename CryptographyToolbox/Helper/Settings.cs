using System;
using System.Text;
using Windows.Storage;
using Windows.UI.Xaml;

namespace CryptographyToolbox.Helper
{
    public static class Settings
    {
        static Settings()
        {
            Encoding = Encoding.GetEncoding(GetSettings(nameof(Encoding), 65001));
            BinaryInputMode = Enum.Parse<BinaryMode>(GetSettings(nameof(BinaryInputMode), BinaryMode.String.ToString()));
            BinaryKeyMode = Enum.Parse<BinaryMode>(GetSettings(nameof(BinaryKeyMode), BinaryMode.String.ToString()));
            BinaryOutputMode = Enum.Parse<BinaryMode>(GetSettings(nameof(BinaryOutputMode), BinaryMode.Base64.ToString()));

            AesLength = GetSettings(nameof(AesLength), 128);
            TextBoxWrapping = Enum.Parse<TextWrapping>(GetSettings(nameof(TextBoxWrapping), TextWrapping.NoWrap.ToString()));

        }
        private static ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
        //public static string Separator { get; set; }

        public static Encoding Encoding { get; set; }

        public static BinaryMode BinaryInputMode { get; set; } = BinaryMode.String;
        public static BinaryMode BinaryOutputMode { get; set; } = BinaryMode.Base64;
        public static BinaryMode BinaryKeyMode { get; set; } = BinaryMode.String;

        public static int AesLength { get; set; } = 128;
        public static TextWrapping TextBoxWrapping { get; set; }

        public static byte KeyPadding { get; set; } = 0;

        private static T GetSettings<T>(string name)
        {
            return (T)settings.Values[name];
        }

        private static void SetSettings<T>(string name, T value)
        {
            settings.Values[name] = value;
        }
        // public static ResourceDictionary resource=new ResourceDictionary();

        private static T GetSettings<T>(string name, T defautValue)
        {

            if (!settings.Values.ContainsKey(name))
            {
                return defautValue;
            }
            else
            {
                return (T)settings.Values[name];
            }
        }
        public static void EnsureSettings<T>(string name, T defautValue)
        {

            if (!settings.Values.ContainsKey(name))
            {
                SetSettings(name, defautValue);
            }
        }
        public static void Save()
        {
            SetSettings(nameof(Encoding), Encoding.CodePage);
            SetSettings(nameof(BinaryInputMode), BinaryInputMode.ToString());
            SetSettings(nameof(BinaryOutputMode), BinaryOutputMode.ToString());
            SetSettings(nameof(BinaryKeyMode), BinaryKeyMode.ToString());
            SetSettings(nameof(AesLength), AesLength);
            SetSettings(nameof(TextBoxWrapping), TextBoxWrapping.ToString());
        }
    }

    public enum BinaryMode
    {
        String,
        Base64,
        Hex,
        HexWithDash,
        DecWithComma
    }
}

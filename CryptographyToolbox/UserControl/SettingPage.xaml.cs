using CryptographyToolbox.Helper;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static CryptographyToolbox.Helper.Settings;
using Encode = System.Text.Encoding;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace CryptographyToolbox
{
    public sealed partial class SettingPage : UserControl
    {
        private bool notLoaded = true;
        public SettingPage()
        {
            this.InitializeComponent();
            switchWrapping.IsOn = TextBoxWrapping == TextWrapping.Wrap;
            cbbInputMode.SelectedIndex = IndexAndBinaryMode.First(p => p.mode == BinaryInputMode).index;
            cbbKeyMode.SelectedIndex = IndexAndBinaryMode.First(p => p.mode == BinaryKeyMode).index;
            cbbOutputMode.SelectedIndex = IndexAndBinaryMode.First(p => p.mode == BinaryOutputMode).index;
            cbbEncoding.SelectedItem = DescriptionsAndEncodings.First(p => p.encoding == Settings.Encoding.CodePage).description;
            notLoaded = false;
        }
        private void switchWrapping_Toggled(object sender, RoutedEventArgs e)
        {
            if(notLoaded)
            {
                return;
            }
            TextBoxWrapping = switchWrapping.IsOn ? TextWrapping.Wrap : TextWrapping.NoWrap;
            MainPage.Current.RefreshUISettings();
        }

        private (string description, int encoding)[] DescriptionsAndEncodings = new (string description, int encoding)[]
        {
            ("默认",Encode.Default.CodePage),
            ( "UTF8",Encode.UTF8.CodePage),
            ( "Unicode", Encode.Unicode.CodePage),
            ("UTF32",Encode.UTF32.CodePage),
            ( "UTF7",Encode.UTF7.CodePage),
            ( "BigEndianUnicode", Encode.BigEndianUnicode.CodePage),
            ( "GB2312", Encode.GetEncoding("GB2312").CodePage),
        };

        private (int index, BinaryMode mode)[] IndexAndBinaryMode = new (int, BinaryMode)[]
        {
            (0,BinaryMode.Base64),
            (1,BinaryMode.DecWithComma),
            (2,BinaryMode.Hex),
            (3,BinaryMode.HexWithDash),
            (4,BinaryMode.String),
        };


        private void cbbEncoding_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (notLoaded)
            {
                return;
            }
            Settings.Encoding =Encode.GetEncoding( DescriptionsAndEncodings.First(p => p.description == (cbbEncoding.SelectedItem as ComboBoxItem).Content as string).encoding);

        }

        private void ModeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (notLoaded)
            {
                return;
            }
            ComboBox cbb = sender as ComboBox;
            BinaryMode mode = IndexAndBinaryMode.First(p => p.index == cbb.SelectedIndex).mode;

            switch (cbb.Tag as string)
            {
                case "0":
                    BinaryInputMode = mode;
                    break;
                case "1":
                    BinaryKeyMode = mode;
                    break;
                case "2":
                    BinaryOutputMode = mode;
                    break;
            }
        }
    }
}

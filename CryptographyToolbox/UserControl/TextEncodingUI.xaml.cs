using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Web;
using static CryptographyToolbox.Tools;
using static CryptographyToolbox.Helper.ConvertHelper;
using CryptographyToolbox.Helper;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace CryptographyToolbox
{
    public sealed partial class TextEncodingUI : UserControl, ICryptographyPageBase
    {
        public TextEncodingUI()
        {
            InitializeComponent();
        }


        public async Task Encrypte()
        {
            if (txtSource.Text == "")
            {
                ShowError("源文本为空！");
                return;
            }
            txtResult.Text = await EncodeText();

        }



        public async Task Decrypte()
        {

            if (txtResult.Text == "")
            {
                ShowError("源文本为空！");
                return;
            }

            txtSource.Text = await DecodeText();


        }
        public async Task<string> DecodeText()
        {
            int type = cbbEncoding.SelectedIndex;
            bool include = chkInclude.IsChecked.Value;
            string text = txtResult.Text;
            return await Task.Run(() =>
            {
                if (type == 0)
                {
                    Regex regex = new Regex(@"\\u(?<code>[0-9A-Fa-f]{4})",RegexOptions.Compiled);
                    MatchEvaluator matchEvaluator = new MatchEvaluator(match =>
                    {
                    return ((char)(Convert.ToInt32(match.Groups["code"].Value, 16))).ToString();
                    });
                    return regex.Replace(text, matchEvaluator);
                }
                if (type == 1)
                {
                    Regex regex = new Regex(@"&#x(?<code>[0-9A-Fa-f]{4});", RegexOptions.Compiled);
                    MatchEvaluator matchEvaluator = new MatchEvaluator(match =>
                    {
                        return ((char)(Convert.ToInt32(match.Groups["code"].Value, 16))).ToString();
                    });
                    return regex.Replace(text, matchEvaluator);
                }
                if (type == 2)
                {
                 return   HttpUtility.UrlDecode(text,Encoding.UTF8);
                }
                if (type == 3)
                {
                    return HttpUtility.UrlDecode(text, Encoding.GetEncoding("GB2312"));
                }

                return "";
            });
        }


        public async Task<string> EncodeText()
        {
            int type = cbbEncoding.SelectedIndex;
            bool include = chkInclude.IsChecked.Value;
            string text = txtSource.Text;
            return await Task.Run(() =>
            {
                if (type == 0)
                {
                    StringBuilder str = new StringBuilder();
                    foreach (var i in text)
                    {
                        str.Append("\\u" + ((int)i).ToString("x4"));
                    }
                    return str.ToString();
                }
                if (type == 1)
                {
                    StringBuilder str = new StringBuilder();
                    foreach (var i in text)
                    {
                        str.Append("&#x" + ((int)i).ToString("X4") + ";");
                    }
                    return str.ToString();
                }
                if (type == 2)
                {
                    StringBuilder str = new StringBuilder();
                    foreach (var i in text)
                    {
                        if (!include && IsEnglishDigitOrLetter(i))
                        {
                            str.Append(i);
                        }
                        else
                        {
                            foreach (var j in Encoding.UTF8.GetBytes(new char[] { i }))
                            {

                                str.Append("%" + j.ToString("x2"));
                            }
                        }
                    }
                    return str.ToString();
                }
                if (type == 3)
                {
                    StringBuilder str = new StringBuilder();
                    Encoding encoding = Encoding.GetEncoding("GB2312");
                    foreach (var i in text)
                    {
                        if (!include && IsEnglishDigitOrLetter(i))
                        {
                            str.Append(i);
                        }
                        else
                        {
                            foreach (var j in encoding.GetBytes(new char[] { i }))
                            {

                                str.Append("%" + j.ToString("x2"));
                            }
                        }
                    }
                    return str.ToString();
                }
                return "";

            });
        }

        private bool IsEnglishDigitOrLetter(char c)
        {
            if (c > '0' && c < '9')
            {
                return true;
            }

            if (c > 'a' && c < 'z')
            {
                return true;
            }

            if (c > 'A' && c < 'Z')
            {
                return true;
            }

            return false;
        }


        public Task EncrypteFile() => throw new NotImplementedException();
        public Task DecrypteFile() => throw new NotImplementedException();


       public void RefreshUISettings()
        {
            txtSource.TextWrapping = txtResult.TextWrapping =Settings. TextBoxWrapping;
        }
        public string Separator => throw new NotImplementedException();
        public string DisplayMode => throw new NotImplementedException();


        public TextBox TxtSource => txtSource;
        public TextBox TxtResult => txtResult;

        public Task GenerateKey() => throw new NotImplementedException();

        private void CbbEncodingSelectionChangedEventHandler(object sender, SelectionChangedEventArgs e)
        {
            if (chkInclude != null)
            {
                chkInclude.Visibility = (cbbEncoding.SelectedIndex == 2 || cbbEncoding.SelectedIndex == 3) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}

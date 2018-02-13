using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace EnDecryption
{
    public sealed partial class NotifyPopup : UserControl
    {
        private Popup m_Popup;

        private string m_TextBlockContent;

        private NotifyPopup()
        {
            InitializeComponent();
            m_Popup = new Popup();
            Width = Window.Current.Bounds.Width;
            Height = Window.Current.Bounds.Height;
            m_Popup.Child = this;
            img.Source= new BitmapImage(new Uri(this.BaseUri, "/Assets/StoreLogo.scale-100.png"));

            Loaded += LoadedEventHandler;
            Unloaded += UnloadedEventHandler;
        }

        public NotifyPopup(string content) : this()
        {
            m_TextBlockContent = content;
        }
        

        public void Show()
        {
            m_Popup.IsOpen = true;
        }

        private void LoadedEventHandler(object sender, RoutedEventArgs e)
        {
            tbNotify.Text = m_TextBlockContent;
            sbOut.Begin();
            sbOut.Completed += SbOut_Completed;
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        private void SbOut_Completed(object sender, object e)
        {
            m_Popup.IsOpen = false;
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            Width = e.Size.Width;
            Height = e.Size.Height;
        }

        private void UnloadedEventHandler(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= Current_SizeChanged;
        }
    
}
}

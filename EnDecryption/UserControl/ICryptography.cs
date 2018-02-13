using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace EnDecryption
{
    public interface ICryptography
    {
        int SeparatorIndex { get; }
        int EncodingIndex { get; }

        TextBox TxtSource { get;  }
        TextBox TxtResult { get;  }

        Task Encrypte();
        Task Decrypte();

        Task GenerateKey();
    }
}

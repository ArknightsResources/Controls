using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ArknightsResources.Controls.Uwp
{
    /// <summary>
    /// 表示可播放明日方舟剧情文件的控件
    /// </summary>
    public sealed partial class StoryPlayer : UserControl
    {
        /// <summary>
        /// 初始化<seealso cref="StoryPlayer"/>类的新实例
        /// </summary>
        public StoryPlayer()
        {
            this.InitializeComponent();
        }
    }
}

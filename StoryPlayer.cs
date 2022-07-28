using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace ArknightsResources.Controls.Uwp
{
    /// <summary>
    /// 表示可播放明日方舟剧情文件的控件
    /// </summary>
    public sealed class StoryPlayer : Control
    {
        /// <summary>
        /// 初始化<seealso cref="StoryPlayer"/>类的新实例
        /// </summary>
        public StoryPlayer()
        {
            this.DefaultStyleKey = typeof(StoryPlayer);
        }
    }
}

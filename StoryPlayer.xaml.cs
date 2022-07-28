using ArknightsResources.Stories.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ArknightsResources.Controls.Uwp
{
    /// <summary>
    /// 表示可播放明日方舟剧情文件的控件
    /// </summary>
    public sealed partial class StoryPlayer : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 标识<see cref="CurrentStory"/>的依赖项属性
        /// </summary>
        public static readonly DependencyProperty CurrentStoryProperty = DependencyProperty.Register(
            nameof(CurrentStory),
            typeof(StoryScene),
            typeof(StoryPlayer),
            new PropertyMetadata(null, (d, e) => OnCurrentStoryChanged(d, e)));
        
        private string _CharacterName;
        private string _StoryText;


        /// <summary>
        /// 初始化<seealso cref="StoryPlayer"/>类的新实例
        /// </summary>
        public StoryPlayer()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 当前<see cref="StoryPlayer"/>要播放的剧情
        /// </summary>
        public StoryScene CurrentStory
        {
            get => (StoryScene)GetValue(CurrentStoryProperty);
            set => SetValue(CurrentStoryProperty, value);
        }

        internal string CharacterName
        {
            get => _CharacterName;
            set
            {
                _CharacterName = value;
                OnPropertiesChanged();
            }
        }

        internal string StoryText
        {
            get => _StoryText;
            set
            {
                _StoryText = value;
                OnPropertiesChanged();
            }
        }

        private static void OnCurrentStoryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StoryPlayer player && e.NewValue is StoryScene scene)
            {
                player.CurrentStory = scene;
            }
        }

        /// <summary>
        /// 通知系统属性已经发生更改
        /// </summary>
        /// <param name="propertyName">发生更改的属性名称,其填充是自动完成的</param>
        private void OnPropertiesChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

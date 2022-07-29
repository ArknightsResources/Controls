using ArknightsResources.Stories.Models;
using ArknightsResources.Stories.Models.Commands;
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
using Windows.UI.Xaml.Controls.Primitives;

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

        private IEnumerator<StoryCommand> StorySceneEnumerator = null;
        private readonly DispatcherTimer StoryAutoPlayTimer = new DispatcherTimer();

        private string _CharacterName;
        private string _StoryText;
        private bool _IsStoryPlayComplete;
        private string _StorySubtitle;
        private bool _IsAuto;


        /// <summary>
        /// 初始化<seealso cref="StoryPlayer"/>类的新实例
        /// </summary>
        public StoryPlayer()
        {
            this.InitializeComponent();
            StoryAutoPlayTimer.Tick += OnStoryAutoPlayTimerTick;
        }

        /// <summary>
        /// 当前<see cref="StoryPlayer"/>要播放的剧情
        /// </summary>
        public StoryScene CurrentStory
        {
            get => (StoryScene)GetValue(CurrentStoryProperty);
            set => SetValue(CurrentStoryProperty, value);
        }

        /// <summary>
        /// 指示当前的<see cref="StoryPlayer"/>是否播放完剧情
        /// </summary>
        public bool IsStoryPlayComplete
        {
            get => _IsStoryPlayComplete;
            set
            {
                _IsStoryPlayComplete = value;
                OnPropertiesChanged();
            }
        }

        internal bool IsAuto
        {
            get => _IsAuto;
            set
            {
                _IsAuto = value;

                if (_IsAuto)
                {
                    StartAutoPlay();
                }
                else
                {
                    StopAutoPlay();
                }

                OnPropertiesChanged();
            }
        }

        public double PlayRatio { get; set; } = 1d;

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

        internal string StorySubtitle
        {
            get => _StorySubtitle;
            set
            {
                _StorySubtitle = value;
                OnPropertiesChanged();
            }
        }

        private static void OnCurrentStoryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StoryPlayer player && e.NewValue is StoryScene scene)
            {
                player.CurrentStory = scene;
                IEnumerator<StoryCommand> enumerator = scene.GetEnumerator();
                enumerator.Reset();
                player.StorySceneEnumerator = enumerator;
                player.HandleStoryCommand();
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

        private void PlayNextStoryCommand(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            PlayNextStoryCommandCore();
        }

        private void PlayNextStoryCommandDoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            PlayNextStoryCommandCore();
        }

        private void PlayNextStoryCommandCore()
        {
            if (CurrentStory is null || StorySceneEnumerator is null)
            {
                return;
            }

            HandleStoryCommand();
        }

        private void HandleStoryCommand()
        {
            if (StorySceneEnumerator.MoveNext() == false)
            {
                IsStoryPlayComplete = true;
                return;
            }

            ApplyStoryCommand(StorySceneEnumerator.Current);

            if (StorySceneEnumerator.Current.CanAutoContinue)
            {
                HandleStoryCommand();
            }
        }

        private void ApplyStoryCommand(StoryCommand cmd)
        {
            switch (cmd)
            {
                case ShowTextWithNameCommand stwn:
                    CharacterName = stwn.Name;
                    StoryText = stwn.Text;
                    break;
                case ShowPlainTextCommand spt:
                    StoryText = spt.Text;
                    CharacterName = string.Empty;
                    break;
                case ShowSubtitleCommand ss:
                    StorySubtitle = ss.Text;
                    break;
                case HideSubtitleCommand _:
                    StorySubtitle = string.Empty;
                    break;
                default:
                    CharacterName = cmd.GetType().Name;
                    StoryText = cmd.ToString();
                    break;
            }
        }

        private void OnStoryAutoPlayTimerTick(object sender, object e)
        {
            if (PlayRatio == 2d)
            {
                StoryAutoPlayTimer.Interval = new TimeSpan(0, 0, StoryText.Length / 4);
            }
            else
            {
                StoryAutoPlayTimer.Interval = new TimeSpan(0, 0, StoryText.Length / 2);
            }

            PlayNextStoryCommandCore();
        }

        private void StartAutoPlay()
        {
            StoryAutoPlayTimer.Start();
        }

        private void StopAutoPlay()
        {
            StoryAutoPlayTimer.Stop();
        }

        private void AutoButtonClicked(object sender, RoutedEventArgs e)
        {
            switch (sender)
            {
                case ToggleButton toggleButton:
                    switch (toggleButton.IsChecked)
                    {
                        case true:
                            AutoButtonTextStoryBoard.Begin();
                            return;
                        default:
                            AutoButtonTextStoryBoard.Stop();
                            return;
                    }
                default:
                    break;
            }
        }

        private void AutoPlaySpeedButtonClicked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton)
            {
                switch (toggleButton.IsChecked)
                {
                    case true:
                        PlayRatio = 2d;
                        TextBlock textBlock = (toggleButton.Content as TextBlock);
                        if (textBlock != null)
                        {
                            textBlock.Text = "2X";
                        }
                        break;
                    default:
                        PlayRatio = 1d;
                        TextBlock textBlock2 = (toggleButton.Content as TextBlock);
                        if (textBlock2 != null)
                        {
                            textBlock2.Text = "1X";
                        }
                        break;
                }
            }
        }
    }
}

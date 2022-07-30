using ArknightsResources.Controls.Uwp.Models;
using ArknightsResources.Stories.Models;
using ArknightsResources.Stories.Models.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

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
        private readonly ObservableCollection<StoryHistoryItem> StoryHistoryList = new ObservableCollection<StoryHistoryItem>();

        private string _CharacterName;
        private string _StoryText;
        private bool _IsStoryPlayComplete;
        private string _StorySubtitle;
        private bool _IsAuto;
        private Visibility _TextAndTopButtonsVisibility;
        private bool _LoadStoryHistory;


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
        internal bool LoadStoryHistory
        {
            get => _LoadStoryHistory;
            set
            {
                _LoadStoryHistory = value;
                OnPropertiesChanged();
                OnPropertiesChanged(nameof(IsTopButtonsTabStop));
            }
        }

        internal bool IsTopButtonsTabStop
        {
            get => !_LoadStoryHistory;
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

        internal Visibility TextAndTopButtonsVisibility
        {
            get => _TextAndTopButtonsVisibility;
            set
            {
                _TextAndTopButtonsVisibility = value;
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
            if (TextAndTopButtonsVisibility == Visibility.Collapsed)
            {
                TextAndTopButtonsVisibility = Visibility.Visible;
                return;
            }

            PlayNextStoryCommandCore();
        }

        private void PlayNextStoryCommandDoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            if (TextAndTopButtonsVisibility == Visibility.Collapsed)
            {
                TextAndTopButtonsVisibility = Visibility.Visible;
                return;
            }

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
                    StoryHistoryList.Add(new StoryHistoryItem(stwn.Name, stwn.Text, StoryHistoryItemType.StoryText));
                    break;
                case ShowPlainTextCommand spt:
                    StoryText = spt.Text;
                    CharacterName = string.Empty;
                    StoryHistoryList.Add(new StoryHistoryItem(string.Empty, spt.Text, StoryHistoryItemType.StoryText));
                    break;
                case ShowSubtitleCommand ss:
                    StorySubtitle = ss.Text;
                    StoryHistoryList.Add(new StoryHistoryItem(string.Empty, ss.Text, StoryHistoryItemType.StoryText));
                    break;
                case HideSubtitleCommand _:
                    StorySubtitle = string.Empty;
                    break;
                case DecisionCommand dec:
                    //TODO: Apply it in user interface
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (var item in dec.AvailableOptions)
                    {
                        stringBuilder.AppendLine($"[{item}]");
                        var textCmd = from text in dec[item] where text is TextCommand select (TextCommand)text;
                        foreach (var itemText in textCmd)
                        {
                            if (itemText is ShowTextWithNameCommand stwn)
                            {
                                StoryHistoryList.Add(new StoryHistoryItem(stwn.Name, stwn.Text, StoryHistoryItemType.Decision));
                            }
                            else
                            {
                                StoryHistoryList.Add(new StoryHistoryItem(string.Empty, itemText.Text, StoryHistoryItemType.Decision));
                            }
                        }
                    }
                    break;
                default:
                    CharacterName = cmd.GetType().Name;
                    StoryText = cmd.ToString();
                    break;
            }
        }

        private void OnStoryAutoPlayTimerTick(object sender, object e)
        {
            if (StoryText.Length < 10)
            {
                StoryAutoPlayTimer.Interval = new TimeSpan(0, 0, 5);
            }
            else if (PlayRatio == 2d)
            {
                StoryAutoPlayTimer.Interval = new TimeSpan(0, 0, StoryText.Length / 4);
            }
            else
            {
                StoryAutoPlayTimer.Interval = new TimeSpan(0, 0, StoryText.Length / 2);
            }

            PlayNextStoryCommandCore();
        }

        private void StartAutoPlay() => StoryAutoPlayTimer.Start();

        private void StopAutoPlay() => StoryAutoPlayTimer.Stop();

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

        private void HideTextAndTopButtons(object sender, RoutedEventArgs e) => TextAndTopButtonsVisibility = Visibility.Collapsed;

        private void ShowStoryHistory(object sender, RoutedEventArgs e) => LoadStoryHistory = true;

        private void HideStoryHistory(object sender, RoutedEventArgs e) => LoadStoryHistory = false;

        internal static Brush GetStoryHistoryItemColor(StoryHistoryItemType type)
        {
            switch (type)
            {
                case StoryHistoryItemType.Decision:
                    //#00a0eb
                    return new SolidColorBrush(new Windows.UI.Color() { R = 0, G = 160, B = 234 });
                case StoryHistoryItemType.StoryText:
                default:
                    //#ffd801
                    return new SolidColorBrush(new Windows.UI.Color() { R = 255, G = 216, B = 1 });
            }
        }
    }
}

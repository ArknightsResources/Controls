namespace ArknightsResources.Controls.Uwp.Models
{
    /// <summary>
    /// 表示一个剧情文本命令的记录
    /// </summary>
    internal readonly struct StoryHistoryItem
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 剧情文本
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// 该记录的类型
        /// </summary>
        public StoryHistoryItemType Type { get; }

        /// <summary>
        /// 构造<see cref="StoryHistoryItem"/>的新实例
        /// </summary>
        /// <param name="name">角色名称</param>
        /// <param name="text">剧情文本</param>
        public StoryHistoryItem(string name, string text, StoryHistoryItemType type)
        {
            Name = name;
            Text = text;
            Type = type;
        }

        public override string ToString()
        {
            return $"{Name}:{Text}";
        }
    }

    internal enum StoryHistoryItemType
    {
        /// <summary>
        /// 普通剧情文本
        /// </summary>
        StoryText,
        /// <summary>
        /// 剧情选择
        /// </summary>
        Decision
    }
}

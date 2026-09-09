using Telegram.Bot.Types.ReplyMarkups;

namespace SmartHome.Telegram.Keyboards;

/// <summary>A rendered bot screen: the message text and its inline keyboard.</summary>
public sealed record Screen(string Text, InlineKeyboardMarkup Keyboard);

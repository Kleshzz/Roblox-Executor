using System;
using System.Drawing;
using System.Text.RegularExpressions;
using FastColoredTextBoxNS;

namespace RobloxExecutor.UI.Controls
{
    public static class LuaStyle
    {
        // Мягкие цвета для тёмной темы
        private static readonly TextStyle KeywordStyle = new TextStyle(new SolidBrush(Color.FromArgb(180, 120, 200)), null, FontStyle.Bold);
        private static readonly TextStyle FunctionStyle = new TextStyle(new SolidBrush(Color.FromArgb(100, 160, 220)), null, FontStyle.Regular);
        private static readonly TextStyle StringStyle = new TextStyle(new SolidBrush(Color.FromArgb(150, 200, 120)), null, FontStyle.Regular);
        private static readonly TextStyle CommentStyle = new TextStyle(new SolidBrush(Color.FromArgb(100, 100, 100)), null, FontStyle.Italic);
        private static readonly TextStyle NumberStyle = new TextStyle(new SolidBrush(Color.FromArgb(200, 160, 100)), null, FontStyle.Regular);
        private static readonly TextStyle DefaultStyle = new TextStyle(new SolidBrush(Color.FromArgb(220, 220, 220)), null, FontStyle.Regular);

        public static void Apply(FastColoredTextBox editor)
        {
            // Отключаем встроенную подсветку Lua, чтобы она не конфликтовала
            editor.Language = Language.Custom;
            
            // Подписываемся на событие изменения текста для применения нашей подсветки
            editor.TextChanged += (sender, e) =>
            {
                var target = e.ChangedRange;
                
                // Очищаем старые стили
                target.ClearStyle(KeywordStyle, FunctionStyle, StringStyle, CommentStyle, NumberStyle);

                // 1. Комментарии (важно делать первыми или учитывать приоритет)
                target.SetStyle(CommentStyle, @"--.*$", RegexOptions.Multiline);
                target.SetStyle(CommentStyle, @"--\[\[.*?\]\]", RegexOptions.Singleline);

                // 2. Строки
                target.SetStyle(StringStyle, @"""""|''|""[\s\S]*?""|'[\s\S]*?'|\[\[[\s\S]*?\]\]");

                // 3. Числа
                target.SetStyle(NumberStyle, @"\b\d+[\.]?\d*\b");

                // 4. Ключевые слова
                target.SetStyle(KeywordStyle, @"\b(and|break|do|else|elseif|end|false|for|function|if|in|local|nil|not|or|repeat|return|then|true|until|while|continue|export|type|typeof)\b");

                // 5. Встроенные функции и Roblox API
                target.SetStyle(FunctionStyle, @"\b(print|warn|error|assert|pcall|xpcall|select|unpack|next|pairs|ipairs|tostring|tonumber|type|setmetatable|getmetatable|require|tick|time|wait|delay|spawn|game|workspace|script|Enum|task|math|table|string|os|debug)\b");
            };

            // Принудительно обновляем текст для применения стилей сразу
            editor.OnTextChanged();
        }
    }
}

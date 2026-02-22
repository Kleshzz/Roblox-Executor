using FastColoredTextBoxNS;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RobloxExecutor.UI.Controls
{
    public static class LuaAutocomplete
    {
        private static AutocompleteMenu menu;
        private static ImageList imageList;

        private static readonly Color BackNormal = Color.FromArgb(30, 30, 35);
        private static readonly Color BackSelected = Color.FromArgb(55, 60, 85);
        private static readonly Color ForeMain = Color.FromArgb(220, 220, 220);
        private static readonly Font MenuFont = new Font("Segoe UI", 9f);

        public static void Init(FastColoredTextBox editor)
        {
            menu = new AutocompleteMenu(editor);
            menu.MinFragmentLength = 1;
            menu.AllowTabKey = true;
            menu.AppearInterval = 50;

            // Включить точку в шаблон поиска — чтобы math.floor, task.wait и т.д. работали
            menu.SearchPattern = @"[\w\.]+";

            // Тёмная тема
            menu.BackColor = BackNormal;
            menu.ForeColor = ForeMain;
            menu.SelectedColor = BackSelected;

            // Компактный размер
            menu.Items.MaximumSize = new Size(300, 180);
            menu.Items.Width = 280;
            menu.Font = MenuFont;

            SetupImageList();
            menu.ImageList = imageList;

            var items = new List<AutocompleteItem>();

            // ── Keywords ───────────────────────────────────────────
            Add(items, new[] {
                "and","break","do","else","elseif","end","false","for",
                "function","if","in","local","nil","not","or","repeat",
                "return","then","true","until","while","continue","export","type","typeof"
            }, "keyword", 1);

            // ── Lua Functions ──────────────────────────────────────
            Add(items, new[] {
                "print","warn","error","assert","pcall","xpcall","select","unpack",
                "next","pairs","ipairs","tostring","tonumber","type","setmetatable",
                "getmetatable","require","rawget","rawset","tick","time","wait","delay","spawn",
                "collectgarbage","load","loadstring","rawequal","rawlen"
            }, "function", 0);

            // ── Roblox API ─────────────────────────────────────────
            Add(items, new[] {
                "game","workspace","script","Enum",
                "Instance.new","Vector3.new","Vector2.new",
                "CFrame.new","CFrame.Angles",
                "Color3.new","Color3.fromRGB","Color3.fromHSV",
                "UDim2.new","UDim.new","Region3.new","Ray.new",
                "task.wait","task.spawn","task.delay","task.defer"
            }, "api", 2);

            // ── Roblox Services ────────────────────────────────────
            Add(items, new[] {
                "Players","HttpService","TweenService","RunService",
                "ReplicatedStorage","ServerStorage","Lighting","SoundService",
                "UserInputService","MarketplaceService","TeleportService",
                "Chat","Teams","PathfindingService",
                "DataStoreService","CollectionService"
            }, "service", 3);

            // ── Instance Methods ───────────────────────────────────
            Add(items, new[] {
                "Connect","Disconnect","Destroy","Clone",
                "FindFirstChild","WaitForChild","GetChildren","GetDescendants","GetService",
                "IsA","FireServer","InvokeServer","FireClient","InvokeClient",
                "FindFirstAncestor","FindFirstChildOfClass","GetAttribute","SetAttribute",
                "AddTag","HasTag","RemoveTag","GetTags"
            }, "method", 0);

            // ── Libraries ─────────────────────────────────────────
            AddLib(items, "math", new[] {
                "abs","acos","asin","atan","ceil","cos","deg",
                "exp","floor","huge","log","max","min","pi",
                "rad","random","randomseed","sin","sqrt","tan"
            });
            AddLib(items, "table", new[] {
                "insert","remove","concat","sort","find",
                "clear","pack","unpack","move","create"
            });
            AddLib(items, "string", new[] {
                "byte","char","find","format","gmatch","gsub",
                "len","lower","match","rep","reverse","sub","upper","split"
            });
            AddLib(items, "task", new[] { "wait", "spawn", "delay", "defer", "synchronize", "desynchronize" });
            AddLib(items, "os", new[] { "time", "clock", "date", "exit", "difftime" });

            menu.Items.SetAutocompleteItems(items);
        }


        // ──────────────────────────────────────────────────────────────────────
        private static void Add(List<AutocompleteItem> list, string[] words, string label, int icon)
        {
            foreach (var w in words)
                list.Add(new LabeledItem(w, label, icon));
        }

        private static void AddLib(List<AutocompleteItem> list, string lib, string[] fns)
        {
            foreach (var fn in fns)
                list.Add(new LabeledItem(lib + "." + fn, lib, 0));
        }

        // ──────────────────────────────────────────────────────────────────────
        private static void SetupImageList()
        {
            imageList = new ImageList
            {
                ImageSize = new Size(16, 16),
                ColorDepth = ColorDepth.Depth32Bit
            };
            imageList.Images.Add(MakeIcon("f", Color.FromArgb(174, 121, 233))); // 0: function/method
            imageList.Images.Add(MakeIcon("k", Color.FromArgb(86, 156, 214))); // 1: keyword
            imageList.Images.Add(MakeIcon("a", Color.FromArgb(214, 157, 133))); // 2: api
            imageList.Images.Add(MakeIcon("s", Color.FromArgb(73, 201, 144))); // 3: service
        }

        private static Bitmap MakeIcon(string letter, Color color)
        {
            var bmp = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                using (var path = new GraphicsPath())
                {
                    const int r = 4;
                    path.AddArc(1, 1, r, r, 180, 90);
                    path.AddArc(11, 1, r, r, 270, 90);
                    path.AddArc(11, 11, r, r, 0, 90);
                    path.AddArc(1, 11, r, r, 90, 90);
                    path.CloseFigure();

                    using (var fill = new SolidBrush(Color.FromArgb(55, color)))
                        g.FillPath(fill, path);
                    using (var pen = new Pen(color, 1f))
                        g.DrawPath(pen, path);
                }

                using (var f = new Font("Arial", 8, FontStyle.Bold))
                using (var tb = new SolidBrush(color))
                {
                    var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(letter, f, tb, new Rectangle(0, 0, 16, 17), sf);
                }
            }
            return bmp;
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    public class LabeledItem : AutocompleteItem
    {
        public readonly string Label;

        private Color _foreColor = Color.Transparent;
        private Color _backColor = Color.Transparent;

        public LabeledItem(string text, string label, int imageIndex) : base(text)
        {
            Label = label;
            ImageIndex = imageIndex;
            // Показываем текст + категорию справа через пробелы
            MenuText = string.IsNullOrEmpty(label)
                ? text
                : text.PadRight(24) + label;
        }

        // Цвет текста элемента (используется библиотекой при отрисовке)
        public override Color ForeColor
        {
            get => _foreColor;
            set => _foreColor = value;
        }

        // Цвет фона элемента (используется библиотекой при отрисовке)
        public override Color BackColor
        {
            get => _backColor;
            set => _backColor = value;
        }

        // В редактор вставляем только само слово
        public override string GetTextForReplace() => Text;
    }
}

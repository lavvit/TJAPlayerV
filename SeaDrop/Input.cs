using System.Text;
using static DxLibDLL.DX;

namespace SeaDrop
{
    /// <summary>
    /// 文字列入力を行う。
    /// </summary>
    public static class Input
    {
        /// <summary>
        /// 文字列入力の初期化を行う。
        /// </summary>
        /// <param name="maximumLength">最大文字数 (バイト)</param>
        /// <param name="escapeCancelable">エスケープキーで入力をキャンセルできるかどうか。</param>
        /// <param name="inputSingleOnly">半角文字のみ入力可能にするかどうか。</param>
        /// <param name="inputNumberOnly">数字のみ入力可能にするかどうか。</param>
        /// <param name="inputDoubleOnly">全角文字のみ入力可能にするかどうか。</param>
        /// <param name="multiLine">複数行入力にするかどうか。</param>
        public static void Init(ulong maximumLength = 512, bool escapeCancelable = true, bool inputSingleOnly = false, bool inputNumberOnly = false, bool inputDoubleOnly = false, bool multiLine = false)
        {
            MaximumLength = maximumLength;
            Handle = MakeKeyInput(MaximumLength,
                escapeCancelable ? 1 : 0,
                inputSingleOnly ? 1 : 0,
                inputNumberOnly ? 1 : 0,
                inputDoubleOnly ? 1 : 0,
                multiLine ? 1 : 0); ;
            Builder = new StringBuilder((int)MaximumLength);
            SetActiveKeyInput(Handle);
            IsEnable = true;
            NumMode = false;
            Text = "";
            start = "";
        }
        public static void Init(string startstr, ulong maximumLength = 512, bool escapeCancelable = true, bool inputSingleOnly = false, bool inputNumberOnly = false, bool inputDoubleOnly = false, bool multiLine = false)
        {
            Init(maximumLength, escapeCancelable, inputSingleOnly, inputNumberOnly, inputDoubleOnly, multiLine);
            Text = startstr;
            start = startstr;
        }
        public static void InitNum(int start, int? min = null, int? max = null, EKey up = EKey.Up, EKey down = EKey.Down)
        {
            Init(512, true, false, true, false, false);
            NumMode = true;
            NNow = start;
            NMin = min;
            NMax = max;
            NUp = up;
            NDown = down;
        }

        public static void Draw(int x = 0, int y = 0, int color = 0xffffff, Handle? handle = null)
        {
            if (!IsEnable && Position < 0) return;

            int width = Drawing.TextSize(Text, -1, handle).Width;
            int height = Drawing.TextSize("H", -1, handle).Height;
            if (Selection.Start >= 0)
            {
                var start = Drawing.TextSize(Text, Selection.Start, handle).Width;
                string texwidth = Selection.End > Selection.Start ? Text.Substring(Selection.Start, Selection.End - Selection.Start) : Text.Substring(Selection.End, Selection.Start - Selection.End);
                var end = Drawing.TextSize(texwidth, -1, handle).Width * (Selection.End > Selection.Start ? 1 : -1);
                Drawing.Box(start + x, y, end, height, 0x0000ff);
            }

            Drawing.Text(x, y, Text, color, handle);
            int curcol = 0xffffff;
            int mode = GetIMEInputModeStr(Builder);
            var cursor = Drawing.TextSize(Text, Position, handle);
            switch (mode)
            {
                case 0:
                    curcol = 0xffff00;
                    break;
            }
            if (!NumMode || Position > 0) Drawing.Line(x + cursor.Width + 2, y, 0, height, curcol, 3);

            x += width;
            DrawIMEInputString(x + 4, y + (height - 16), Handle);
        }

        public static string? Enter()
        {
            if (!IsEnable) return null;
            if (NumMode)
            {
                if (Key.IsPushed(NUp))
                {
                    NNow++;
                    if (NMax != null && NNow >= NMax) NNow = NMax.Value;
                }
                if (Key.IsPushed(NDown))
                {
                    NNow--;
                    if (NMin != null && NNow <= NMin) NNow = NMin.Value;
                }
            }
            if (Text.Contains("\u0001"))
            {
                Text = Text.Substring(0, Text.Length - 1);
                Selection = (0, Text.Length);
            }
            if (GetDragFileNum() > 0)
            {
                StringBuilder sb = new StringBuilder("", 256);
                if (GetDragFilePath(sb) == 0)
                {
                    var dir = new Uri($"{Environment.ProcessPath}");
                    string absPath = sb.ToString();
                    string rel = dir.MakeRelativeUri(new Uri(sb.ToString())).ToString();
                    if (PathAppend) Text += rel;
                    else Text = rel;
                }
            }
            if (IsEnable && CheckKeyInput(Handle) != 0)
            {
                string? str = CheckKeyInput(Handle) != 2 ? Text : start;
                if (str == null) str = "";
                End();
                return str;
            }
            return null;
        }

        /// <summary>
        /// 文字入力を終了する。
        /// </summary>
        public static void End()
        {
            if (IsEnable)
            {
                var result = DeleteKeyInput(Handle);
                if (result == 0)
                {
                    IsEnable = false;
                    Builder = null;
                }
            }
            SetUseIMEFlag(FALSE);
        }

        /// <summary>
        /// 現在のキー入力の状態。
        /// </summary>
        public static KeyInputState KeyInputState
        {
            get
            {
                var result = CheckKeyInput(Handle);
                switch (result)
                {
                    case 0:
                        return KeyInputState.Typing;
                    case 1:
                        return KeyInputState.Finished;
                    case 2:
                        return KeyInputState.Canceled;
                    case -1:
                    default:
                        return KeyInputState.Error;
                }
            }
        }

        /// <summary>
        /// テキスト。
        /// </summary>
        public static string Text
        {
            get
            {
                try
                {
                    GetKeyInputString(Builder, Handle);
                    if (NumMode && string.IsNullOrEmpty(Builder != null ? Builder.ToString() : ""))
                    {
                        return NNow.ToString();
                    }
                    else
                    {
                        return Builder != null ? Builder.ToString() : "";
                    }
                }
                catch { return ""; }
            }
            set
            {
                SetKeyInputString(value, Handle);
            }
        }
        /*public static string IMEText
        {
            get
            {
                try
                {
                    GetKeyInputString(Builder, Handle);
                    return Builder != null ? Builder.ToString() : "";
                }
                catch { return ""; }
            }
            set
            {
                SetIMEInputString(value);
            }
        }*/

        /// <summary>
        /// 現在位置。
        /// </summary>
        public static int Position
        {
            get
            {
                return GetKeyInputCursorPosition(Handle);
            }
            set
            {
                SetKeyInputCursorPosition(value, Handle);
            }
        }

        /// <summary>
        /// 選択範囲。
        /// </summary>
        public static (int Start, int End) Selection
        {
            get
            {
                GetKeyInputSelectArea(out var s, out var e, Handle);
                return (s, e);
            }
            set
            {
                SetKeyInputSelectArea(value.Start, value.End, Handle);
            }
        }

        /// <summary>
        /// 有効かどうか。
        /// </summary>
        public static bool IsEnable { get; private set; }
        private static StringBuilder? Builder;
        private static int Handle;
        private static ulong MaximumLength;
        private static string? start;
        public static bool PathAppend = false;

        private static bool NumMode = false;
        private static int NNow = 0;
        private static int? NMin = null, NMax = null;
        private static EKey NUp = EKey.Up, NDown = EKey.Down;
    }

    /// <summary>
    /// キー入力の状態。
    /// </summary>
    public enum KeyInputState
    {
        /// <summary>
        /// タイピング中。
        /// </summary>
        Typing,
        /// <summary>
        /// 完了。
        /// </summary>
        Finished,
        /// <summary>
        /// キャンセルされた。
        /// </summary>
        Canceled,
        /// <summary>
        /// エラー。
        /// </summary>
        Error
    }
}
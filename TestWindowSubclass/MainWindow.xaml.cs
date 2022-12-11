// ============================================================================
// 
// メインウィンドウのコードビハインド
// 
// ============================================================================

// ----------------------------------------------------------------------------
// 
// ----------------------------------------------------------------------------

using System.Runtime.InteropServices;

using PInvoke;

using TestWindowSubclass.Helpers;

using Windows.Graphics;

using WinRT.Interop;

namespace TestWindowSubclass;

public sealed partial class MainWindow : WindowEx
{
    // ====================================================================
    // コンストラクター
    // ====================================================================

    /// <summary>
    /// メインコンストラクター
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        // 各種初期化
        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
        Content = null;
        Title = "AppDisplayName".GetLocalized();
        IntPtr hWnd = WindowNative.GetWindowHandle(this);
        ShowHelpButton(hWnd);

        // サブクラス化
        _subclassProc = new SubclassProc(CustomSubclassProc);
        SetWindowSubclass(hWnd, _subclassProc, IntPtr.Zero, IntPtr.Zero);
    }

    // ====================================================================
    // P/Invoke
    // ====================================================================

    /// <summary>
    /// コールバック関数プロトタイプ
    /// https://learn.microsoft.com/ja-jp/windows/win32/api/commctrl/nc-commctrl-subclassproc
    /// </summary>
    /// <param name="hWnd">ウィンドウハンドル</param>
    /// <param name="msg">メッセージ</param>
    /// <param name="wPalam">追加のメッセージ情報</param>
    /// <param name="lParam">追加のメッセージ情報</param>
    /// <param name="idSubclass">サブクラス ID</param>
    /// <param name="refData">SetWindowSubclass() で提供される参照データ</param>
    /// <returns></returns>
    internal delegate IntPtr SubclassProc(IntPtr hWnd, User32.WindowMessage msg, IntPtr wPalam, IntPtr lParam, IntPtr idSubclass, IntPtr refData);

    /// <summary>
    /// 次のハンドラーを呼び出す
    /// https://learn.microsoft.com/ja-jp/windows/win32/api/commctrl/nf-commctrl-defsubclassproc
    /// </summary>
    /// <param name="hWnd">ウィンドウハンドル</param>
    /// <param name="msg">メッセージ</param>
    /// <param name="wPalam">追加のメッセージ情報</param>
    /// <param name="lParam">追加のメッセージ情報</param>
    /// <returns></returns>
    [DllImport(FILE_NAME_COMCTL32_DLL)]
    internal static extern IntPtr DefSubclassProc(IntPtr hWnd, User32.WindowMessage msg, IntPtr wPalam, IntPtr lParam);

    /// <summary>
    /// ウィンドウサブクラスコールバックを設定
    /// https://learn.microsoft.com/ja-jp/windows/win32/api/commctrl/nf-commctrl-setwindowsubclass
    /// </summary>
    /// <param name="hWnd">ウィンドウハンドル</param>
    /// <param name="subclassProc">ウィンドウプロシージャー</param>
    /// <param name="idSubclass">サブクラス ID</param>
    /// <param name="refData">参照データ</param>
    /// <returns></returns>
    [DllImport(FILE_NAME_COMCTL32_DLL)]
    internal static extern Boolean SetWindowSubclass(IntPtr hWnd, SubclassProc subclassProc, IntPtr idSubclass, IntPtr refData);

    // ====================================================================
    // private 定数
    // ====================================================================

    /// <summary>
    /// DLL ファイル名
    /// </summary>
    private const String FILE_NAME_COMCTL32_DLL = "Comctl32.dll";

    /// <summary>
    /// ウィンドウサイズ変更の方向転換位置
    /// </summary>
    private const Int32 TURN_MIN = 400;
    private const Int32 TURN_MAX = 600;

    // ====================================================================
    // private 変数
    // ====================================================================

    /// <summary>
    /// カスタムウィンドウプロシージャーを保持
    /// </summary>
    private readonly SubclassProc _subclassProc;

    /// <summary>
    /// ウィンドウサイズ変更量
    /// </summary>
    private Int32 _delta = 10;

    // ====================================================================
    // private 関数
    // ====================================================================

    /// <summary>
    /// カスタムウィンドウプロシージャー
    /// </summary>
    /// <param name="hWnd">ウィンドウハンドル</param>
    /// <param name="msg">メッセージ</param>
    /// <param name="wPalam">追加のメッセージ情報</param>
    /// <param name="lParam">追加のメッセージ情報</param>
    /// <returns></returns>
    private IntPtr CustomSubclassProc(IntPtr hWnd, User32.WindowMessage msg, IntPtr wPalam, IntPtr lParam, IntPtr _1, IntPtr _2)
    {
        switch (msg)
        {
            case User32.WindowMessage.WM_SYSCOMMAND:
                if ((User32.SysCommands)wPalam == User32.SysCommands.SC_CONTEXTHELP)
                {
                    // ヘルプボタンの場合は自前処理
                    if (AppWindow.Size.Height < TURN_MIN && _delta < 0)
                    {
                        _delta = -_delta;
                    }
                    else if (AppWindow.Size.Height > TURN_MAX && _delta > 0)
                    {
                        _delta = -_delta;
                    }
                    AppWindow.Resize(new SizeInt32(AppWindow.Size.Width, AppWindow.Size.Height + _delta));
                    return IntPtr.Zero;
                }

                // ヘルプボタン以外は次のハンドラーにお任せ
                return DefSubclassProc(hWnd, msg, wPalam, lParam);
            default:
                // WM_SYSCOMMAND 以外は次のハンドラーにお任せ
                return DefSubclassProc(hWnd, msg, wPalam, lParam);
        }
    }

    /// <summary>
    /// タイトルバーにヘルプボタンを表示
    /// </summary>
    /// <param name="hWnd">ウィンドウハンドル</param>
    private void ShowHelpButton(IntPtr hWnd)
    {
        User32.SetWindowLongFlags exStyle = (User32.SetWindowLongFlags)User32.GetWindowLong(hWnd, User32.WindowLongIndexFlags.GWL_EXSTYLE);
        User32.SetWindowLong(hWnd, User32.WindowLongIndexFlags.GWL_EXSTYLE, exStyle | User32.SetWindowLongFlags.WS_EX_CONTEXTHELP);
    }
}

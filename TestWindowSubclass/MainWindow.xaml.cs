// ============================================================================
// 
// メインウィンドウのコードビハインド
// 
// ============================================================================

// ----------------------------------------------------------------------------
// 
// ----------------------------------------------------------------------------

using TestWindowSubclass.Helpers;

using Windows.Graphics;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;

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
        HWND hWnd = (HWND)WindowNative.GetWindowHandle(this);
        ShowHelpButton(hWnd);

        // サブクラス化
        _subclassProc = new SUBCLASSPROC(CustomSubclassProc);
        PInvoke.SetWindowSubclass(hWnd, _subclassProc, UIntPtr.Zero, UIntPtr.Zero);
    }

    // ====================================================================
    // private 定数
    // ====================================================================

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
    private readonly SUBCLASSPROC _subclassProc;

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
    private LRESULT CustomSubclassProc(HWND hWnd, UInt32 msg, WPARAM wPalam, LPARAM lParam, UIntPtr _1, UIntPtr _2)
    {
        switch (msg)
        {
            case PInvoke.WM_SYSCOMMAND:
                if ((UInt32)wPalam == PInvoke.SC_CONTEXTHELP)
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
                    return (LRESULT)IntPtr.Zero;
                }

                // ヘルプボタン以外は次のハンドラーにお任せ
                return PInvoke.DefSubclassProc(hWnd, msg, wPalam, lParam);
            default:
                // WM_SYSCOMMAND 以外は次のハンドラーにお任せ
                return PInvoke.DefSubclassProc(hWnd, msg, wPalam, lParam);
        }
    }

    /// <summary>
    /// タイトルバーにヘルプボタンを表示
    /// </summary>
    /// <param name="hWnd">ウィンドウハンドル</param>
    private void ShowHelpButton(HWND hWnd)
    {
        Int32 exStyle = PInvoke.GetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
        PInvoke.SetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, exStyle | (Int32)WINDOW_EX_STYLE.WS_EX_CONTEXTHELP);
    }
}

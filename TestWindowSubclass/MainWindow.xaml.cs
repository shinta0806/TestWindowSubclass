// ============================================================================
// 
// メインウィンドウのコードビハインド
// 
// ============================================================================

// ----------------------------------------------------------------------------
// 
// ----------------------------------------------------------------------------

using Microsoft.UI.Xaml;
using PInvoke;
using TestWindowSubclass.Helpers;
using Windows.System;
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
    }

    // ====================================================================
    // private 関数
    // ====================================================================

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

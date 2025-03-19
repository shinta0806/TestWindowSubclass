// ============================================================================
// 
// メインウィンドウのコードビハインド
// 
// ============================================================================

// ----------------------------------------------------------------------------
// 
// ----------------------------------------------------------------------------

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Windows.Graphics;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

using WinRT.Interop;

namespace TestWindowSubclass.Views.MainWindows;

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
		_instance = this;
		AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
		Content = new MainPage();
		Title = "TestWindowSubclass";
		HWND hWnd = (HWND)WindowNative.GetWindowHandle(this);
		ShowHelpButton(hWnd);

		// サブクラス化
		unsafe
		{
			delegate* unmanaged[Stdcall]<HWND, UInt32, WPARAM, LPARAM, nuint, nuint, LRESULT> subclassProc = &CustomSubclassProc;
			PInvoke.SetWindowSubclass(hWnd, subclassProc, UIntPtr.Zero, UIntPtr.Zero);
		}
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
	/// CustomSubclassProc からのアクセス用
	/// </summary>
	private static MainWindow? _instance;

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
	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private static LRESULT CustomSubclassProc(HWND hWnd, UInt32 msg, WPARAM wPalam, LPARAM lParam, nuint _1, nuint _2)
	{
		switch (msg)
		{
			case PInvoke.WM_SYSCOMMAND:
				if ((UInt32)wPalam == PInvoke.SC_CONTEXTHELP && _instance != null)
				{
					// ヘルプボタンの場合は自前処理
					if (_instance.AppWindow.Size.Height < TURN_MIN && _instance._delta < 0)
					{
						_instance._delta = -_instance._delta;
					}
					else if (_instance.AppWindow.Size.Height > TURN_MAX && _instance._delta > 0)
					{
						_instance._delta = -_instance._delta;
					}
					_instance.AppWindow.Resize(new SizeInt32(_instance.AppWindow.Size.Width, _instance.AppWindow.Size.Height + _instance._delta));
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
	private static void ShowHelpButton(HWND hWnd)
	{
		Int32 exStyle = PInvoke.GetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
		_ = PInvoke.SetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, exStyle | (Int32)WINDOW_EX_STYLE.WS_EX_CONTEXTHELP);
	}
}

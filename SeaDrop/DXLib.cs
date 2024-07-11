using System.Reflection;
using System.Text;
using static DxLibDLL.DX;

namespace SeaDrop
{
    public class DXLib
    {
        public static string AppPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "";

        public static int Width, Height;
        public static bool VSync = true, MultiThreading = true;
        public static bool Logging = false;

        private static bool isEnd, ProgEnd, TEnd;

        private static Scene? BaseScene, BaseOld, NowScene, OldScene;

        public static void Init(Scene scene, int width = 640, int height = 480, double scale = 1.0)
        {

            //SetFullScreenScalingMode(DX_FSSCALINGMODE_NEAREST); // フルスクリーンモード時の画面の拡大モードを最近点モードに設定

            ChangeWindowMode(TRUE); // ウインドウモードに変更
            SetWaitVSyncFlag(FALSE); // 垂直同期を無効化
            SetMultiThreadFlag(TRUE);//マルチスレッドを有効化する
            SetAlwaysRunFlag(TRUE); //ソフトがアクティブじゃなくても処理続行するようにする
            SetDoubleStartValidFlag(TRUE);//複数起動を許可する
            SetOutApplicationLogValidFlag(Logging ? 1 : 0);

            SetUseDxLibWM_PAINTProcess(FALSE);//ウィンドウを掴んでも動くようにするおまじない
            SetUseTSFFlag(TRUE);// ＩＭＥの変換候補表示の処理に TSF を使用するかどうかを設定する
            SetUseIMEFlag(TRUE);// ＩＭＥを使用するかどうかを設定する

            SetWindowSizeChangeEnableFlag(TRUE, TRUE);
            SetSize(width, height, scale);
            if (DxLib_Init() < 0) return; // ＤＸライブラリ初期化処理 エラーが起きたら直ちに終了

            if (MultiThreading) BaseScene = scene;
            SceneChange(scene, false);
            Update(); // メイン処理
            EndApp(); //終了処理
        }
        private static void Update()
        {
            if (MultiThreading)
            {
                #region マルチスレッド
                Task.Run(() =>
                {
                    try
                    {
                        // EndFlag が０の間はループ
                        while (!ProgEnd)
                        {
                            WaitVSync(VSync ? 1 : 0);
                            Updates();
                        }
                    }
                    catch (Exception)
                    {
                        isEnd = true;
                    }
                });
                Task.Run(() =>
                {
                    try
                    {
                        // 描画先を裏画面に
                        SetDrawScreen(DX_SCREEN_BACK);

                        // EndFlag が０の間はループ
                        while (!ProgEnd)
                        {
                            // 画面を初期化
                            ClearDrawScreen();
                            WaitVSync(VSync ? 1 : 0);

                            FPS.Update();
                            BaseScene?.Draw();
                            NowScene?.Draw();

                            // 裏画面の内容を表画面に反映
                            ScreenFlip();
                        }
                    }
                    catch (Exception)
                    {
                        isEnd = true;
                    }
                    TEnd = true;
                });

                // ProcessMessage ループ
                while (ProcessMessage() == 0)
                {

                    if (isEnd) break;
                }

                // プログラムが終了したフラグを立てる
                ProgEnd = true;

                // 読み込みスレッドが終了するのを待つ
                while (!TEnd)
                {
                    Thread.Sleep(10);
                }
                #endregion
            }
            else
            {
                // メッセージループに代わる処理をする
                while (ProcessMessage() == 0 && ScreenFlip() == 0 && ClearDrawScreen() == 0 && !isEnd)//
                {
                    WaitVSync(VSync ? 1 : 0);

                    FPS.Update();
                    Updates();
                    BaseScene?.Draw();
                    NowScene?.Draw();
                }
            }
        }
        private static void Updates()
        {
            Key.Update();
            FPS.Update(true);
            if (NowScene != null) NowScene?.Update();
            else BaseScene?.Update();

            if (!Input.IsEnable && GetDragFileNum() > 0)
            {
                StringBuilder sb = new StringBuilder("", 256);
                if (GetDragFilePath(sb) == 0)
                {
                    if (NowScene != null) NowScene?.Drag(sb.ToString());
                    else BaseScene?.Drag(sb.ToString());
                }
            }

            Task.Run(Tcp.UpdateServer);
            Task.Run(Udp.UpdateServer);
        }

        public static void End()
        {
            isEnd = true;
        }
        private static void EndApp()
        {
            Tcp.EndServer();
            Internet.Dispose();

            DxLib_End();        // ＤＸライブラリ使用の終了処理
            Environment.Exit(0);
        }
        public static void SceneChange(Scene scene, bool layer = false, bool threading = true, bool instant = false)
        {
            GC.Collect();
            if (threading)
            {
                Task.Run(() =>
            {
                scene?.Enable();
                if (!layer)
                {
                    BaseOld = BaseScene;
                    BaseOld?.Disable();
                    if (!instant) BaseScene = scene;
                }
                else
                {
                    OldScene = NowScene;
                    OldScene?.Disable();
                    if (!instant) NowScene = scene;
                }
            });
                if (instant)
                {
                    if (!layer) BaseScene = scene;
                    else NowScene = scene;
                }
            }
            else
            {
                scene?.Enable();
                if (!layer)
                {
                    BaseOld = BaseScene;
                    BaseOld?.Disable();
                    BaseScene = scene;
                }
                else
                {
                    OldScene = NowScene;
                    OldScene?.Disable();
                    NowScene = scene;
                }
            }
        }

        public static void SetSize(int width = 1920, int height = 1080, double scale = 1.0)
        {
            SetGraphMode(width, height, 32); //ゲームサイズ決める
            SetWindowSizeExtendRate(scale); //起動時のウィンドウサイズを設定 ( 1 = 100%)
            Width = width;
            Height = height;
            if (DxLib_Init() < 0) return;
            SetDrawScreen(DX_SCREEN_BACK);
        }

        public static void SetDrop(bool enable)
        {
            SetDragFileValidFlag(enable ? 1 : 0);
        }

        public static void Error(object e, string str = "", string errorname = null)
        {
            if (!string.IsNullOrEmpty(str)) str += "\n";
            if (!string.IsNullOrEmpty(errorname)) errorname = " : " + errorname;

            throw new Exception(e.ToString() + str + errorname);
        }
    }
}
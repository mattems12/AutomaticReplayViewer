using InputManager;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

class BHReplayViewer : ReplayViewer
{
    public BHReplayViewer() : base("Brawlhalla", "Brawlhalla", false)
    {
        Title = (Bitmap) Image.FromFile("Images/Title.png");
        ReplaysLoaded = (Bitmap)Image.FromFile("Images/ReplaysLoaded.png");
    }

    public void StartLoop(int ReplaysToPlay, Keys RecordStart, Keys RecordStop)
    {
        NoErrors = true;
        getProcess();

        if (NoErrors)
        {
            LoopThread = new Thread(() => PlaybackLoop(ReplaysToPlay, RecordStart, RecordStop));
            LoopThread.Start();
        }
        else
        {
            OnLoopEnd(new EventArgs());
        }
    }

    protected override void MenuStateActive(ref bool menu)
    {
        screen = ScreenGrabber.PrintWindow(hWnd);

        menu = ScreenGrabber.Contains(screen, Title);

        if (menu)
            return;
        else if (ScreenGrabber.Contains(screen, ReplaysLoaded))
        {
            Keyboard.KeyDown(Keys.Enter);
            Thread.Sleep(50);
            Keyboard.KeyUp(Keys.Enter);
        }
        else if (previousscreen == null)
        {
            previousscreen = screen;
            return;
        }
        else if (screen == previousscreen) // Make this better
        {
            Keyboard.KeyDown(Keys.Escape);
            Thread.Sleep(50);
            Keyboard.KeyUp(Keys.Escape);
            Thread.Sleep(50);
            Keyboard.KeyDown(Keys.Up);
            Thread.Sleep(50);
            Keyboard.KeyUp(Keys.Up);
            Thread.Sleep(50);
            Keyboard.KeyDown(Keys.Enter);
            Thread.Sleep(50);
            Keyboard.KeyUp(Keys.Enter);
        }
        previousscreen = screen;
    }

    protected override void NavigateDefault()
    {
        Mouse.Move(1320,43);    // TODO - account for possible different resolutions
        Thread.Sleep(20);
        Mouse.PressButton(Mouse.MouseKeys.Left);
    }

    protected override void GameNotOpen()
    {
        base.GameNotOpen();
        ProgressText = "Brawlhalla was not open";
        NoErrors = false;
    }

    protected override void InMatchInputs()
    {

    }

    private bool NoErrors;
    private Bitmap screen;
    private Bitmap previousscreen;
    private Bitmap Title;
    private Bitmap ReplaysLoaded;
}
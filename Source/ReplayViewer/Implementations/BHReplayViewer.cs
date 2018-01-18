using AForge.Imaging.Filters;
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

        if (previousscreen == null)
        {
            previousscreen = screen;
            return;
        }

        Difference diff = new Difference(screen);

        if (ScreenGrabber.IsBlack(diff.Apply(previousscreen)))
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
            return;
        }
        previousscreen = screen;
    }

    protected override void NavigateDefault()
    {
        Mouse.Move(1320, 43);    // TODO - account for possible different resolutions
        Thread.Sleep(20);
        Mouse.PressButton(Mouse.MouseKeys.Left);

        Thread.Sleep(100);

        // TODO - navigate to appropriate replay

        Keyboard.KeyDown(Keys.Enter);
        Thread.Sleep(50);
        Keyboard.KeyUp(Keys.Enter);
        Mouse.Move(9999, 0);
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
}
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
            previousscreen = ScreenGrabber.PrintWindow(hWnd);
            ResX = previousscreen.Width;
            ResY = previousscreen.Height;

            ResizeBicubic Resize = new ResizeBicubic(220*ResY/1080, 146*ResY/1080);
            Title = Resize.Apply(Title);

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

        /*
         List of resolutions on my machine that this wont work correctly for
         1280x1024
         1280x960
         1152x864
         1024x768
         800x600
         None of these are 16:9
         I think this is because the game decides to leave the 'bars' to fill out the vertical axis rather than the horizontal axis
         Therefore, the title image that the game screen is compared to is incorrectly scaled and the condition is never met
         TODO - figure out how the game decides whether to fill out the vertical or horizontal axis
                 - maybe it will decide to fill out the vertical if the screen is small enough?
                 - in the cases where it doesnt work the screen is filled out w black bars
                 - most of these are 4:3 hmmm
        */
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
        Mouse.Move(ResX - 65*ResY/1080, 61*ResY/1080);    // TODO - account for possible different resolutions - better but still incomplete
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

    private int ResX;
    private int ResY;
    private bool NoErrors;
    private Bitmap screen;
    private Bitmap previousscreen;
    private Bitmap Title;
}
using Avalonia.Controls;
using Avalonia.Input;
using ColorMC.Core.Objs;
using ColorMC.Gui.UI.Model.GameCloud;
using ColorMC.Gui.UI.Windows;
using System.Threading;

namespace ColorMC.Gui.UI.Controls.GameCloud;

public partial class GameCloudControl : UserControl, IUserControl
{
    private bool _switch1 = false;

    private readonly Tab1Control _tab1 = new();
    private readonly Tab2Control _tab2 = new();
    private readonly Tab3Control _tab3 = new();

    private CancellationTokenSource _cancel = new();

    private readonly GameCloudModel _model;

    private int _now;

    public GameSettingObj Obj { get; }

    public IBaseWindow Window => App.FindRoot(VisualRoot);
    public UserControl Con => this;
    public string Title =>
        string.Format(App.GetLanguage("GameCloudWindow.Title"), Obj.Name);

    public GameCloudControl() : this(new() { Empty = true })
    {

    }

    public GameCloudControl(GameSettingObj obj)
    {
        InitializeComponent();

        Obj = obj;

        if (!obj.Empty)
        {
            _model = new(this, obj);
            DataContext = _model;
        }

        Tabs.SelectionChanged += Tabs_SelectionChanged;

        ScrollViewer1.PointerWheelChanged += ScrollViewer1_PointerWheelChanged;

        Content1.Content = _tab1;
    }

    private void ScrollViewer1_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (e.Delta.Y > 0)
        {
            ScrollViewer1.LineLeft();
            ScrollViewer1.LineLeft();
            ScrollViewer1.LineLeft();
            ScrollViewer1.LineLeft();
            ScrollViewer1.LineLeft();
        }
        else if (e.Delta.Y < 0)
        {
            ScrollViewer1.LineRight();
            ScrollViewer1.LineRight();
            ScrollViewer1.LineRight();
            ScrollViewer1.LineRight();
            ScrollViewer1.LineRight();
        }
    }

    public void Opened()
    {
        Window.SetTitle(Title);

        _model.Load();
    }

    private void Tabs_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        switch (Tabs.SelectedIndex)
        {
            case 0:
                Go(_tab1);
                break;
            case 1:
                Go(_tab2);
                break;
            case 2:
                Go(_tab3);
                break;
        }

        _now = Tabs.SelectedIndex;
    }

    private void Go(UserControl to)
    {
        _cancel.Cancel();
        _cancel.Dispose();

        _cancel = new();
        Tabs.IsEnabled = false;

        if (!_switch1)
        {
            Content2.Content = to;
            _ = App.PageSlide500.Start(Content1, Content2, _now < Tabs.SelectedIndex, _cancel.Token);
        }
        else
        {
            Content1.Content = to;
            _ = App.PageSlide500.Start(Content2, Content1, _now < Tabs.SelectedIndex, _cancel.Token);
        }

        _switch1 = !_switch1;
        Tabs.IsEnabled = true;
    }

    public void Closed()
    {
        App.GameCloudWindows.Remove(_model.Obj.UUID);
    }
}

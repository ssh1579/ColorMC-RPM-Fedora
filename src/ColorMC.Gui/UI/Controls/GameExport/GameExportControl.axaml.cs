using Avalonia.Controls;
using Avalonia.Input;
using ColorMC.Core.LaunchPath;
using ColorMC.Core.Objs;
using ColorMC.Gui.UI.Model;
using ColorMC.Gui.UI.Model.GameExport;
using ColorMC.Gui.UI.Windows;
using System.IO;
using System.Threading;

namespace ColorMC.Gui.UI.Controls.GameExport;

public partial class GameExportControl : UserControl, IUserControl
{
    private bool _switch1 = false;

    private readonly Tab1Control _tab1 = new();
    private readonly Tab2Control _tab2 = new();
    private readonly Tab3Control _tab3 = new();
    private readonly Tab4Control _tab4 = new();

    private CancellationTokenSource _cancel = new();

    private int _now;

    private GameSettingObj _obj;

    public IBaseWindow Window => App.FindRoot(VisualRoot);
    public UserControl Con => this;
    public string Title =>
        string.Format(App.GetLanguage("GameExportWindow.Title"), _obj.Name);

    public GameExportControl()
    {
        InitializeComponent();
    }

    public GameExportControl(GameSettingObj obj) : this()
    {
        _obj = obj;

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

    public async void Opened()
    {
        Window.SetTitle(Title);

        _tab2.Opened();
        _tab4.Opened();

        var model = (DataContext as GameExportModel)!;
        await model.LoadMod();
        model.LoadFile();

        var icon = model.Obj.GetIconFile();
        if (File.Exists(icon))
        {
            Window.SetIcon(new(icon));
        }
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
            case 3:
                Go(_tab4);
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
        App.GameExportWindows.Remove(_obj.UUID);
    }

    public void SetBaseModel(BaseModel model)
    {
        var amodel = new GameExportModel(model, _obj);
        DataContext = amodel;
    }
}

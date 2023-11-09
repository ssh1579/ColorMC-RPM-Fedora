﻿using Avalonia.Controls;
using ColorMC.Gui.UI.Model.Items;
using ColorMC.Gui.UI.Model.ServerPack;
using System;

namespace ColorMC.Gui.UI.Flyouts;

public class ServerPackFlyout1
{
    private readonly ServerPackConfigModel _obj;
    private readonly ServerPackModel _model;
    public ServerPackFlyout1(Control con, ServerPackModel model, ServerPackConfigModel obj)
    {
        _model = model;
        _obj = obj;

        _ = new FlyoutsControl(new (string, bool, Action)[]
        {
            (App.Lang("Button.Delete"), true, Button1_Click),
        }, con);
    }

    private void Button1_Click()
    {
        _model.DeleteFile(_obj);
    }
}

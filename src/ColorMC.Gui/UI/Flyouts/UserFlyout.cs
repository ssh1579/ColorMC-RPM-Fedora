﻿using Avalonia.Controls;
using ColorMC.Core.Objs;
using ColorMC.Gui.Objs;
using ColorMC.Gui.UI.Model.User;
using System;

namespace ColorMC.Gui.UI.Flyouts;

public class UserFlyout
{
    private readonly UserDisplayObj _obj;
    private readonly UsersControlModel _model;
    public UserFlyout(Control con, UsersControlModel model)
    {
        _model = model;
        _obj = model.Item!;

        _ = new FlyoutsControl(new (string, bool, Action)[]
        {
            (App.Lang("UserWindow.Flyouts.Text1"), true, Button1_Click),
            (App.Lang("UserWindow.Flyouts.Text2"), _obj.AuthType != AuthType.Offline, Button2_Click),
            (App.Lang("UserWindow.Flyouts.Text3"), _obj.AuthType != AuthType.Offline
                && _obj.AuthType != AuthType.OAuth, Button4_Click),
            (App.Lang("UserWindow.Flyouts.Text4"), true, Button3_Click),
            (App.Lang("UserWindow.Flyouts.Text5"), _obj.AuthType == AuthType.Offline, Button5_Click)
        }, con);
    }

    private void Button5_Click()
    {
        _model.Edit(_obj);
    }

    private void Button4_Click()
    {
        _model.ReLogin(_obj);
    }

    private void Button3_Click()
    {
        _model.Remove(_obj);
    }

    private void Button2_Click()
    {
        _model.Refresh(_obj);
    }

    private void Button1_Click()
    {
        _model.Select(_obj);
    }
}

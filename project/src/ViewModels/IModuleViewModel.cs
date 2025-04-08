﻿using System.Windows.Input;
using CourseApp.Models;

public interface IModuleViewModel : IBaseViewModel
{
    Module CurrentModule { get; }
    bool IsCompleted { get; }
    ICommand CompleteModuleCommand { get; }
    ICommand OnModuleImageClick { get; }
    string TimeSpent { get; }
    int CoinBalance { get; }
}
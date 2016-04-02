﻿using System;
using ReactiveUI;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CodeHub.Core.Services
{
    public interface IActionMenuService
    {
        IActionMenu Create(string title = null);

        IPickerMenu CreatePicker();

        void ShareUrl(object sender, Uri uri);

        void SendToPasteBoard(string str);
    }

    public interface IActionMenu
    {
        void AddButton(string title, IReactiveCommand clickAction);

        Task Show(object sender);
    }

    public interface IPickerMenu
    {
        ICollection<string> Options { get; }

        int SelectedOption { get; set; }

        Task<int> Show(object sender);
    }

    public static class ActionMenuFactoryExtensions
    {
        public static void ShareUrl(this IActionMenuService @this, object sender, string uri)
        {
            @this.ShareUrl(sender, new Uri(uri));
        }
    }
}


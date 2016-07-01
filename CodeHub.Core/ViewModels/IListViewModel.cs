﻿using ReactiveUI;

namespace CodeHub.Core.ViewModels
{
    public interface IListViewModel<T>
    {
        IReadOnlyReactiveList<T> Items { get; }

        string SearchText { get; set; }

        bool IsEmpty { get; }
    }
}


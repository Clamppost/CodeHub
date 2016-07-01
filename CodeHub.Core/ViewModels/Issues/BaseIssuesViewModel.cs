using System;
using GitHubSharp.Models;
using CodeHub.Core.Filters;
using CodeHub.Core.ViewModels.PullRequests;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using CodeHub.Core.Utils;
using ReactiveUI;

namespace CodeHub.Core.ViewModels.Issues
{
    public abstract class BaseIssuesViewModel<TFilterModel> : LoadableViewModel, IBaseIssuesViewModel where TFilterModel : BaseIssuesFilterModel<TFilterModel>, new()
    {
        protected FilterableCollectionViewModel<IssueModel, TFilterModel> _issues;

        public FilterableCollectionViewModel<IssueModel, TFilterModel> Issues
        {
            get { return _issues; }
        }

        public IReactiveCommand<object> GoToIssueCommand { get; } = ReactiveCommand.Create();

        protected BaseIssuesViewModel()
        {
            GoToIssueCommand
                .OfType<IssueModel>()
                .Subscribe(x =>
                {
                    var isPullRequest = x.PullRequest != null && !(string.IsNullOrEmpty(x.PullRequest.HtmlUrl));
                    var s1 = x.Url.Substring(x.Url.IndexOf("/repos/", StringComparison.Ordinal) + 7);
                    var issuesIndex = s1.LastIndexOf("/issues", StringComparison.Ordinal);
                    issuesIndex = issuesIndex < 0 ? 0 : issuesIndex;
                    var repoId = RepositoryIdentifier.FromFullName(s1.Substring(0, issuesIndex));

                    if (repoId == null)
                        return;

                    if (isPullRequest)
                        NavigateTo(new PullRequestViewModel(repoId.Owner, repoId.Name, x.Number));
                    else
                        NavigateTo(new IssueViewModel(repoId.Owner, repoId.Name, x.Number));
                });
        }

        protected virtual List<IGrouping<string, IssueModel>> Group(IEnumerable<IssueModel> model)
        {
            var order = Issues.Filter.SortType;
            if (order == BaseIssuesFilterModel<TFilterModel>.Sort.Comments)
            {
                var a = Issues.Filter.Ascending ? model.OrderBy(x => x.Comments) : model.OrderByDescending(x => x.Comments);
                var g = a.GroupBy(x => FilterGroup.IntegerCeilings.First(r => r > x.Comments)).ToList();
                return FilterGroup.CreateNumberedGroup(g, "Comments");
            }
            if (order == BaseIssuesFilterModel<TFilterModel>.Sort.Updated)
            {
                var a = Issues.Filter.Ascending ? model.OrderBy(x => x.UpdatedAt) : model.OrderByDescending(x => x.UpdatedAt);
                var g = a.GroupBy(x => FilterGroup.IntegerCeilings.First(r => r > x.UpdatedAt.TotalDaysAgo()));
                return FilterGroup.CreateNumberedGroup(g, "Days Ago", "Updated");
            }
            if (order == BaseIssuesFilterModel<TFilterModel>.Sort.Created)
            {
                var a = Issues.Filter.Ascending ? model.OrderBy(x => x.CreatedAt) : model.OrderByDescending(x => x.CreatedAt);
                var g = a.GroupBy(x => FilterGroup.IntegerCeilings.First(r => r > x.CreatedAt.TotalDaysAgo()));
                return FilterGroup.CreateNumberedGroup(g, "Days Ago", "Created");
            }

            return null;
        }
    }

    public interface IBaseIssuesViewModel
    {
        IReactiveCommand<object> GoToIssueCommand { get; }
    }
}


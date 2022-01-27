using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using PackageDiff;

namespace PackageDiffComment
{
    internal static class Publisher
    {
        public static string Owner { get;set;} = "alnlarsen";
        public static string RepoName { get; set; } = "package-diff-comment";
        public static string Token { get; set; } = "";

        public static async Task Publish(string comment)
        {
            Console.WriteLine($"Publishing report:");
            Console.WriteLine(comment);

            var owner = Environment.GetEnvironmentVariable("GITHUB_REPOSITORY_OWNER");
            var reponame = Environment.GetEnvironmentVariable("GITHUB_REPOSITORY").Split("/").Last();
            var sha = Environment.GetEnvironmentVariable("GITHUB_SHA");

            var github = new GitHubClient(new ProductHeaderValue("package-diff-comment"))
            {
                Credentials = new Credentials(Token)
            };
            var repo = github.Repository.Get(owner, reponame).Result;
            var prs = await github.PullRequest.GetAllForRepository(repo.Id, new PullRequestRequest { State = ItemStateFilter.All });
            var commit = await github.Git.Commit.Get(repo.Id, sha);

            foreach (var pr in prs)
            {
                if (pr.Head.Sha != sha) continue;
                await github.Issue.Comment.Create(repo.Id, pr.Number, comment);
            }
        }
    }
}

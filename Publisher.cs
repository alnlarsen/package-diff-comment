using Octokit;

namespace PackageDiffComment
{
    internal static class Publisher
    {
        public static string Token { get; set; } = "";
        public static bool Called = false;
        public static object LockObj = new object();
        public static async Task Publish(string comment)
        {
            lock (LockObj)
            {
                if (Called) return;
                Called = true;
            }

            Console.WriteLine($"Publishing report:");
            Console.WriteLine(comment);

            Console.WriteLine("Reading repo data...");
            var owner = Environment.GetEnvironmentVariable("GITHUB_REPOSITORY_OWNER");
            Console.WriteLine($"Owner = {owner}");
            var reponame = Environment.GetEnvironmentVariable("GITHUB_REPOSITORY").Split("/").Last();
            Console.WriteLine($"Repo name = {reponame}");
            var sha = Environment.GetEnvironmentVariable("GITHUB_SHA");
            Console.WriteLine($"Commit sha = {sha}");

            var github = new GitHubClient(new ProductHeaderValue("package-diff-comment"))
            {
                Credentials = new Credentials(Token)
            };

            var repo = await github.Repository.Get(owner, reponame);
            var prs = await github.PullRequest.GetAllForRepository(repo.Id, new PullRequestRequest { State = ItemStateFilter.All });

            Console.WriteLine($"Found {prs.Count} pull requests.");

            foreach (var pr in prs)
            {
                if (pr.Head.Sha != sha) continue;
                Console.WriteLine($"Found PR with head == {sha}!");
                await github.Issue.Comment.Create(repo.Id, pr.Number, comment);
            }
        }
    }
}

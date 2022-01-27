using System.Collections.Concurrent;
using System.Text;
using OpenTap.Package;
using PackageDiff.ReportGenerators;

namespace PackageDiffComment;

public class MarkdownReportGenerator : IReportGenerator
{
    private StringBuilder report = new StringBuilder();
    public void Create(PackageDef lower, PackageDef higher, string name)
    {
        report.AppendLine($"Diff of {lower.Name} `{lower.Version} > {higher.Version}`");
    }

    class Category
    {
        public string category { get; set; }
        public List<string> Removed = new List<string>();
        public List<string> Added = new List<string>();
    }

    private ConcurrentDictionary<string, Category> categoryMap = new ConcurrentDictionary<string, Category>();

    private bool anyChanges = false;
    public void Added(string category, string item)
    {
        anyChanges = true;
        categoryMap.GetOrAdd(category, c => new Category() { category = c }).Added.Add(item);
    }

    public void Removed(string category, string item)
    {
        anyChanges = true;
        categoryMap.GetOrAdd(category, c => new Category() { category = c }).Removed.Add(item);
    }

    /// <summary>
    /// Post a comment to the PR with the content of <see cref="report"/>
    /// </summary>
    private void publish()
    {
        var result = report.ToString();
        Publisher.Publish(result).GetAwaiter().GetResult();
    }

    public void Finish()
    {
        if (!anyChanges)
        {
            report.AppendLine("No changes.");
            publish();
            return;
        }

        var keys = categoryMap.Keys.ToList();
        keys.Sort();

        report.AppendLine("<details>");

        foreach (var key in keys)
        {
            var category = categoryMap[key];
            report.AppendLine($"## {key}");
            report.AppendLine("```diff");
            foreach (var item in category.Added)
            {
                report.AppendLine($"+{item}");
            }
            foreach (var item in category.Removed)
            {
                report.AppendLine($"-{item}");
            }

            report.AppendLine("```");
        }
        report.AppendLine("</details>");
        publish();
    }
}
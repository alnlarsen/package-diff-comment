# Pull Request Version Comment Action

Github action that can comment on pull requests to indicate the diff of an OpenTAP package and the package from the branch from which it is derived.
This uses the OpenTAP package [Package Diff](http://packages.opentap.io/index.html#/?name=Package%20Diff&version=0.1.0-beta.16%2Bcae36d16&os=Windows,Linux&architecture=AnyCPU) and relies on semantic git versioning using `tap sdk gitversion` (see [OpenTAP docs](https://doc.opentap.io/Developer%20Guide/Plugin%20Packaging%20and%20Versioning/#git-assisted-versioning))

## Prerequisites

* A GitHub repository that follows a branching model compatible with the gitversion system of OpenTAP (see [OpenTAP docs](https://doc.opentap.io/Developer%20Guide/Plugin%20Packaging%20and%20Versioning/#git-assisted-versioning)).
* A .gitversion file in the root of the git repository
* A .TapPackage Artifact
* The packages from which they are derived to be available from a [package repository](https://packages.opentap.io)

## Usage

To use get comments on merged PRs in your GitHub repository, create a workflow (eg: `.github/workflows/package-diff-comment.yaml` see [Creating a Workflow file](https://help.github.com/en/articles/configuring-a-workflow#creating-a-workflow-file)) with content like below:

```yaml
push:
    branches:
      - 'main'

# This grants access to the GITHUB_TOKEN so the action can make calls to GitHub's rest API
permissions:
  contents: read
  pull-requests: write
  issues: write

jobs:
  package-diff-comment:
    runs-on: ubuntu-latest
    name: package-diff-comment
    steps:
      - name: Checkout
        uses: actions/checkout@v2
        with:
          # This action needs the entire history of the repository to calculate the version
          fetch-depth: 0
      - name: Run comment action
        uses: alnlarsen/package-diff-comment
        with:
          token: ${{ secrets.GITHUB_TOKEN }}
          # (Optional) Content of the comment to add to a merged pull request. Use {version} 
          # to insert the version number.
          body: This change is part of version `{version}` or later.
          # (Optional) Content of the comment to add to an issue that was closed as a 
          # consequence of a PR merge. Use {version} to insert the version number.
          issue-body: A fix for this is in version `{version}` or later.
```

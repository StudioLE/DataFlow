# StudioLE.DataFlow

Feed | Status 
------- | -------
Latest NuGet.org Release | [![Nuget](https://img.shields.io/nuget/v/StudioLE.Libraries.DataFlow)](https://www.nuget.org/packages/StudioLE.Libraries.DataFlow/)
Latest Production Release | [![Production feed in Azure Artifacts](https://feeds.dev.azure.com/StudioLE/_apis/public/Packaging/Feeds/bfe316ce-2b20-4a66-bc40-82df1b3caa61/Packages/cd2957dc-9faa-4bb7-904a-a1fd30b4d8b3/Badge)](https://dev.azure.com/StudioLE/DataFlow/_packaging?_a=package&feed=bfe316ce-2b20-4a66-bc40-82df1b3caa61&package=cd2957dc-9faa-4bb7-904a-a1fd30b4d8b3&preferRelease=true)
Latest GitHub Release | [![GitHub release](https://img.shields.io/github/v/release/StudioLE/DataFlow)](https://github.com/StudioLE/DataFlow/releases)
Azure Pipeline Build |  [![Build Status](https://dev.azure.com/StudioLE/DataFlow/_apis/build/status/StudioLE.DataFlow?branchName=master)](https://dev.azure.com/StudioLE/DataFlow/_build/latest?definitionId=4&branchName=master)
Azure Pipeline Coverage | [![Azure DevOps coverage](https://img.shields.io/azure-devops/coverage/StudioLE/DataFlow/4/master)](https://dev.azure.com/StudioLE/DataFlow/_build/latest?definitionId=4&branchName=master)
Azure Pipeline Tests | [![Azure DevOps tests (branch)](https://img.shields.io/azure-devops/tests/StudioLE/DataFlow/4/master)](https://dev.azure.com/StudioLE/DataFlow/_build/latest?definitionId=4&branchName=master)

This project is a sandbox for experimenting with DataFlow pipelines in C# based on the implementation set out in [this article](https://michaelscodingspot.com/pipeline-pattern-tpl-dataflow/).

If you're looking for an actual DataFlow library you should probably look elsewhere.

## Contributing

The `master` branch is protected to ensure the commit history is clean and linear.
- Contributions should be made to a new branch and a pull request created.
- Pull request checks must pass before merging. 
- If changes are minimal, merge should be by `squash`.
- Otherwise, merge by `rebase` (learn more about [Merging vs. Rebasing](https://www.atlassian.com/git/tutorials/merging-vs-rebasing)).
- Never, use merge commit.

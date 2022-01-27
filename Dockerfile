# Set the base image as the .NET 6.0 SDK (this includes the runtime)
FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env

# Copy everything and publish the release (publish implicitly restores and builds)
COPY . ./src
RUN dotnet publish ./src/package-diff-comment.csproj -c Release -o ./out --no-self-contained

# Label the container
LABEL maintainer="Alexander Larsen <alexander.larsen@keysight.com>"
LABEL repository="https://github.com/alnlarsen/package-diff-comment"

# Label as GitHub action
LABEL com.github.actions.name="package-diff-comment"
# Limit to 160 characters
LABEL com.github.actions.description="A Github action that adds a comments to a PR with the diff of the package and the package of the branch from which it was derived."
# See branding:
# https://docs.github.com/actions/creating-actions/metadata-syntax-for-github-actions#branding
LABEL com.github.actions.icon="git-pull-request"
LABEL com.github.actions.color="green"

# Relayer the .NET SDK, anew with the build output
FROM opentapio/opentap:beta-bionic-slim
COPY --from=build-env /out/Octokit.dll /opt/tap
COPY --from=build-env /out/Octokit.GraphQL.dll /opt/tap
COPY --from=build-env /out/Octokit.GraphQL.Core.dll /opt/tap
COPY --from=build-env /out/package-diff-comment.dll /opt/tap
RUN dotnet /opt/tap.dll package install "Package Diff" --version 0.1.0-beta.16+cae36d16
ENTRYPOINT [ "dotnet", "/opt/tap.dll" ]

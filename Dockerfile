# Set the base image as the .NET 5.0 SDK (this includes the runtime)
FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env

# Copy everything and publish the release (publish implicitly restores and builds)
COPY . ./src
RUN dotnet build ./src/package-diff-comment.csproj -c Release -o /out

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

COPY --from=build-env /out /out

ENTRYPOINT [ "dotnet", "/out/tap.dll" ]

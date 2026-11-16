namespace SqlArtisan.IntegrationTests.Infrastructure;

/// <summary>
/// A fixture backed by a Testcontainers container, which needs a Docker endpoint to start.
/// <see cref="ContainerLaneTestFramework"/> keys its skip on this marker.
/// </summary>
public interface IContainerFixture : IDatabaseFixture;

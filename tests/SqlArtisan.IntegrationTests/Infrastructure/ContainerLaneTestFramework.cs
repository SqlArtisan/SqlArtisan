using System.Reflection;
using DotNet.Testcontainers.Configurations;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: TestFramework(
    "SqlArtisan.IntegrationTests.Infrastructure.ContainerLaneTestFramework",
    "SqlArtisan.IntegrationTests")]

namespace SqlArtisan.IntegrationTests.Infrastructure;

// Without a Docker endpoint every container lane failed at its fixture, burying the SQLite and
// DB-less results; it reports those classes skipped instead. Under CI it never skips, since a
// lane skipped there would pass having run nothing.
public sealed class ContainerLaneTestFramework(IMessageSink messageSink)
    : XunitTestFramework(messageSink)
{
    private const string SkipReason =
        "No Docker endpoint is available, and this class runs against a container; "
            + "start Docker to run it, or filter on Engine=Sqlite.";

    private static readonly Lazy<bool> ContainersUnavailable = new(
        () => string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI"))
            && !HasDockerEndpoint());

    protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName) =>
        new Executor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);

    // A DOCKER_HOST Testcontainers cannot parse throws from its settings, and escaping the
    // executor that would end the run with results missing; it is a misconfiguration, not an
    // absence, so the fixtures start and fail on it as they did before this framework.
    private static bool HasDockerEndpoint()
    {
        try
        {
            return TestcontainersSettings.OS.DockerEndpointAuthConfig is not null;
        }
        catch (TypeInitializationException)
        {
            return true;
        }
    }

    private static bool NeedsContainer(Type testClass) =>
        testClass.GetInterfaces().Any(
            i => i.IsGenericType
                && i.GetGenericTypeDefinition() == typeof(IClassFixture<>)
                && typeof(IContainerFixture).IsAssignableFrom(i.GetGenericArguments()[0]));

    private sealed class Executor(
        AssemblyName assemblyName,
        ISourceInformationProvider sourceInformationProvider,
        IMessageSink diagnosticMessageSink)
        : XunitTestFrameworkExecutor(assemblyName, sourceInformationProvider, diagnosticMessageSink)
    {
        protected override async void RunTestCases(
            IEnumerable<IXunitTestCase> testCases,
            IMessageSink executionMessageSink,
            ITestFrameworkExecutionOptions executionOptions)
        {
            using AssemblyRunner runner = new(
                TestAssembly,
                testCases,
                DiagnosticMessageSink,
                executionMessageSink,
                executionOptions);
            await runner.RunAsync();
        }
    }

    private sealed class AssemblyRunner(
        ITestAssembly testAssembly,
        IEnumerable<IXunitTestCase> testCases,
        IMessageSink diagnosticMessageSink,
        IMessageSink executionMessageSink,
        ITestFrameworkExecutionOptions executionOptions)
        : XunitTestAssemblyRunner(
            testAssembly,
            testCases,
            diagnosticMessageSink,
            executionMessageSink,
            executionOptions)
    {
        protected override Task<RunSummary> RunTestCollectionAsync(
            IMessageBus messageBus,
            ITestCollection testCollection,
            IEnumerable<IXunitTestCase> testCases,
            CancellationTokenSource cancellationTokenSource) =>
            new CollectionRunner(
                testCollection,
                testCases,
                DiagnosticMessageSink,
                messageBus,
                TestCaseOrderer,
                new ExceptionAggregator(Aggregator),
                cancellationTokenSource).RunAsync();
    }

    private sealed class CollectionRunner(
        ITestCollection testCollection,
        IEnumerable<IXunitTestCase> testCases,
        IMessageSink diagnosticMessageSink,
        IMessageBus messageBus,
        ITestCaseOrderer testCaseOrderer,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource)
        : XunitTestCollectionRunner(
            testCollection,
            testCases,
            diagnosticMessageSink,
            messageBus,
            testCaseOrderer,
            aggregator,
            cancellationTokenSource)
    {
        protected override Task<RunSummary> RunTestClassAsync(
            ITestClass testClass,
            IReflectionTypeInfo @class,
            IEnumerable<IXunitTestCase> testCases)
        {
            if (!NeedsContainer(@class.Type) || !ContainersUnavailable.Value)
            {
                return base.RunTestClassAsync(testClass, @class, testCases);
            }

            return new SkippedClassRunner(
                testClass,
                @class,
                testCases,
                DiagnosticMessageSink,
                MessageBus,
                TestCaseOrderer,
                new ExceptionAggregator(Aggregator),
                CancellationTokenSource,
                CollectionFixtureMappings).RunAsync();
        }
    }

    // Creates no class fixture: starting one is the failure being avoided.
    private sealed class SkippedClassRunner(
        ITestClass testClass,
        IReflectionTypeInfo @class,
        IEnumerable<IXunitTestCase> testCases,
        IMessageSink diagnosticMessageSink,
        IMessageBus messageBus,
        ITestCaseOrderer testCaseOrderer,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource,
        IDictionary<Type, object> collectionFixtureMappings)
        : XunitTestClassRunner(
            testClass,
            @class,
            testCases,
            diagnosticMessageSink,
            messageBus,
            testCaseOrderer,
            aggregator,
            cancellationTokenSource,
            collectionFixtureMappings)
    {
        protected override Task AfterTestClassStartingAsync() => Task.CompletedTask;

        protected override object[] CreateTestClassConstructorArguments() => [];

        protected override Task<RunSummary> RunTestMethodAsync(
            ITestMethod testMethod,
            IReflectionMethodInfo method,
            IEnumerable<IXunitTestCase> testCases,
            object[] constructorArguments) =>
            new SkippedMethodRunner(
                testMethod,
                Class,
                method,
                testCases,
                DiagnosticMessageSink,
                MessageBus,
                new ExceptionAggregator(Aggregator),
                CancellationTokenSource,
                constructorArguments).RunAsync();
    }

    private sealed class SkippedMethodRunner(
        ITestMethod testMethod,
        IReflectionTypeInfo @class,
        IReflectionMethodInfo method,
        IEnumerable<IXunitTestCase> testCases,
        IMessageSink diagnosticMessageSink,
        IMessageBus messageBus,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource,
        object[] constructorArguments)
        : XunitTestMethodRunner(
            testMethod,
            @class,
            method,
            testCases,
            diagnosticMessageSink,
            messageBus,
            aggregator,
            cancellationTokenSource,
            constructorArguments)
    {
        // A skip reason makes the case runner report the test skipped without invoking it; a
        // test's own reason wins, since it is skipped with Docker running too.
        protected override Task<RunSummary> RunTestCaseAsync(IXunitTestCase testCase) =>
            new XunitTestCaseRunner(
                testCase,
                testCase.DisplayName,
                testCase.SkipReason ?? SkipReason,
                [],
                testCase.TestMethodArguments,
                MessageBus,
                new ExceptionAggregator(Aggregator),
                CancellationTokenSource).RunAsync();
    }
}

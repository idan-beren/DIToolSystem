using System.Reflection;
using DIToolSystem.DICustomContainer;

namespace DIToolSystemTests;

[TestFixture]
public class ContainerTests
{
    private IContainer _container = null!;

    [SetUp]
    public void SetUp()
    {
        _container = new Container();
    }

    public interface ITestService
    {
        Guid Id { get; }
    }

    private class TestServiceA : ITestService
    {
        public Guid Id { get; } = Guid.NewGuid();
    }

    private class TestServiceB : ITestService
    {
        public Guid Id { get; } = Guid.NewGuid();
    }

    public class ConcreteNoDeps
    {
        public int Value => 42;
    }

    private interface IRequiresService
    {
        ITestService Dep { get; }
    }

    private class RequiresServiceImpl(ITestService dep) : IRequiresService
    {
        public ITestService Dep { get; } = dep;
    }

    public class CircularA(CircularB b)
    {
        public CircularB B = b;
    }

    public class CircularB(CircularA a)
    {
        public CircularA A = a;
    }

    private interface IAssemblyService
    {
    }

    private class AssemblyImpl1 : IAssemblyService
    {
    }

    private class AssemblyImpl2 : IAssemblyService
    {
    }

    [Test]
    public void RegisterType_And_Resolve_Creates_Instance()
    {
        _container.RegisterType<TestServiceA>();
        var inst = _container.Resolve<TestServiceA>();
        Assert.That(inst, Is.Not.Null);
        Assert.That(inst, Is.InstanceOf<TestServiceA>());
    }

    [Test]
    public void Register_TImplementation_TInterface_And_Resolve_Returns_Implementation()
    {
        _container.Register<TestServiceA, ITestService>();
        var inst = _container.Resolve<ITestService>();
        Assert.That(inst, Is.Not.Null);
        Assert.That(inst, Is.InstanceOf<TestServiceA>());
    }

    [Test]
    public void RegisterFactory_And_Resolve_Uses_Factory()
    {
        _container.Register<ITestService>(_ => new TestServiceA());
        var inst = _container.Resolve<ITestService>();
        Assert.That(inst, Is.Not.Null);
        Assert.That(inst, Is.InstanceOf<TestServiceA>());
    }

    [Test]
    public void RegisterAssembly_Registers_All_Implementations_And_ResolveAll_Returns_Them()
    {
        _container.RegisterAssembly<IAssemblyService>(Assembly.GetExecutingAssembly());
        var all = _container.ResolveAll<IAssemblyService>().ToList();
        
        Assert.That(all, Has.Count.GreaterThanOrEqualTo(2));
        var types = all.Select(x => x.GetType()).ToList();
        Assert.That(types, Does.Contain(typeof(AssemblyImpl1)));
        Assert.That(types, Does.Contain(typeof(AssemblyImpl2)));
    }

    [Test]
    public void SingleInstance_Sets_Singleton_Returns_Same_Instance_On_Multiple_Resolves()
    {
        _container.Register<TestServiceA, ITestService>();
        _container.SingleInstance();
        var a = _container.Resolve<ITestService>();
        var b = _container.Resolve<ITestService>();
        Assert.That(b, Is.SameAs(a));
    }

    [Test]
    public void SingleInstance_Throws_When_No_LastRegistration()
    {
        var fresh = new Container();
        var ex = Assert.Throws<InvalidOperationException>(() => fresh.SingleInstance());
        StringAssert.Contains("No registration available to set as single instance", ex!.Message);
    }

    [Test]
    public void Named_Allows_Registering_Name_And_ResolveNamed_Returns_Instance()
    {
        _container.Register<TestServiceA, ITestService>();
        _container.Named("my-name");
        var inst = _container.ResolveNamed<ITestService>("my-name");
        Assert.That(inst, Is.Not.Null);
        Assert.That(inst, Is.InstanceOf<TestServiceA>());
    }

    [Test]
    public void Named_Throws_When_No_LastRegistration()
    {
        var fresh = new Container();
        var ex = Assert.Throws<InvalidOperationException>(() => fresh.Named("x"));
        StringAssert.Contains("No registration available to name", ex!.Message);
    }

    [Test]
    public void Resolve_Throws_When_No_Registration_For_Interface()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => _container.Resolve<ITestService>());
        StringAssert.Contains("No registration available to resolve", ex!.Message);
    }

    [Test]
    public void ResolveNamed_Throws_When_NotFound()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => _container.ResolveNamed<ITestService>("no-such"));
        StringAssert.Contains("No registration available to name", ex!.Message);
    }

    [Test]
    public void ResolveAll_Yields_Registered_Implementations()
    {
        _container.Register<TestServiceA, ITestService>();
        _container.Register<TestServiceB, ITestService>();
        var all = _container.ResolveAll<ITestService>().ToList();
        Assert.That(all, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(all.Any(x => x.GetType() == typeof(TestServiceA)), Is.True);
            Assert.That(all.Any(x => x.GetType() == typeof(TestServiceB)), Is.True);
        });
    }

    [Test]
    public async Task ResolveAsync_Returns_Task_With_Instance()
    {
        _container.Register<TestServiceA, ITestService>();
        var task = _container.ResolveAsync<ITestService>();
        Assert.That(task, Is.InstanceOf<Task<ITestService>>());
        var inst = await task;
        Assert.That(inst, Is.InstanceOf<TestServiceA>());
    }

    [Test]
    public void Lazy_Resolution_Works_For_Registered_Interface()
    {
        _container.Register<TestServiceA, ITestService>();
        _container.RegisterType<Lazy<ITestService>>();

        var lazy = _container.Resolve<Lazy<ITestService>>();

        Assert.That(lazy, Is.Not.Null);
        var value = lazy.Value;
        Assert.That(value, Is.InstanceOf<TestServiceA>());
    }

    [Test]
    public void Resolving_Unregistered_Interface_Or_Abstract_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => _container.Resolve<ITestService>());
    }

    [Test]
    public void Multiple_Registrations_For_Same_Service_Cause_MultipleRegistrationException_On_Resolve()
    {
        _container.Register<TestServiceA, ITestService>();
        _container.Register<TestServiceB, ITestService>();

        var ex = Assert.Throws<InvalidOperationException>(() => _container.Resolve<ITestService>());
        StringAssert.Contains("Multiple registrations found for type", ex!.Message);
    }

    [Test]
    public void Circular_Dependency_Throws_CircularDependencyException()
    {
        _container.RegisterType<CircularA>();
        _container.RegisterType<CircularB>();

        var ex = Assert.Throws<InvalidOperationException>(() => _container.Resolve<CircularA>());
        StringAssert.Contains("Circular dependency detected", ex!.Message);
    }

    [Test]
    public void Factory_Singleton_Behavior_Works()
    {
        _container.Register<ITestService>(_ => new TestServiceA()).SingleInstance();
        var a = _container.Resolve<ITestService>();
        var b = _container.Resolve<ITestService>();
        Assert.That(b, Is.SameAs(a));
    }

    [Test]
    public void Resolve_Instance_With_Dependencies_Resolves_Constructor_Injection()
    {
        _container.Register<TestServiceA, ITestService>();
        _container.Register<RequiresServiceImpl, IRequiresService>();

        var resolved = _container.Resolve<IRequiresService>();
        Assert.That(resolved, Is.Not.Null);
        Assert.That(resolved, Is.InstanceOf<RequiresServiceImpl>());
        Assert.That(resolved.Dep, Is.InstanceOf<TestServiceA>());
    }
}